using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LSL;
using System;

public class LSLDevice : NeuroDevice
{
    // LSL Inlet
    public StreamInlet eegInlet;
    public StreamInlet inferenceInlet;
    // LSL Outlet
    public StreamOutlet outlet;
    public string EEGStreamNameIn = "CortexEEG";
    public string InferenceStreamNameIn = "CortexInference";
    public string StreamNameOut = "CortexMarkers";
    private StreamInfo[] inStreamInfo;
    private const string StreamType = "Markers";
    private const int ChannelCount = 1; // Single channel for markers
    private const double SampleRate = 0; // Irregular sampling rate
    private const channel_format_t ChannelFormat = channel_format_t.cf_string;
    private double[] eeg_sample;
    private double[] inference_sample;

    private double prev_ts = 0;

    public LSLDevice()
    {
        OnRawDataReceived = new UnityEvent<double[], double>();
        OnInferenceReceived = new UnityEvent<double[], double>();
        InitMarkerStream();
    }
    
    public LSLDevice(string eegStreamInName, string inferenceStreamInName, string markersStreamOutName)
    {
        OnRawDataReceived = new UnityEvent<double[], double>();
        OnInferenceReceived = new UnityEvent<double[], double>();
        EEGStreamNameIn = eegStreamInName;
        InferenceStreamNameIn = inferenceStreamInName;
        StreamNameOut = markersStreamOutName;
        InitMarkerStream();
    }

    private void InitDataStream()
    {
        inStreamInfo = LSL.LSL.resolve_stream("name", EEGStreamNameIn, timeout: 1000);
        if (inStreamInfo.Length > 0)
        {
            Debug.Log("Found LSL stream with name: " + EEGStreamNameIn);
            eegInlet = new StreamInlet(inStreamInfo[0]);
            eeg_sample = new double[inStreamInfo[0].channel_count()]; // Adjust to the number of channels in your stream
        }
        else
        {
            Debug.LogError("No LSL stream found with the name: " + EEGStreamNameIn);
        }
    }

    private void InitInferenceStream()
    {
        inStreamInfo = LSL.LSL.resolve_stream("name", InferenceStreamNameIn, timeout: 1000);
        if (inStreamInfo.Length > 0)
        {
            Debug.Log("Found LSL stream with name: " + InferenceStreamNameIn);
            inferenceInlet = new StreamInlet(inStreamInfo[0]);
            inference_sample = new double[inStreamInfo[0].channel_count()]; // Adjust to the number of channels in your stream
        }
        else
        {
            Debug.LogError("No LSL stream found with the name: " + InferenceStreamNameIn);
        }
    }

    private void InitMarkerStream()
    {
        try
        {
            StreamInfo streamInfo = new StreamInfo(EEGStreamNameIn, StreamType, ChannelCount, SampleRate, ChannelFormat, StreamNameOut);
            outlet = new StreamOutlet(streamInfo);
            Debug.Log("Started LSL stream with name: " + StreamNameOut);
        }
        catch (Exception e)
        {
            Debug.LogError("Error opening LSL stream: " + e.Message);
        }
    }

    public override void GetDataStream()
    {
        if (eegInlet == null)
        {
            InitDataStream();
        }
        if (eegInlet != null)
        {
            double timestamp = eegInlet.pull_sample(eeg_sample, timeout: 0.0f);
            if (timestamp != 0)
            {
                OnRawDataReceived.Invoke(eeg_sample, timestamp);
            }
        }
    }

    public override void GetInferenceStream()
    {
        if (inferenceInlet == null)
        {
            InitInferenceStream();
        }
        if (inferenceInlet != null)
        {
            double timestamp = inferenceInlet.pull_sample(inference_sample, timeout:0.0f);
            if (timestamp != 0)
            {
                OnInferenceReceived.Invoke(inference_sample, timestamp);
            }
        }
    }

    public override void SendMarker(string marker)
    {
        if (outlet == null)
        {
            InitMarkerStream();
        }
        double ts = LSL.LSL.local_clock();
        string[] sample = new string[] { marker };
        outlet.push_sample(sample, ts);
        double delta = ts - prev_ts;
        prev_ts = ts;
        Debug.Log("Sent marker: " + marker + " with delta: " + Math.Round((float)delta, 2));
    }

    void OnApplicationQuit()
    {
        if (eegInlet != null)
        {
            eegInlet.close_stream();
        }

        if (inferenceInlet != null)
        {
            inferenceInlet.close_stream();
        }
    }
}

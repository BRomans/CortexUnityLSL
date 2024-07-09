using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LSL;
using System;

public class LSLDevice : NeuroDevice
{
    // LSL Inlet
    public StreamInlet inlet;
    public StreamOutlet outlet;
    public string StreamNameIn = "myeeg";
    public string StreamNameOut = "CortexUnity";
    private StreamInfo[] inStreamInfo;
    private const string StreamType = "Markers";
    private const int ChannelCount = 1; // Single channel for markers
    private const double SampleRate = 0; // Irregular sampling rate
    private const channel_format_t ChannelFormat = channel_format_t.cf_string;
    private double[] sample;

    private double prev_ts = 0;

    public LSLDevice()
    {
        OnDataReceived = new UnityEvent<double[], double>();
        InitMarkerStream();
    }
    private void InitDataStream()
    {
        inStreamInfo = LSL.LSL.resolve_stream("name", StreamNameIn, timeout:1000);
        if (inStreamInfo.Length > 0)
        {
            Debug.Log("Found LSL stream with name: " + StreamNameIn);
            inlet = new StreamInlet(inStreamInfo[0]);
            sample = new double[inStreamInfo[0].channel_count()]; // Adjust to the number of channels in your stream
        }
        else
        {
            Debug.LogError("No LSL stream found with the name: " + StreamNameIn);
        }
    }

    private void InitMarkerStream()
    {
        try
        {
            StreamInfo streamInfo = new StreamInfo(StreamNameIn, StreamType, ChannelCount, SampleRate, ChannelFormat, StreamNameOut);
            outlet = new StreamOutlet(streamInfo);
            Debug.Log("Started LSL stream with name: " + StreamNameOut);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error opening LSL stream: " + e.Message);
        }
    }

    public override void GetDataStream()
    {
        if (inlet == null)
        {
            InitDataStream();
        }
        if (inlet != null)
        {
            
            if (inlet.pull_sample(sample, 0.0f) > 0)
            {
                double timestamp = inlet.info().created_at();
                OnDataReceived.Invoke(sample, timestamp);
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
        if (inlet != null)
        {
            inlet.close_stream();
        }
    }
}

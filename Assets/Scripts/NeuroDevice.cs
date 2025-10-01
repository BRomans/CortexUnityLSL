using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public abstract class NeuroDevice
{
    public enum Device {LSL}
    public UnityEvent<double[], double> OnRawDataReceived;
    public UnityEvent<double[], double> OnInferenceReceived;
    public abstract void GetDataStream();
    public abstract void GetInferenceStream();
    public abstract void SendMarker(string marker);
}
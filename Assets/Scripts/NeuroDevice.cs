using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public abstract class NeuroDevice
{
    public enum Device {LSL}
    public UnityEvent<double[], double> OnDataReceived;
    public abstract void GetDataStream();
    public abstract void SendMarker(string marker);
}
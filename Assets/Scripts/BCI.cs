using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BCI : MonoBehaviour
{

    public UnityEvent<double[], double> OnDataReceived;
    private  NeuroDevice Device;

    private bool StreamEEG = false;

    // Start is called before the first frame update
    void Start()
    {
        Device = new LSLDevice();
        Device.OnDataReceived.AddListener(ReceiveData);
    }

    void Update()
    {
        if (StreamEEG)
        {
            Device.GetDataStream();
        }
    }

    void ReceiveData(double[] data, double timestamp)
    {
        Debug.Log("Received sample: " + string.Join(", ", data));
        OnDataReceived.Invoke(data, timestamp);
    }

    public void SendMarker(string marker)
    {
        Device.SendMarker(marker);
    }

    public void StartStopStream()
    {
        StreamEEG = !StreamEEG;
    }

}

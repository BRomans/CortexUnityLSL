using UnityEngine;
using UnityEngine.Events;

public class BCI : MonoBehaviour
{

    public UnityEvent<double[], double> OnRawDataReceived;
    public UnityEvent<double[], double> OnInferenceReceived;
    private NeuroDevice Device;

    private bool StreamEEG = false;
    private double firstInferenceTimestamp = 0.0;
    private double firstRawTimestamp = 0.0;

    // Start is called before the first frame update
    void Start()
    {
        Device = new LSLDevice();
        Device.OnRawDataReceived.AddListener(ReceiveRawData);
        Device.OnInferenceReceived.AddListener(ReceiveInferenceData);
    }

    void Update()
    {
        if (StreamEEG)
        {
            Device.GetDataStream();
            Device.GetInferenceStream();
        }
    }

    void ReceiveRawData(double[] data, double timestamp)
    {
        if (firstRawTimestamp == 0.0)
        {
            firstRawTimestamp = timestamp;
        }

        double relativeTime = timestamp - firstRawTimestamp;

        string formattedData = string.Format(
            "CortexEEG [{0:F2}s] " +
            "  Ch1: {1:F3} " +
            "  Ch2: {2:F3} " +
            "  Ch3: {3:F3} " +
            "  Ch4: {4:F3} " +
            "  Ch5: {5:F3} " +
            "  Ch6: {6:F3} " +
            "  Ch7: {7:F3} " +
            "  Ch8: {8:F3}",
            relativeTime,
            data[0],
            data[1],
            data[2],
            data[3],
            data[4],
            data[5],
            data[6],
            data[7]
        );
        OnRawDataReceived.Invoke(data, timestamp);
    }

    void ReceiveInferenceData(double[] data, double timestamp)
    {
        if (firstInferenceTimestamp == 0.0)
        {
            firstInferenceTimestamp = timestamp;
        }

        double relativeTime = timestamp - firstInferenceTimestamp;

        // Format the cognitive state data nicely
        string formattedData = string.Format(
            "CortexInference [{0:F2}s] " +
            "  Arousal: {1:F3} " +
            "  Valence: {2:F3} " +
            "  Focus:   {3:F3} " +
            "  Calm:    {4:F3}",
            relativeTime,
            data[0],
            data[1],
            data[2],
            data[3]
        );

        Debug.Log(formattedData);
        OnInferenceReceived.Invoke(data, timestamp);
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

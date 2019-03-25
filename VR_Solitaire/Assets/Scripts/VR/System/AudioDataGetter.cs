using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get realtime audio data with SimpleSpectrum
/// </summary>
public class AudioDataGetter : MonoBehaviour
{
    public SimpleSpectrum simpleSpectrumInstance; // The SimpleSpectrum in the scene for us to use to get 
    public int minSampleRange; // The smallest sample range for us to take the average
    public int maxSampleRange;

    public float avgAmp; // The average realtime audio amplitude taken from the the selected range

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        avgAmp = GetAvgAmp();
    }

    /// <summary>
    /// Get the average amplitude from the selected range of sample
    /// </summary>
    /// <returns></returns>
    public float GetAvgAmp()
    {
        float sampleSum = 0; // The sum of all the chosen sample amplitudes

        for (int i = minSampleRange; i < maxSampleRange; i++)
        {
            sampleSum += simpleSpectrumInstance.oldYScalesCopy[i];
        }

        return sampleSum / (float)(maxSampleRange - minSampleRange);
    }
}

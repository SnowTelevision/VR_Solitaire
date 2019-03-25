using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpectralFluxInfo
{
    public float time;
    public float spectralFlux;
    public float threshold;
    public float prunedSpectralFlux;
    public bool isPeak;
}

public class SpectralFluxAnalyzer
{
    int numSamples = 1024;

    // Sensitivity multiplier to scale the average threshold.
    // In this case, if a rectified spectral flux sample is > 1.5 times the average, it is a peak
    float thresholdMultiplier = 1.5f;

    // Number of samples to average in our window
    int thresholdWindowSize = 50;

    public List<SpectralFluxInfo> SpectralFluxSamples;

    float[] curSpectrum;
    float[] prevSpectrum;

    int indexToProcess;

    public bool IsDone { get; set; }

    public SpectralFluxAnalyzer()
    {
        SpectralFluxSamples = new List<SpectralFluxInfo>();

        IsDone = false;

        // Start processing from middle of first window and increment by 1 from there
        indexToProcess = thresholdWindowSize / 2;

        curSpectrum = new float[numSamples];
        prevSpectrum = new float[numSamples];
    }

    public void SetCurSpectrum(float[] spectrum)
    {
        curSpectrum.CopyTo(prevSpectrum, 0);
        spectrum.CopyTo(curSpectrum, 0);
    }

    public async Task AnalyzeSpectrumAsync(float[] spectrum, float time)
    {
        // Set spectrum
        SetCurSpectrum(spectrum);

        // Get current spectral flux from spectrum
        SpectralFluxInfo curInfo = new SpectralFluxInfo();
        curInfo.time = time;
        curInfo.spectralFlux = CalculateRectifiedSpectralFlux();
        SpectralFluxSamples.Add(curInfo);

        // We have enough samples to detect a peak
        if (SpectralFluxSamples.Count >= thresholdWindowSize)
        {
            // Get Flux threshold of time window surrounding index to process
            SpectralFluxSamples[indexToProcess].threshold = GetFluxThreshold(indexToProcess);

            // Only keep amp amount above threshold to allow peak filtering
            SpectralFluxSamples[indexToProcess].prunedSpectralFlux = GetPrunedSpectralFlux(indexToProcess);

            // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
            int indexToDetectPeak = indexToProcess - 1;

            bool curPeak = IsPeak(indexToDetectPeak);

            if (curPeak)
            {
                SpectralFluxSamples[indexToDetectPeak].isPeak = true;
            }

            indexToProcess++;
        }
        else
        {
            Debug.Log(string.Format("Not ready yet.  At spectral flux sample size of {0} growing to {1}", SpectralFluxSamples.Count, thresholdWindowSize));
        }
    }

    public void AnalyzeSpectrum(float[] spectrum, float time)
    {
        // Set spectrum
        SetCurSpectrum(spectrum);

        // Get current spectral flux from spectrum
        SpectralFluxInfo curInfo = new SpectralFluxInfo();
        curInfo.time = time;
        curInfo.spectralFlux = CalculateRectifiedSpectralFlux();
        SpectralFluxSamples.Add(curInfo);

        // We have enough samples to detect a peak
        if (SpectralFluxSamples.Count >= thresholdWindowSize)
        {
            // Get Flux threshold of time window surrounding index to process
            SpectralFluxSamples[indexToProcess].threshold = GetFluxThreshold(indexToProcess);

            // Only keep amp amount above threshold to allow peak filtering
            SpectralFluxSamples[indexToProcess].prunedSpectralFlux = GetPrunedSpectralFlux(indexToProcess);

            // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
            int indexToDetectPeak = indexToProcess - 1;

            bool curPeak = IsPeak(indexToDetectPeak);

            if (curPeak)
            {
                SpectralFluxSamples[indexToDetectPeak].isPeak = true;
            }

            indexToProcess++;
        }
        else
        {
            Debug.Log(string.Format("Not ready yet.  At spectral flux sample size of {0} growing to {1}", SpectralFluxSamples.Count, thresholdWindowSize));
        }
    }

    public List<float> GetSpectrumData()
    {
        var spectrum = new List<float>();
        if (SpectralFluxSamples.Count > 0)
        {

            foreach (var sample in SpectralFluxSamples)
            {
                spectrum.Add(sample.spectralFlux);
            }

        }

        return spectrum;
        //return await LoadSpectrumDataAsync(new List<float>());
    }

    public IEnumerator LoadSpectrumDataAsync(List<float> spectrum)
    {
        yield return spectrum;
    }

    float CalculateRectifiedSpectralFlux()
    {
        float sum = 0f;

        // Aggregate positive changes in spectrum data
        for (int i = 0; i < numSamples; i++)
        {
            sum += Mathf.Max(0f, curSpectrum[i] - prevSpectrum[i]);
        }
        return sum;
    }

    float GetFluxThreshold(int spectralFluxIndex)
    {
        // How many samples in the past and future we include in our average
        int windowStartIndex = Mathf.Max(0, spectralFluxIndex - thresholdWindowSize / 2);
        int windowEndIndex = Mathf.Min(SpectralFluxSamples.Count - 1, spectralFluxIndex + thresholdWindowSize / 2);

        // Add up our spectral flux over the window
        float sum = 0f;
        for (int i = windowStartIndex; i < windowEndIndex; i++)
        {
            sum += SpectralFluxSamples[i].spectralFlux;
        }

        // Return the average multiplied by our sensitivity multiplier
        float avg = sum / (windowEndIndex - windowStartIndex);
        return avg * thresholdMultiplier;
    }

    float GetPrunedSpectralFlux(int spectralFluxIndex)
    {
        return Mathf.Max(0f, SpectralFluxSamples[spectralFluxIndex].spectralFlux - SpectralFluxSamples[spectralFluxIndex].threshold);
    }

    bool IsPeak(int spectralFluxIndex)
    {
        if (SpectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > SpectralFluxSamples[spectralFluxIndex + 1].prunedSpectralFlux &&
            SpectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > SpectralFluxSamples[spectralFluxIndex - 1].prunedSpectralFlux)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void LogSample(int indexToLog)
    {
        int windowStart = Mathf.Max(0, indexToLog - thresholdWindowSize / 2);
        int windowEnd = Mathf.Min(SpectralFluxSamples.Count - 1, indexToLog + thresholdWindowSize / 2);
        Debug.Log(string.Format(
            "Peak detected at song time {0} with pruned flux of {1} ({2} over thresh of {3}).\n" +
            "Thresh calculated on time window of {4}-{5} ({6} seconds) containing {7} samples.",
            SpectralFluxSamples[indexToLog].time,
            SpectralFluxSamples[indexToLog].prunedSpectralFlux,
            SpectralFluxSamples[indexToLog].spectralFlux,
            SpectralFluxSamples[indexToLog].threshold,
            SpectralFluxSamples[windowStart].time,
            SpectralFluxSamples[windowEnd].time,
            SpectralFluxSamples[windowEnd].time - SpectralFluxSamples[windowStart].time,
            windowEnd - windowStart
        ));
    }
}

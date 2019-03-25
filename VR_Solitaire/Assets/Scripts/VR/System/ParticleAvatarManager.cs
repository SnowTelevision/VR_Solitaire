using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAvatarManager : MonoBehaviour
{
    public ParticleSystemRenderer psr; // The ParticleSystemRenderer on the particle avatar
    public AudioDataGetter audioAmplitude; // The getter for the current playing audio's amplitude
    public float maxAudioEffectAmplitude; // How much to we clamp the max audio amplitude (any value above it will be clampped to this value)
    public float maxParticleEmission; // The maximum emission strength of the particle avatar
    public float minParticleAlpha; // The minimum particle alpha

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAvatarEmission();
    }

    public void UpdateAvatarEmission()
    {
        // Calculate the current emission strength
        float targetEmission = (Mathf.Clamp(audioAmplitude.avgAmp, 0, maxAudioEffectAmplitude) / maxAudioEffectAmplitude) * maxParticleEmission;

        if (psr.material.HasProperty("_EmissionColor"))
        {
            psr.material.SetColor("_EmissionColor", GetHDRcolor.GetColorInHDR(psr.material.color, targetEmission));
        }

        Color targetColor = psr.material.GetColor("_TintColor");
        targetColor.a = Mathf.Clamp(Mathf.Clamp(audioAmplitude.avgAmp, 0, maxAudioEffectAmplitude) / maxAudioEffectAmplitude, minParticleAlpha, 1);
        psr.material.SetColor("_TintColor", targetColor);
    }
}

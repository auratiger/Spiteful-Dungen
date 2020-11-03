using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TorchFlicker : MonoBehaviour
{
    [SerializeField]
    public Light2D light;
    
    [SerializeField]
    private float intensityJumpScale = .5f;
    
    private float intensityBase;
    private float intensityScrollSpeed = 1f;
    
    private void Start()
    {
        intensityBase = light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        light.intensity = NewIntensity(intensityBase, intensityJumpScale, intensityScrollSpeed);
    }

    private float NewIntensity(float intensityBase, float intensityJumpScale, float intensityScrollSpeed)
    {
        return (intensityBase + (intensityJumpScale *
                                 Mathf.PerlinNoise(Time.time * intensityScrollSpeed,
                                     1f + Time.time * intensityScrollSpeed)));
    }
}

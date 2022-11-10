using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator {

    public MinMax elevationMinMax;

    private ShapeSettings settings;
    private INoiseFilter[] noiseFilters;


    public void UpdateSettings(ShapeSettings _settings) {

        settings = _settings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];

        for(int i = 0; i < noiseFilters.Length; i++) {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }

        elevationMinMax = new MinMax();

    }

    public float CalculateUnscaledElevation(Vector3 _pointOnUnitSphere) {

        float firstLayerValue = 0;
        float elevation = 0;

        if(noiseFilters.Length > 0) {
            firstLayerValue = noiseFilters[0].Evaluate(_pointOnUnitSphere);
            if(settings.noiseLayers[0].enabled) {
                elevation = firstLayerValue;
            }
        }

        for(int i = 1; i < noiseFilters.Length; i++) {
            if(settings.noiseLayers[i].enabled) {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(_pointOnUnitSphere) * mask;
            }
        }

        elevationMinMax.AddValue(elevation);

        return elevation;

    }

    public float GetScaledElevation(float _unscaledElevation) {
        float elevation = Mathf.Max(0,_unscaledElevation);
        elevation = settings.planetRadius * (1 + elevation);
        return elevation;
    }
}

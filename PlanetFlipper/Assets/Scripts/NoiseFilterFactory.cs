using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory {

    public static INoiseFilter CreateNoiseFilter(NoiseSettings _settings) {

        switch(_settings.filterType) {

            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(_settings.simpleNoiseSettings);

            case NoiseSettings.FilterType.Ridgid:
                return new RidgidNoiseFilter(_settings.ridgidNoiseSettings);
            
        }

        return null;

    }

}

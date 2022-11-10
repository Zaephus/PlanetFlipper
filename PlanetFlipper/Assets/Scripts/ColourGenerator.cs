using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator {

    private ColourSettings settings;
    private Texture2D texture;
    private const int textureResolution = 50;
    private INoiseFilter biomeNoiseFilter;

    public void UpdateSettings(ColourSettings _settings) {

        settings = _settings;
        
        if(texture == null || texture.height != settings.biomeColourSettings.biomes.Length) {
            texture = new Texture2D(textureResolution * 2, settings.biomeColourSettings.biomes.Length, TextureFormat.RGBA32, false);
        }

        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColourSettings.noise);

    }

    public void UpdateElevation(MinMax _elevationMinMax) {
        settings.planetMaterial.SetVector("_ElevationMinMax", new Vector4(_elevationMinMax.Min, _elevationMinMax.Max));
    }

    public float BiomePercentFromPoint(Vector3 _pointOnUnitSphere) {
        
        float heightPercent = (_pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(_pointOnUnitSphere) - settings.biomeColourSettings.noiseOffset) * settings.biomeColourSettings.noiseStrength;

        float biomeIndex = 0;
        int numBiomes = settings.biomeColourSettings.biomes.Length;

        float blendRange = settings.biomeColourSettings.blendAmount / 2 + 0.001f;

        for(int i = 0; i < numBiomes; i++) {

            float dist = heightPercent - settings.biomeColourSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dist);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;

        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);

    }

    public void UpdateColours() {

        Color[] colours = new Color[texture.width * texture.height];

        int colourIndex = 0;

        foreach(var biome in settings.biomeColourSettings.biomes) {
        
            for(int i = 0; i < textureResolution * 2; i++) {

                Color gradientCol;
                if(i < textureResolution) {
                    gradientCol = settings.oceanColour.Evaluate(i / (textureResolution - 1f));
                }
                else {
                    gradientCol = biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                }
                Color tintCol = biome.tint;
                colours[colourIndex] = gradientCol * (1 - biome.tintPercent) + tintCol * biome.tintPercent;
                colourIndex ++;

            }

        }

        texture.SetPixels(colours);
        texture.Apply();

        settings.planetMaterial.SetTexture("_PlanetTexture", texture);

    }

}

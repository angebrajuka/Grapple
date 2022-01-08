using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Decor
{
    public string name;
    public float frequency;
}

[System.Serializable]
public class JsonBiome
{
    public string name;
    public string color;
    public string rain_temp_map_color;
    public Decor[] decorations;
}

[System.Serializable]
public class BiomesJson
{
    public JsonBiome[] biomes;
}

public struct Biome
{
    public static Dictionary<string, GameObject> s_decorations = new Dictionary<string, GameObject>();

    public static void Init()
    {
        var decorations = Resources.LoadAll<GameObject>("Decorations");

        s_decorations.Clear();
        for(int i=0; i<decorations.Length; i++)
        {
            s_decorations.Add(decorations[i].name, decorations[i]);
        }
    }

    public GameObject[] decorations;
    public float[] decorationThreshholds;
    public Color color;

    public Biome(JsonBiome jsonBiome)
    {
        decorations = new GameObject[jsonBiome.decorations.Length];
        ColorUtility.TryParseHtmlString(jsonBiome.color, out color);
        decorationThreshholds = new float[jsonBiome.decorations.Length];

        // for(int i=0; i<decorations.Length; i++)
        // {
        //     decorations[i] = s_decorations[jsonBiome.decorations[i].name];
        //     decorationThreshholds[i] = jsonBiome.decorations[i].frequency;
        //     decorationThreshholds[i] += (i > 0 ? decorationThreshholds[i-1] : 0);
        // }
    }
}
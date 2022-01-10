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
public class BiomeData
{
    public string name;
    public string color;
    public float height_min, height_max;
    // public float scale
    public string rain_temp_map_color;
    public Decor[] decorations;
    public float density;
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
            var name = decorations[i].name;
            s_decorations.Add(name, decorations[i]);
        }
    }

    public GameObject[] decorations;
    public float[] decorationThreshholds;
    public Color color;

    public Biome(BiomeData biomeData)
    {
        decorations = new GameObject[biomeData.decorations.Length];
        ColorUtility.TryParseHtmlString(biomeData.color, out color);
        decorationThreshholds = new float[biomeData.decorations.Length];

        for(int i=0; i<decorations.Length; i++)
        {
            var name = biomeData.decorations[i].name;
            decorations[i] = s_decorations[s_decorations.ContainsKey(name) ? name : "P_Tree_Oak"];
            decorationThreshholds[i] = biomeData.decorations[i].frequency;
            if(i > 0) decorationThreshholds[i] += decorationThreshholds[i-1];
        }
        for(int i=0; i<decorations.Length; i++)
        {
            decorationThreshholds[i] /= decorationThreshholds[decorationThreshholds.Length-1];
        }
    }
}
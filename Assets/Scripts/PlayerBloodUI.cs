using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerBloodUI : MonoBehaviour
{
    public static PlayerBloodUI instance;

    // hierarchy
    public GameObject prefab_splatter;

    static Texture2D[] splatter_textures;
    static AudioClip[] clips;
    public static LinkedList<(Vector2 pos, int index)> splatters;

    public void Init()
    {
        instance = this;

        splatter_textures = Resources.LoadAll<Texture2D>("Sprites/BloodSplatters");
        clips = Resources.LoadAll<AudioClip>("Audio/BloodSplatterSounds");
        splatters = new LinkedList<(Vector2 pos, int index)>();
    }

    public static void AddSplatter(Vector2 position=default(Vector2), int index=-1, bool sound=true)
    {
        if(position == default(Vector2))
        {
            position = new Vector2(Random.value*Screen.width, Random.value*Screen.height);
        }
        if(index == -1) index = Random.Range(0, splatter_textures.Length);

        var splatter = Instantiate(instance.prefab_splatter, position, Quaternion.identity, instance.transform);
        splatter.GetComponent<RawImage>().texture = splatter_textures[index];
        if(sound) AudioManager.PlayClip(clips[Random.Range(0, clips.Length)]);

        splatters.AddLast((position, index));
    }
}
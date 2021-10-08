using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour
{
    // components
    private AudioSource audioSource;


    private AudioClip[] clips;
    private int clip;

    public void Init()
    {
        audioSource = GetComponent<AudioSource>();

        // clips = Resources.LoadAll<AudioClip>("Music");

        // Shuffle();

        // audioSource is play on awake, initial clip is record scratch
    }

    public void Shuffle()
    {
        AudioClip oldLast = clips[clips.Length-1];

        for(int i=0; i<clips.Length-2; i++)
        {
            int value = Random.Range(i+1, clips.Length);
            AudioClip temp = clips[i];
            clips[i] = clips[value];
            clips[value] = temp;
        }

        if(clips[0] == oldLast)
        {
            clips[0] = clips[1];
            clips[1] = oldLast;
        }

        clip = 0;
    }

    void PlayClip()
    {
        audioSource.clip = clips[clip];
        audioSource.Play();
        clip ++;

        if(clip == clips.Length)
        {
            Shuffle();
        }
    }

    void Update()
    {
        if(!audioSource.isPlaying)
        {
            // PlayClip();
        }
    }
}
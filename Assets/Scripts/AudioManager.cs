using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum Mixer
{
    MUSIC,
    SFX,
    MENU
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public static float volMaster, volMusic, volSFX, volMenu;

    // hierarchy
    public AudioMixer mixer;
    public AudioMixerGroup mixer_master;
    public AudioMixerGroup mixer_music;
    public AudioMixerGroup mixer_sfx;
    public AudioMixerGroup mixer_menu;

    public void Init()
    {
        instance = this;
        LoadAudioSettings();
    }

    public static void DefaultSettings()
    {
        volMaster = 0.5f;
        volMusic = 0.5f;
        volSFX = 0.5f;
        volMenu = 0.5f;
    }

    public static void UpdateAudioSettings()
    {
        instance.mixer.SetFloat("Vol_Master", Mathf.Log10(volMaster) * 20);
        instance.mixer.SetFloat("Vol_Music", Mathf.Log10(volMusic) * 20);
        instance.mixer.SetFloat("Vol_SFX", Mathf.Log10(volSFX) * 20);
        instance.mixer.SetFloat("Vol_Menu", Mathf.Log10(volMenu) * 20);
        SaveAudioSettings();
    }

    public static void LoadAudioSettings()
    {
        try
        {
            volMaster = PlayerPrefs.GetFloat("VolMaster");
            volMusic = PlayerPrefs.GetFloat("VolMusic");
            volSFX = PlayerPrefs.GetFloat("VolSFX");
            volMenu = PlayerPrefs.GetFloat("VolMenu");
        }
        catch
        {
            DefaultSettings();
        }
        UpdateAudioSettings();
    }

    public static void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("VolMaster", volMaster);
        PlayerPrefs.SetFloat("VolMusic", volMusic);
        PlayerPrefs.SetFloat("VolSFX", volSFX);
        PlayerPrefs.SetFloat("VolMenu", volMenu);
    }

    public static void PitchShift(float pitch)
    {
        instance.mixer.SetFloat("PitchSFX", pitch);
    }

    static AudioSource[] sources;
    public static void PauseAllAudio()
    {
        sources = Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        for(int i=0; i<sources.Length; i++) {
            if(!sources[i].isPlaying || sources[i].transform.tag == "Music") {
                sources[i] = null;
            } else {
                sources[i].Pause();
                var comp = sources[i].gameObject.GetComponent<DestroyWhenDonePlaying>();
                if(comp != null)
                {
                    comp.paused = true;
                }
            }
        }
    }
    public static void ResumeAllAudio()
    {
        foreach(var source in sources)
        {
            if(source != null)
            {
                source.UnPause();
                var comp = source.gameObject.GetComponent<DestroyWhenDonePlaying>();
                if(comp != null)
                {
                    comp.paused = false;
                }
            }
        }
    }

    public static GameObject PlayClip(AudioClip clip, float volume=1, Mixer mixer=Mixer.SFX, float spatialBlend=0.0f, Vector3 position=default(Vector3), bool destroy=true)
    {
        GameObject gameObject = new GameObject();
        gameObject.transform.parent = instance.transform;
        gameObject.transform.position = position;
        AudioSource source = gameObject.AddComponent<AudioSource>();
        switch(mixer)
        {
        case Mixer.SFX:
            source.outputAudioMixerGroup = instance.mixer_sfx;
            break;
        case Mixer.MENU:
            source.outputAudioMixerGroup = instance.mixer_menu;
            break;
        case Mixer.MUSIC:
            source.outputAudioMixerGroup = instance.mixer_music;
            break;
        }
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = spatialBlend;
        source.Play();
        if(destroy)
        {
            var d = gameObject.AddComponent<DestroyWhenDonePlaying>();
            d.s = source;
        }
        return gameObject;
    }
}

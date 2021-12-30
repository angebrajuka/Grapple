using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    public AudioClip clipHover;
    public AudioClip clipClick;
    public int diff;

    Text textComp;
    int originalTextSize;

    void Start()
    {
        textComp = transform.GetChild(0).GetComponent<Text>();
        originalTextSize = textComp.fontSize;
    }

    public void OnEnter()
    {
        AudioManager.PlayClip(clipHover, 0.5f, Mixer.MENU);
        textComp.fontSize = originalTextSize + diff;
    }

    public void OnExit()
    {
        textComp.fontSize = originalTextSize;
    }

    public void OnClick()
    {
        AudioManager.PlayClip(clipClick, 1.0f, Mixer.MENU);
    }
}

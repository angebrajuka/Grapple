using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public AudioClip clipHover;
    public AudioClip clipClick;
    public int diff;
    public bool toggle, atLeastOne, defaultSelected;
    public Color selectedColor;
    public bool deselectSiblings;
    public MenuButton[] deselectOnClick;


    Text textComp;
    int originalTextSize;
    Color originalColor;

    bool p_selected;
    [HideInInspector] public bool Selected
    {
        get { return p_selected; }
        set
        {
            p_selected = value;
            GetComponent<Image>().color = p_selected ? selectedColor : originalColor;
        }
    }

    void Start()
    {
        textComp = transform.GetChild(0).GetComponent<Text>();
        if(textComp != null) originalTextSize = textComp.fontSize;
        var img = GetComponent<Image>();
        if(img != null) originalColor = img.color;

        if(deselectSiblings)
        {
            int currentSize = deselectOnClick.Length;
            Array.Resize(ref deselectOnClick, currentSize + transform.parent.childCount);
            for(int i=0; i < transform.parent.childCount; i++)
            {
                if(i == transform.GetSiblingIndex()) continue;

                var button = transform.parent.GetChild(i).GetComponent<MenuButton>();
                if(button == null) continue;

                deselectOnClick[currentSize++] = button;
            }
            Array.Resize(ref deselectOnClick, currentSize);
        }

        if(defaultSelected) Selected = true;
    }

    public void OnEnter()
    {
        if(!Input.GetMouseButton(0)) AudioManager.PlayClip(clipHover, 0.5f, Mixer.MENU);
        if(textComp != null) textComp.fontSize = originalTextSize + diff;
    }

    public void OnExit()
    {
        if(textComp != null) textComp.fontSize = originalTextSize;
    }

    public void OnClick()
    {
        AudioManager.PlayClip(clipClick, 1.0f, Mixer.MENU);
        if(toggle)
        {
            if(!(atLeastOne && Selected)) Selected = !Selected;

            if(Selected)
            {
                foreach(var b in deselectOnClick)
                {
                    b.Selected = false;
                }
            }
        }
    }
}

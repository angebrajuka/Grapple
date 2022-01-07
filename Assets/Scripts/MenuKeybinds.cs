using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuKeybinds : MonoBehaviour
{
    public GameObject keybindButton;
    public RectTransform overlay;
    public RectTransform content;

    [HideInInspector] public bool waitingForKey = false;
    [HideInInspector] public MenuButton focusedButton;

    void SetButtonText(MenuButton button)
    {
        Text keybindText = button.transform.GetChild(0).GetComponent<Text>();
        keybindText.text = "";
        for(int bind = 0; bind<PlayerInput.keybinds[button.control].Length; bind++)
        {
            keybindText.text += ((KeyCode)PlayerInput.keybinds[button.control][bind]).ToString() + (bind <PlayerInput.keybinds[button.control].Length-1 ? ", " : "");
        }
    }

    void OverlayFalse()
    {
        overlay.gameObject.SetActive(false);
        waitingForKey = false;
        transform.GetComponent<Menu>().escBack = true;
    }

    void Start()
    {
        int i = 0;
        foreach(var pair in PlayerInput.keybinds)
        {
            GameObject go = Instantiate(keybindButton, content.transform);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition -= new Vector2(0, 10 + (60*i));
            MenuButton button = go.transform.GetChild(1).GetComponent<MenuButton>();
            button.control = pair.Key;
            button.menuKeybinds = this;
            rect.transform.GetChild(0).GetComponent<Text>().text = pair.Key;

            SetButtonText(button);

            i++;
        }

        var size = content.offsetMin;
        size.y = -(PlayerInput.keybinds.Count * 60 + 30);
        content.offsetMin = size;
    }

    void LateUpdate()
    {
        if(!waitingForKey || !Input.anyKey) return;

        if(Input.GetKey(KeyCode.Escape))
        {
            OverlayFalse();
            return;
        }

        if(Input.GetKey(KeyCode.Backspace))
        {
            PlayerInput.keybinds[focusedButton.control] = new int[0];
            PlayerInput.SaveKeybinds();
            SetButtonText(focusedButton);
            OverlayFalse();
            return;
        }

        for(int i=0; i<(int)KeyCode.Joystick8Button19; i++)
        {
            if(Input.GetKey((KeyCode)i)) 
            {
                if(Array.Exists<int>(PlayerInput.keybinds[focusedButton.control], e => e == i)) continue;

                var len = PlayerInput.keybinds[focusedButton.control].Length;
                var prev = PlayerInput.keybinds[focusedButton.control];
                PlayerInput.keybinds[focusedButton.control] = new int[len+1];
                for(int b=0; b<len; b++)
                {
                    PlayerInput.keybinds[focusedButton.control][b] = prev[b];
                }
                PlayerInput.keybinds[focusedButton.control][len] = i;
                SetButtonText(focusedButton);
                PlayerInput.SaveKeybinds();
                OverlayFalse();

                return;
            }
        }
    }
}

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuHandler : MonoBehaviour
{
    static MenuHandler instance;
    private static int currentMenu;
    private static Stack<int> prevMenu;
    public static bool anyMenu;

    public void Init()
    {
        instance = this;

        prevMenu = new Stack<int>();

        for(int i=0; i<instance.transform.childCount; i++)
        {
            instance.transform.GetChild(i).gameObject.SetActive(false);
        }
        Close();
        anyMenu = false;
    }

    public static void Close()
    {
        instance.transform.GetChild(currentMenu).gameObject.SetActive(false);
    }

    public static void Back()
    {
        Close();
        anyMenu = false;
        if(prevMenu.Count != 0)
        {
            currentMenu = prevMenu.Pop();
            instance.transform.GetChild(currentMenu).gameObject.SetActive(true);
            anyMenu = true;
        }
    }

    public static int CurrentMenu
    {
        get
        {
            return currentMenu;
        }
        set
        {
            if(anyMenu)
            {
                prevMenu.Push(currentMenu);
                Close();
            }
            currentMenu = value;
            instance.transform.GetChild(currentMenu).gameObject.SetActive(true);
            anyMenu = true;
        }
    }
}
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevConsole : MonoBehaviour
{
    public static DevConsole instance;

    // hierarchy
    public GameObject backing;
    public TMP_InputField inputField;

    public static bool isActive=false;
    static Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();

    public void Init()
    {
        instance = this;

        foreach(var methodInfo in typeof(Commands).GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
            commands.Add(methodInfo.Name, methodInfo);
        }

        Disable();
    }

    void Enable()
    {
        isActive = true;
        PauseHandler.frozenInput = true;
        backing.SetActive(true);
        inputField.ActivateInputField();
        inputField.text = "";
    }

    public void Disable()
    {
        isActive = false;
        PauseHandler.frozenInput = false;
        backing.SetActive(false);
    }

    public void OnCommandEntered()
    {
        string text = inputField.text.ToLower();
        string[] words = text.Split(' ');
        try
        {
            commands[words[0]].Invoke(null, new object[]{words});
        }
        catch {}

        Disable();
    }

    void Update() {
        if(!PauseHandler.paused && Input.GetKeyDown(KeyCode.BackQuote)) {
            if(!isActive)   Enable();
            else            Disable();
        }
    }
}

public static class Commands
{
    public static void time(string[] args)
    {
        // float amount = float.Parse(args[2]);

        // switch(args[1])
        // {
        // case "set":
        //     DaylightCycle.time = amount;
        //     break;
        // case "add":
        //     DaylightCycle.time += amount;
        //     break;
        // case "subtract":
        //     DaylightCycle.time -= amount;
        //     break;
        // default:
        //     return;
        // }
    }

    public static void health(string[] args)
    {
        float amount = float.Parse(args[2]);

        switch(args[1])
        {
        case "add":
            PlayerTarget.instance.Heal(amount);
            break;
        case "sub":
            PlayerTarget.instance.Damage(amount, -PlayerMovement.rb.transform.forward, 100);
            break;
        default:
            return;
        }
    }


    public static void tp(string[] args)
    {
        if(float.TryParse(args[1], out float x) && float.TryParse(args[2], out float y) && float.TryParse(args[3], out float z))
        {
            Vector3 pos = PlayerMovement.rb.position;
            pos.Set(x, y, z);
            PlayerMovement.rb.position = pos;
        }
    }

    public static void kfa(string[] args)
    {
        for(int i=0; i<PlayerInventory.hasGun.Length; i++)
        {
            PlayerInventory.hasGun[i] = true;
        }
        fa(null);
    }

    public static void fa(string[] args)
    {
        foreach(var pair in PlayerInventory.maxAmmo)
        {
            PlayerInventory.reserveAmmo[pair.Key] = pair.Value;
        }
        PlayerHUD.UpdateAmmoReserve();
    }
}
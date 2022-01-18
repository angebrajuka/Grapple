using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour
{
    public InputField inputField;
    public Text textObject;

    public static bool isActive=false;
    static Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();

    public void Init()
    {
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
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
        textObject.text = "";
    }

    void Disable()
    {
        isActive = false;
        PauseHandler.frozenInput = false;
        inputField.gameObject.SetActive(false);
    }

    public void OnCommandEntered()
    {
        string text = textObject.text.ToLower();
        string[] words = text.Split(' ');
        try
        {
            commands[words[0]].Invoke(null, new object[]{words});
        }
        catch {}

        Disable();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
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
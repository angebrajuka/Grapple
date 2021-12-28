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
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
        textObject.text = "";
    }

    void Disable()
    {
        isActive = false;
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
        // float amount = float.Parse(args[2]);

        // switch(args[1])
        // {
        // case "add":
        //     PlayerTarget.target.Heal(amount);
        //     break;
        // case "sub":
        //     PlayerTarget.target.Damage(amount);
        //     break;
        // default:
        //     return;
        // }
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
        // foreach(var pair in Items.guns)
        // {
        //     if(args.Length != 2)
        //     {
        //         AutoAddItem(new string[]{"give", pair.Key, "1"});
        //     }
        // }
    }

    public static void fa(string[] args)
    {
        // for(int i=0; i<(args.Length==2 ? Int32.Parse(args[1]) : 1); i++)
        // {
        //     foreach(string type in Items.GetAmmoTypes())
        //     {
        //         AutoAddItem(new string[]{"give", type+"", Items.items[type].maxStack+""});
        //     }
        // }
    }
}
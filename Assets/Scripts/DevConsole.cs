using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour
{
    public InputField inputField;
    public Text textObject;

    public static bool isActive=false;

    public void Init()
    {
        Disable();
    }


    // static bool Time(string[] args)
    // {
    //     float amount = float.Parse(args[2]);

    //     switch(args[1])
    //     {
    //     case "set":
    //         DaylightCycle.time = amount;
    //         break;
    //     case "add":
    //         DaylightCycle.time += amount;
    //         break;
    //     case "subtract":
    //         DaylightCycle.time -= amount;
    //         break;
    //     default:
    //         return false;
    //     }
    
    //     return true;
    // }

    // static bool Health(string[] args)
    // {
    //     float amount = float.Parse(args[2]);

    //     switch(args[1])
    //     {
    //     case "add":
    //         PlayerTarget.target.Heal(amount);
    //         break;
    //     case "sub":
    //         PlayerTarget.target.Damage(amount);
    //         break;
    //     default:
    //         return false;
    //     }
    
    //     return true;
    // }


    // static bool Teleport(string[] args)
    // {
    //     if(Int32.TryParse(args[1], out int x) && Int32.TryParse(args[2], out int y))
    //     {
    //         PlayerMovement.rb.position = new Vector3(x, y, 0);

    //         return true;
    //     }

    //     return false;
    // }

    // static bool KFA(string[] args)
    // {
    //     foreach(var pair in Items.guns)
    //     {
    //         if(args.Length != 2)
    //         {
    //             AutoAddItem(new string[]{"give", pair.Key, "1"});
    //         }
    //     }

    //     return true;
    // }

    // static bool FA(string[] args)
    // {
    //     for(int i=0; i<(args.Length==2 ? Int32.Parse(args[1]) : 1); i++)
    //     {
    //         foreach(string type in Items.GetAmmoTypes())
    //         {
    //             AutoAddItem(new string[]{"give", type+"", Items.items[type].maxStack+""});
    //         }
    //     }

    //     return true;
    // }



    static readonly Dictionary<string, Func<string[], bool>> commands = new Dictionary<string, Func<string[], bool>>
    {
        // {"time",        Time        },
        // {"health",      Health      },
        // {"tp",          Teleport    },
        // {"kfa",         KFA         },
        // {"fa",          FA          }
    };

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

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            if(!isActive)   Enable();
            else            Disable();
        }
    }

    public void OnCommandEntered()
    {
        string text = textObject.text.ToLower();
        string[] words = text.Split(' ');
        try
        {
            commands[words[0]](words);
        }
        catch {}
        
        Disable();
    }
}

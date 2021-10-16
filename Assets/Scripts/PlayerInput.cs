using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class JsonKeybindPair
{
    public string name;
    public int[] keys;

    public JsonKeybindPair(KeyValuePair<string, int[]> bind)
    {
        this.name = bind.Key;
        this.keys = bind.Value;
    }
}

[System.Serializable]
public class KeybindsJson
{
    public JsonKeybindPair[] keybinds;
}

public class PlayerInput : MonoBehaviour
{
    static string CONTROLS_PATH
    {
        get { return SaveData.DIRECTORY_PATH+"/controls.json"; }
    }
    public static float MAX_LOOK_SPEED = 6f;

    public static PlayerInput instance;

    // settings
    public static Dictionary<string, int[]> keybinds;
    public static Vector2 speed_look;
    public static float speed_scroll;

    public void Init()
    {
        instance = this;

        keybinds = new Dictionary<string, int[]>();

        speed_look = new Vector2(0, 0);
        LoadSettings();
    }

    public static void LoadKeybindsString(string bindsTxt)
    {
        var binds = JsonUtility.FromJson<KeybindsJson>(bindsTxt).keybinds;
        foreach(var bind in binds)
        {
            if(!keybinds.ContainsKey(bind.name)) keybinds.Add(bind.name, bind.keys);
            else keybinds[bind.name] = bind.keys;
        }
    }

    public static void LoadSettings(bool forceDefault=false)
    {
        keybinds.Clear();

        LoadKeybindsString(Resources.Load<TextAsset>("DefaultControls").text); // default
        if(!forceDefault && System.IO.File.Exists(CONTROLS_PATH))
        {
            LoadKeybindsString(System.IO.File.ReadAllText(CONTROLS_PATH)); // load keybinds if exists
        }

        speed_look.Set(1.1f, 0.8f);
        if(PlayerPrefs.HasKey("speed_look_x"))
        {
            speed_look.Set(PlayerPrefs.GetFloat("speed_look_x"), PlayerPrefs.GetFloat("speed_look_y"));
        }

        speed_scroll = 1;
        if(PlayerPrefs.HasKey("speed_scroll"))
        {
            speed_scroll = PlayerPrefs.GetFloat("speed_scroll");
        }
    }

    public static void SaveKeybinds()
    {
        var binds = new KeybindsJson();
        binds.keybinds = new JsonKeybindPair[keybinds.Count];
        int i = 0;
        foreach(var bind in keybinds)
        {
            binds.keybinds[i++] = new JsonKeybindPair(bind);
        }
        string bindsTxt = JsonUtility.ToJson(binds);

        System.IO.File.WriteAllText(CONTROLS_PATH, bindsTxt);
    }

    public static void SaveLookSpeed()
    {
        PlayerPrefs.SetFloat("speed_look_x", speed_look.x);
        PlayerPrefs.SetFloat("speed_look_y", speed_look.y);
        PlayerPrefs.Save();
    }

    public static bool GetKey(string key)
    {
        bool down = false;
        foreach(var bind in keybinds[key])
        {
            if(Input.GetKey((KeyCode)bind)) down = true;
        }
        return down;
    }

    public static bool GetKeyDown(string key)
    {
        bool down = false;
        foreach(var bind in keybinds[key])
        {
            if(Input.GetKeyDown((KeyCode)bind)) down = true;
        }
        return down;
    }

    public static bool GetKeyUp(string key)
    {
        bool up = false;
        foreach(var bind in keybinds[key])
        {
            if(Input.GetKeyUp((KeyCode)bind)) up = true;
        }
        return up;
    }
}
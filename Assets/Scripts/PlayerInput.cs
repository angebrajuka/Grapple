using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class JsonKeybindPair
{
    public string name;
    public int key;

    public JsonKeybindPair(KeyValuePair<string, KeyCode> bind)
    {
        this.name = bind.Key;
        this.key = (int)bind.Value;
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
    public static Dictionary<string, KeyCode> keybinds;
    public static Vector2 speed_look;

    // input
    public static Vector3 input_move;
    public static Vector2 input_look;

    public void Init()
    {
        instance = this;

        keybinds = new Dictionary<string, KeyCode>();

        input_move = new Vector3(0, 0);
        input_look = new Vector2(0, 0);
        speed_look = new Vector2(0, 0);

        LoadSettings();
    }

    public static void LoadKeybindsString(string bindsTxt)
    {
        var binds = JsonUtility.FromJson<KeybindsJson>(bindsTxt).keybinds;
        foreach(var bind in binds)
        {
            if(!keybinds.ContainsKey(bind.name)) keybinds.Add(bind.name, (KeyCode)bind.key);
            else keybinds[bind.name] = (KeyCode)bind.key;
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
        return Input.GetKey(keybinds[key]);
    }

    public static bool GetKeyDown(string key)
    {
        return Input.GetKeyDown(keybinds[key]);
    }

    void Update()
    {
        input_move.Set(0, 0, 0);
        input_look.Set(0, 0);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseHandler.ShowCursor();
            MenuHandler.CurrentMenu = 0;
            enabled = false;
            return;
        }

        // movement
        {
            if(GetKey("walk_front"))    input_move.z ++;
            if(GetKey("walk_back"))     input_move.z --;
            if(GetKey("walk_left"))     input_move.x --;
            if(GetKey("walk_right"))    input_move.x ++;
            input_move.Normalize();
        }

        // mouse look
        {
            input_look.x = Input.GetAxis("Mouse X") * speed_look.x;
            input_look.y = Input.GetAxis("Mouse Y") * speed_look.y;
        }
    }
}
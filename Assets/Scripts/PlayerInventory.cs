using System.Collections.Generic;
using UnityEngine;

public class Gun
{
    public string name;
    public int ammo;
    public float time_lastShot;

    public Gun(string name="", int ammo=0, float time_lastShot=-1000)
    {
        this.name = name;
        this.ammo = ammo;
        this.time_lastShot = time_lastShot;
    }
}

public class PlayerInventory : MonoBehaviour
{
    public static Gun[] guns;
    private static int _currentGun;
    public static GunStats CurrentGunStats { get { return Guns.guns[guns[_currentGun].name]; } }
    public static Gun CurrentGun { get { return guns[_currentGun]; } }
    public static Dictionary<string, int> reserveAmmo;
    public static int Ammo
    {
        get { return guns[_currentGun].ammo; }
        set { guns[_currentGun].ammo = value; }
    }
    public static int ReserveAmmo { get { return reserveAmmo[guns[_currentGun].name]; } }
    public static float Time_LastShot { get { return guns[_currentGun].time_lastShot; } }

    public static void Init()
    {
        guns = new Gun[3]
        {
            new Gun("DoubleBarrelShotgun"),
            new Gun("PumpActionShotgun"),
            new Gun()
        };
        reserveAmmo = new Dictionary<string, int>();
        foreach(var pair in Guns.guns)
        {
            if(pair.Value != null && !reserveAmmo.ContainsKey(pair.Value.ammoType)) reserveAmmo.Add(pair.Value.ammoType, 0);
        }
    }

    public static int _CurrentGun
    {
        get { return _currentGun; }
        set
        {
            _currentGun = value;
            PlayerAnimator.instance.UpdateGun();
        }
    }

    public static string CurrentGunName { get { return CurrentGun.name; } }

    void Update()
    {
        if(PlayerInput.GetKey("gun_switch_0")) _CurrentGun = 0;
        if(PlayerInput.GetKey("gun_switch_1")) _CurrentGun = 1;
        if(PlayerInput.GetKey("gun_switch_2")) _CurrentGun = 2;
    }
}
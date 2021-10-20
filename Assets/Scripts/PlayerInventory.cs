using System.Collections.Generic;
using UnityEngine;

public struct GunData
{
    public int ammo;
    public float time_lastShot;

    public GunData(int i)
    {
        ammo = 0;
        time_lastShot = -1000;
    }
}

public class PlayerInventory : MonoBehaviour
{
    public static GunData[] gunDatas;
    public static string[] guns;
    private static int _currentGun;
    public static Gun CurrentGun { get { return Guns.guns[guns[_currentGun]]; } }
    public static Dictionary<string, int> reserveAmmo;
    public static int Ammo
    {
        get { return gunDatas[_currentGun].ammo; }
        set { gunDatas[_currentGun].ammo = value; }
    }
    public static int ReserveAmmo { get { return reserveAmmo[guns[_currentGun]]; } }
    public static float Time_LastShot { get { return gunDatas[_currentGun].time_lastShot; } }

    public static void Init()
    {
        guns = new string[3]{"PumpActionShotgun", "DoubleBarrelShotgun", ""};
        gunDatas = new GunData[guns.Length];
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

    public static string CurrentGunName { get { return CurrentGun == null ? "" : CurrentGun.name; } }

    void Update()
    {
        if(PlayerInput.GetKey("gun_switch_0")) _CurrentGun = 0;
        if(PlayerInput.GetKey("gun_switch_1")) _CurrentGun = 1;
        if(PlayerInput.GetKey("gun_switch_2")) _CurrentGun = 2;
    }
}
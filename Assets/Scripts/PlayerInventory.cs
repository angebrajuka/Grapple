using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static bool[] hasGun;
    public static int _currentGun;
    public static int _nextGun;
    public static Gun CurrentGun { get { return Guns.guns[_currentGun]; } }
    public static Dictionary<string, int> reserveAmmo;
    public static int Ammo
    {
        get { return CurrentGun.ammo; }
        set
        {
            CurrentGun.ammo = value;
        }
    }
    public static int ReserveAmmo
    {
        get { return reserveAmmo[CurrentGun.ammoType]; }
        set
        {
            reserveAmmo[CurrentGun.ammoType] = value;
            PlayerHUD.UpdateAmmoReserve();
        }
    }

    public static void Init()
    {
        hasGun = new bool[]{true, true, true};
        reserveAmmo = new Dictionary<string, int>();
        foreach(var gun in Guns.guns)
        {
            if(!reserveAmmo.ContainsKey(gun.ammoType)) reserveAmmo.Add(gun.ammoType, 0);
        }

        reserveAmmo["TwelveGauge"] = 16;
        reserveAmmo["Grenade"] = 16;

        _currentGun = 0;
        _nextGun = 0;
    }

    public static string CurrentGunName { get { return CurrentGun.name; } }

    void Update()
    {
        if(PlayerInput.GetKey("gun_switch_0")) _nextGun = 0;
        if(PlayerInput.GetKey("gun_switch_1")) _nextGun = 1;
        if(PlayerInput.GetKey("gun_switch_2")) _nextGun = 2;

        if(PlayerInput.GetKey("reload")) PlayerAnimator.instance.CheckReload(true);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static bool[] hasGun;
    public static int _currentGun;
    public static Gun CurrentGun { get { return Guns.guns[_currentGun]; } }
    public static Dictionary<string, int> reserveAmmo;
    public static int Ammo
    {
        get { return Guns.guns[_currentGun].ammo; }
        set { Guns.guns[_currentGun].ammo = value; }
    }
    public static int ReserveAmmo { get { return reserveAmmo[Guns.guns[_currentGun].ammoType]; } }
    public static float Time_LastShot { get { return Guns.guns[_currentGun].timeLastShot; } }

    public static void Init()
    {
        hasGun = new bool[]{true, true, false};
        reserveAmmo = new Dictionary<string, int>();
        foreach(var gun in Guns.guns)
        {
            if(!reserveAmmo.ContainsKey(gun.ammoType)) reserveAmmo.Add(gun.ammoType, 0);
        }
    }

    public static string CurrentGunName { get { return CurrentGun.name; } }

    void Update()
    {
        int switchTo = _currentGun;

        if(PlayerInput.GetKey("gun_switch_0")) switchTo = 0;
        if(PlayerInput.GetKey("gun_switch_1")) switchTo = 1;
        if(PlayerInput.GetKey("gun_switch_2")) switchTo = 2;

        if(hasGun[switchTo]) _currentGun = switchTo;

        if(PlayerInput.GetKey("reload")) PlayerAnimator.instance.CheckReload(true);
    }
}
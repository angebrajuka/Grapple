using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static string[] guns;
    private static int _currentGun;
    public static Gun CurrentGun { get { return Guns.guns[guns[_currentGun]]; } }

    public static void Init()
    {
        guns = new string[3]{"PumpActionShotgun", "DoubleBarrelShotgun", ""};
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
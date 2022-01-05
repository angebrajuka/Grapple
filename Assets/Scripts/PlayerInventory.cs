using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class StringIntPair
{
    public string key;
    public int value;
}

[System.Serializable]
class AmmoDataJson
{
    public StringIntPair[] maxAmmo;
    public StringIntPair[] startAmmo;
}

public class PlayerInventory : MonoBehaviour
{
    public static bool[] hasGun;
    public static int _currentGun;
    public static int _nextGun;
    public static Gun CurrentGun { get { return Guns.guns[_currentGun]; } }
    public static Dictionary<string, int> maxAmmo;
    public static Dictionary<string, int> startAmmo;
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
        hasGun = new bool[3];
        maxAmmo = new Dictionary<string, int>();
        startAmmo = new Dictionary<string, int>();
        reserveAmmo = new Dictionary<string, int>();
        var ammoData = JsonUtility.FromJson<AmmoDataJson>(Resources.Load<TextAsset>("AmmoData").text);
        foreach(var pair in ammoData.maxAmmo)
        {
            maxAmmo.Add(pair.key, pair.value);
            reserveAmmo.Add(pair.key, 0);
        }

        foreach(var pair in ammoData.startAmmo)
        {
            startAmmo.Add(pair.key, pair.value);
        }
    }

    public static void Reset()
    {
        foreach(var pair in maxAmmo)
        {
            reserveAmmo[pair.Key] = 0;
        }
        foreach(var pair in startAmmo)
        {
            reserveAmmo[pair.Key] = pair.Value;
        }

        hasGun[0] = true;
        hasGun[1] = true;
        hasGun[2] = true;

        _currentGun = 0;
        _nextGun = 0;
    }

    public static string CurrentGunName { get { return CurrentGun.name; } }

    public static bool CanShoot
    {
        get {
            return PlayerAnimator.state != PlayerAnimator.State.SWAPPING && 
            (PlayerAnimator.IsIdle || CurrentGun.shotgunReload) && 
            CurrentGunName == PlayerAnimator.activeGun && 
            (CurrentGun.primed || !CurrentGun.animateBetweenShots) &&
            Ammo >= CurrentGun.ammoPerShot && 
            Time.time > CurrentGun.timeLastShot+CurrentGun.timeBetweenShots;
        }
    }

    public static void FinishReload()
    {
        if(CurrentGun.shotgunReload)
        {
            ReserveAmmo --;
            Ammo ++;
            CurrentGun.primed = true;
        }
        else
        {
            int _ammo = ReserveAmmo/CurrentGun.ammoPerShot;
            int _clip = CurrentGun.ammo/CurrentGun.ammoPerShot;
            int _clipSize = CurrentGun.magSize/CurrentGun.ammoPerShot;
            
            if(_ammo > _clipSize - _clip)
            {
                ReserveAmmo -= (_clipSize - _clip)*CurrentGun.ammoPerShot;
                Ammo = CurrentGun.magSize;
            }
            else if(_ammo > 0)
            {
                int num = _ammo*CurrentGun.ammoPerShot;
                Ammo += num;
                ReserveAmmo -= num;
            }
        }
    }

    void Update()
    {
        if(PlayerInput.GetKey("gun_switch_0")) _nextGun = 0;
        if(PlayerInput.GetKey("gun_switch_1")) _nextGun = 1;
        if(PlayerInput.GetKey("gun_switch_2")) _nextGun = 2;

        if(PlayerInput.GetKey("reload")) PlayerAnimator.instance.CheckReload(true);

        if(CanShoot && PlayerInput.GetKey("shoot"))
        {
            CurrentGun.Shoot();
        }
    }
}
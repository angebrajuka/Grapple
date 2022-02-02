using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoData {
    public Ammo ammo;
    public int start, max;
    public Texture2D tex;
    public float spacing;
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    // hierarchy
    public AmmoData[] ammoDatas;

    public static bool[] hasGun;
    public static int _currentGun;
    public static int _nextGun;
    public static Gun CurrentGun { get { return Guns.guns[_currentGun]; } }
    public static Dictionary<Ammo, int> maxAmmo;
    public static Dictionary<Ammo, int> startAmmo;
    public static Dictionary<Ammo, int> reserveAmmo;
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

    public void Init()
    {
        instance = this;

        hasGun = new bool[Guns.guns.Length];
        maxAmmo = new Dictionary<Ammo, int>();
        startAmmo = new Dictionary<Ammo, int>();
        reserveAmmo = new Dictionary<Ammo, int>();
        foreach(var datum in ammoDatas)
        {
            maxAmmo.Add(datum.ammo, datum.max);
            startAmmo.Add(datum.ammo, datum.start);
            reserveAmmo.Add(datum.ammo, 0);
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
        for(int i=1; i<hasGun.Length; i++)
        {
            hasGun[i] = false;
        }

        _currentGun = 0;
        _nextGun = 0;
    }

    public static string CurrentGunName { get { return CurrentGun.name; } }

    public static bool CanShoot { get {
        return CurrentGun.chamber == Gun.Chamber.FULL &&
        Ammo >= CurrentGun.ammoPerShot && 
        Time.time > CurrentGun.timeLastShot+CurrentGun.timeBetweenShots; } }

    public static bool CanReload { get {
        return ReserveAmmo >= CurrentGun.ammoPerShot &&
        Ammo < CurrentGun.magSize; } }

    public static void FinishReload()
    {
        if(CurrentGun.shotgunReload)
        {
            ReserveAmmo --;
            Ammo ++;
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
        if(CurrentGun.chamber != Gun.Chamber.FULL) CurrentGun.chamber = CurrentGun.chamberPostReload;
    }

    void Update()
    {
        for(int i=0; i<hasGun.Length; i++)
        {
            if(hasGun[i] && PlayerInput.GetKey("gun_switch_"+i))
            {
                _nextGun = i;
                break;
            }
        }

        if(PlayerInput.GetKey("reload")) PlayerAnimator.instance.CheckReload(true);

        if(CanShoot && PlayerAnimator.CanShoot && PlayerInput.GetKey("shoot"))
        {
            CurrentGun.Shoot();
        }
    }
}
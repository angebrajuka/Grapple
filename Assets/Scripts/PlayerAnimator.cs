using System.Collections.Generic;
using UnityEngine;

using static PlayerAnimator.State;

public class PlayerAnimator : MonoBehaviour
{
    public static PlayerAnimator instance;

    // hierarchy
    public Transform gunPos;
    public Animator gunPosAnimator;
    public Animator gunReloadAnimator;
    public GunAnimationEvents gunAnimationEvents;
    public float recoilSpeed_moveBack, recoilSpeed_rotateBack;
    public float recoilSpeed_moveForward, recoilSpeed_rotateForward;
    public Vector3 backPosition, backRotation;

    public static Dictionary<string, Transform> guns;
    public static string activeGun="";
    public static Transform ActiveGun { get { return guns[activeGun]; } }

    public enum State
    {
        RAISED,
        SWAPPING,
        RECOIL_BACK,
        RECOIL_FORWARD
    }
    public static State state;

    public static Animator GunPosAnimator
    {
        get
        {
            instance.gunPosAnimator.enabled = true;
            return instance.gunPosAnimator;
        }
    }

    public static bool IsReloading { get { return instance.gunReloadAnimator.GetInteger("state") == 1; } }
    public static bool IsIdle { get { return instance.gunReloadAnimator.GetInteger("state") == 0; } }

    public void Init()
    {
        instance = this;

        state = RAISED;
        guns = new Dictionary<string, Transform>();

        foreach(var gun in Guns.guns)
        {
            guns.Add(gun.name, gun.transform);
            if(guns[gun.name] != null) guns[gun.name].gameObject.SetActive(false);
        }

        AtLowest();
    }

    bool CanReload { get { return IsIdle && (PlayerInventory.CurrentGun.primed || !PlayerInventory.CurrentGun.animateBetweenShots) && PlayerInventory.ReserveAmmo >= PlayerInventory.CurrentGun.ammoPerShot && PlayerInventory.Ammo < PlayerInventory.CurrentGun.magSize; } }
    bool ShotgunAutoReload { get { return Time.time > PlayerInventory.CurrentGun.timeLastShot+PlayerInventory.CurrentGun.timeBetweenShots*2 && PlayerInventory.CurrentGun.shotgunReload; } }

    public void CheckReload(bool force=false)
    {
        if(CanReload && (PlayerInventory.Ammo <= 0 || ShotgunAutoReload || force))
        {
            gunReloadAnimator.SetInteger("state", 1); // reloading
        }
    }

    public void AtLowest()
    {
        if(activeGun != "")
        {
            guns[activeGun].gameObject.SetActive(false);
        }
        activeGun = PlayerInventory.CurrentGunName;
        if(activeGun != "")
        {
            guns[activeGun].gameObject.SetActive(true);
            gunReloadAnimator.SetInteger("gun", PlayerInventory.CurrentGun.index);
            CheckReload();
            PlayerHUD.UpdateAmmoReserve();
            // PlayerHUD.UpdateAmmoImage();
        }
        GunPosAnimator.Play("Base Layer.Raising"); // raise
    }

    public void Recoil()
    {
        state = RECOIL_BACK;
    }

    void Update()
    {
        var pos = gunPos.localPosition;
        var rot = gunPos.localRotation;

        switch(state)
        {
        case RECOIL_BACK:
            var backRotation = Quaternion.Euler(this.backRotation*PlayerInventory.CurrentGun.recoil);
            Vector3 backPosition = PlayerInventory.CurrentGun.recoil*this.backPosition;
            pos = Vector3.MoveTowards(pos, backPosition, Time.deltaTime*recoilSpeed_moveBack);
            rot = Quaternion.RotateTowards(rot, backRotation, Time.deltaTime*recoilSpeed_rotateBack);
            if(pos == backPosition) state = RECOIL_FORWARD;

            break;
        case RECOIL_FORWARD:
            pos = Vector3.MoveTowards(pos, Vector3.zero, Time.deltaTime*recoilSpeed_moveForward);
            rot = Quaternion.RotateTowards(rot, Quaternion.identity, Time.deltaTime*recoilSpeed_rotateForward);
            if(pos == Vector3.zero)
            {
                gunAnimationEvents.Raised();
            }

            break;
        case RAISED:
            if(PlayerInventory.hasGun[PlayerInventory._nextGun] && PlayerInventory._nextGun != PlayerInventory._currentGun && !IsReloading)
            {
                PlayerInventory._currentGun = PlayerInventory._nextGun;
                state = SWAPPING;
                gunReloadAnimator.SetInteger("state", 0);
                GunPosAnimator.Play("Base Layer.Lowering");
            }
            else if(IsIdle)
            {
                if(!PlayerInventory.CurrentGun.primed && !IsReloading && PlayerInventory.CurrentGun.animateBetweenShots)
                {
                    gunReloadAnimator.SetInteger("state", 2);
                }
                CheckReload();
            }
            break;
        default:
            break;
        }

        gunPos.localPosition = pos;
        gunPos.localRotation = rot;
    }
}
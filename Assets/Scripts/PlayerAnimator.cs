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

    public void Init()
    {
        instance = this;

        state = RAISED;
        guns = new Dictionary<string, Transform>();

        foreach(var pair in Guns.guns)
        {
            guns.Add(pair.Key, pair.Value == null ? null : pair.Value.transform);
            if(guns[pair.Key] != null) guns[pair.Key].gameObject.SetActive(false);
        }
    }

    public void CheckReload()
    {
        if(PlayerInventory.Ammo <= 0) // TODO
        {
            gunReloadAnimator.SetInteger("gun", PlayerInventory.CurrentGunStats.index);
            gunReloadAnimator.SetBool("reloading", true);
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
            CheckReload();
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
            var backRotation = Quaternion.Euler(this.backRotation*PlayerInventory.CurrentGunStats.recoil);
            Vector3 backPosition = PlayerInventory.CurrentGunStats.recoil*this.backPosition;
            pos = Vector3.MoveTowards(pos, backPosition, Time.deltaTime*recoilSpeed_moveBack);
            rot = Quaternion.RotateTowards(rot, backRotation, Time.deltaTime*recoilSpeed_rotateBack);
            if(pos == backPosition) state = RECOIL_FORWARD;

            break;
        case RECOIL_FORWARD:
            pos = Vector3.MoveTowards(pos, Vector3.zero, Time.deltaTime*recoilSpeed_moveForward);
            rot = Quaternion.RotateTowards(rot, Quaternion.identity, Time.deltaTime*recoilSpeed_rotateForward);
            if(pos == Vector3.zero) gunAnimationEvents.Raised();

            break;
        case RAISED:
            if(activeGun != PlayerInventory.CurrentGunName)
            {
                state = SWAPPING;
                gunReloadAnimator.SetInteger("gun", -1);
                gunReloadAnimator.SetBool("reloading", false);
                GunPosAnimator.Play("Base Layer.Lowering"); // lower
            }
            else
            {
                gunAnimationEvents.Raised();
            }
            break;
        default:
            break;
        }

        gunPos.localPosition = pos;
        gunPos.localRotation = rot;
    }
}
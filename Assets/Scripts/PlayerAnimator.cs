using System.Collections.Generic;
using UnityEngine;
using System;

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

    public class State {
        private Action set, update;

        static void Default() {}

        public State(Action set=null, Action update=null) {
            this.set = (set == null ? Default : set);
            this.update = (update == null ? Default : update);
        }

        public void Set() {
            set();
        }

        public void Update() {
            update();
        }
    }

    public static readonly State RAISED = new State(() => {
        PlayerAnimator.GunPosAnimator.enabled = false;
        instance.gunReloadAnimator.SetInteger("state", 0);
    }, () => {
        if(PlayerInventory.CurrentGun.chamber == Gun.Chamber.SHELL) {
            SetState(EJECTING);
            return;
        }
        if(PlayerInventory.CurrentGun.chamber == Gun.Chamber.EMPTY && PlayerInventory.Ammo >= PlayerInventory.CurrentGun.ammoPerShot) {
            SetState(PRIMING);
            return;
        }
        instance.CheckReload();
    });
    public static readonly State SWAPPING = new State(() => {
        PlayerInventory._currentGun = PlayerInventory._nextGun;
        instance.gunReloadAnimator.SetInteger("state", 0);
        GunPosAnimator.Play("Base Layer.Lowering");
    });
    public static readonly State EJECTING = new State(() => {
        instance.gunReloadAnimator.SetInteger("state", 2);
    });
    public static readonly State PRIMING = new State(() => {
        instance.gunReloadAnimator.SetInteger("state", 3);
    });
    public static readonly State RELOADING = new State(() => {
        instance.gunReloadAnimator.SetInteger("state", 1);
    });
    public static readonly State RECOIL_BACK = new State(() => {
        instance.gunReloadAnimator.SetInteger("state", 0);
    }, () => {
        var pos = instance.gunPos.localPosition;
        var rot = instance.gunPos.localRotation;
        var backRotation = Quaternion.Euler(instance.backRotation*PlayerInventory.CurrentGun.recoil);
        Vector3 backPosition = PlayerInventory.CurrentGun.recoil*instance.backPosition;
        pos = Vector3.MoveTowards(pos, backPosition, Time.deltaTime*instance.recoilSpeed_moveBack);
        rot = Quaternion.RotateTowards(rot, backRotation, Time.deltaTime*instance.recoilSpeed_rotateBack);
        if(pos == backPosition) SetState(RECOIL_FORWARD);
        instance.gunPos.localPosition = pos;
        instance.gunPos.localRotation = rot;
    });
    public static readonly State RECOIL_FORWARD = new State(() => {
        instance.gunReloadAnimator.SetInteger("state", 0);
    }, () => {
        var pos = instance.gunPos.localPosition;
        var rot = instance.gunPos.localRotation;
        pos = Vector3.MoveTowards(pos, Vector3.zero, Time.deltaTime*instance.recoilSpeed_moveForward);
        rot = Quaternion.RotateTowards(rot, Quaternion.identity, Time.deltaTime*instance.recoilSpeed_rotateForward);
        if(pos == Vector3.zero)
        {
            SetState(RAISED);
        }
        instance.gunPos.localPosition = pos;
        instance.gunPos.localRotation = rot;
    });
    
    private static State p_state;
    public static State state { get { return p_state; } }

    public static void SetState(State newState) {
        p_state = newState;
        p_state.Set();
    }

    public static Animator GunPosAnimator { get {
        instance.gunPosAnimator.enabled = true;
        return instance.gunPosAnimator; } }

    public static bool CanShoot { get { 
        return state != SWAPPING && 
            (state != EJECTING || PlayerInventory.CurrentGun.shotgunReload) && 
            PlayerInventory.CurrentGunName == activeGun; }}

    public void Init()
    {
        instance = this;

        SetState(RAISED);
        guns = new Dictionary<string, Transform>();

        foreach(var gun in Guns.guns)
        {
            guns.Add(gun.name, gun.transform);
            if(guns[gun.name] != null) guns[gun.name].gameObject.SetActive(false);
        }

        AtLowest();
    }

    bool CanReload { get { return state == RAISED; } }
    bool ShotgunAutoReload { get { return PlayerInventory.CurrentGun.shotgunReload && Time.time > PlayerInventory.CurrentGun.timeLastShot+PlayerInventory.CurrentGun.timeBetweenShots*2; } }

    public void CheckReload(bool force=false)
    {
        if(CanReload && PlayerInventory.CanReload && (ShotgunAutoReload || force || PlayerInventory.Ammo <= 0))
        {
            SetState(RELOADING);
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

    void Update()
    {
        if(state != RECOIL_BACK && state != RECOIL_FORWARD && PlayerInventory.hasGun[PlayerInventory._nextGun] && PlayerInventory._nextGun != PlayerInventory._currentGun)
        {
            SetState(SWAPPING);
        }

        state.Update();
    }
}
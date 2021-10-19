using System.Collections.Generic;
using UnityEngine;

using static PlayerAnimator.State;

public class PlayerAnimator : MonoBehaviour
{
    public static PlayerAnimator instance;

    // hierarchy
    public Transform gunPos;
    public Animator gunPosAnimator;

    public static Dictionary<string, GameObject> guns;
    public static string activeGun="";
    public enum State
    {
        RAISED,
        SWAPPING,
        LOWERED,
        RECOIL
    }
    public static State state;

    public void Init()
    {
        instance = this;

        state = LOWERED;
        guns = new Dictionary<string, GameObject>();

        foreach(var pair in Guns.guns)
        {
            guns.Add(pair.Key, pair.Value == null ? null : Instantiate(pair.Value.mesh, gunPos));
            if(guns[pair.Key] != null) guns[pair.Key].SetActive(false);
        }

        AtLowest();
    }

    public static bool CanShoot
    {
        get { return state == RAISED && PlayerInventory.CurrentGunName == activeGun; }
    }

    public void AtLowest()
    {
        state = LOWERED;
        if(activeGun != "")
        {
            guns[activeGun].SetActive(false);
        }
        activeGun = PlayerInventory.CurrentGunName;
        if(activeGun != "")
        {
            guns[activeGun].SetActive(true);
            gunPosAnimator.Play("Base Layer.Raising"); // raise
            state = SWAPPING;
        }
    }

    public void Recoil()
    {
        state = RECOIL;
        gunPosAnimator.Play("Base Layer.Recoil");
    }

    public void UpdateGun()
    {
        if(activeGun != PlayerInventory.CurrentGunName)
        {
            if(state == RAISED)
            {
                state = SWAPPING;
                gunPosAnimator.Play("Base Layer.Lowering"); // lower
            }
            else if(state == LOWERED)
            {
                AtLowest();
            }
        }
    }
}
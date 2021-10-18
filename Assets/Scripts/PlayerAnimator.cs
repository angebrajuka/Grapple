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
        LOWERED
    }
    public State state;

    public void Init()
    {
        instance = this;

        state = LOWERED;
        guns = new Dictionary<string, GameObject>();

        foreach(var pair in Guns.guns)
        {
            guns.Add(pair.Key, pair.Value == null ? null : Instantiate(pair.Value.mesh, gunPos));
            if(guns[pair.Key] != null) guns[pair.Key].SetActive(true);
        }

        AtLowest();
    }

    public bool CanShoot
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
            gunPosAnimator.SetInteger("state", 0);
            state = SWAPPING;
        }
    }

    public void UpdateGun()
    {
        if(activeGun != PlayerInventory.CurrentGunName)
        {
            if(state == RAISED)
            {
                state = SWAPPING;
                gunPosAnimator.SetInteger("state", 1); // lower
            }
            else if(state == LOWERED)
            {
                AtLowest();
            }
        }
    }
}
using UnityEngine;

public class ReloadAnimationEvents : MonoBehaviour
{
    public Animator animator;

    public void Sound(int index)
    {
        Debug.Log(index);
        AudioManager.PlayClip(PlayerInventory.CurrentGunStats.clip_reloads[index], PlayerInventory.CurrentGunStats.volume_reloads[index]);
    }

    public void Finish()
    {
        animator.SetInteger("state", -1);
        PlayerInventory.Ammo = PlayerInventory.CurrentGunStats.magSize;
    }

    public void Update()
    {
        // playerHand.position = hand.localPostion;
        // playerHand.localEulerAngles = hand.localEulerAngles;
    }
}
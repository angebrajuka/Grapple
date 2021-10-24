using UnityEngine;

public class ReloadAnimationEvents : MonoBehaviour
{
    public Animator animator;

    public void Sound(int index)
    {
        AudioManager.PlayClip(PlayerInventory.CurrentGun.clip_reloads[index], PlayerInventory.CurrentGun.volume_reloads[index]);
    }

    public void Finish()
    {
        animator.SetBool("reloading", false);
        PlayerInventory.Ammo = PlayerInventory.CurrentGun.magSize;
    }
}
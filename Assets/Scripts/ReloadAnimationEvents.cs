using UnityEngine;

public class ReloadAnimationEvents : MonoBehaviour
{
    // hierarchy
    public Transform[] doubleBarrelShells;
    public Transform pumpActionShell;
    public Animator animator;
    public GameObject prefab_shell;

    public void Sound(int index)
    {
        AudioManager.PlayClip(PlayerInventory.CurrentGun.clip_reloads[index], PlayerInventory.CurrentGun.volume_reloads[index]);
    }

    public void ShellEject(GameObject go, Transform origin, Vector3 velocity)
    {
        var shell = Instantiate(prefab_shell, origin.position, origin.rotation);
        var rb = shell.GetComponent<Rigidbody>();
        rb.velocity += PlayerMovement.m_rigidbody.velocity;
        rb.AddRelativeForce(velocity);
    }

    public void ShellEjectDB(float force)
    {
        foreach(var shell in doubleBarrelShells)
        {
            ShellEject(prefab_shell, shell, Vector3.up*force);
        }
    }

    public void ShellEjectPA(float force)
    {
        ShellEject(prefab_shell, pumpActionShell, Vector3.right*force);
    }

    public void Idle()
    {
        PlayerInventory.CurrentGun.primed = true;
        animator.SetInteger("state", 0);
    }

    public void Finish()
    {
        if(!PlayerAnimator.IsReloading) return;

        PlayerShooting.FinishReload();
        if(PlayerInventory._nextGun == PlayerInventory._currentGun && PlayerInventory.CurrentGun.shotgunReload && PlayerInventory.Ammo < PlayerInventory.CurrentGun.magSize && PlayerInventory.ReserveAmmo >= PlayerInventory.CurrentGun.ammoPerShot)
        {
            PlayerAnimator.instance.CheckReload(true);
            return;
        }
        animator.SetInteger("state", 0);
    }
}
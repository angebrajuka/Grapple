using UnityEngine;

public class ReloadAnimationEvents : MonoBehaviour
{
    // hierarchy
    public Transform[] doubleBarrelShells;
    public Transform pumpActionShell;
    public Transform[] grenadeLauncherShell;
    public Animator animator;
    public GameObject prefab_shell;
    public GameObject prefab_shell_grenade;

    public void Sound(AnimationEvent e)
    {
        // if(e.stringParameter == "true" && animator.GetInteger("state") != e.intParameter) return;
        AudioManager.PlayClip((AudioClip)e.objectReferenceParameter, e.floatParameter);
    }

    public void ShellEject(GameObject go, Transform origin, Vector3 velocity)
    {
        var shell = Instantiate(prefab_shell, origin.position, origin.rotation);
        var rb = shell.GetComponent<Rigidbody>();
        rb.velocity += PlayerMovement.rb.velocity;
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

    public void ShellEjectGrenadeLauncher(float force)
    {
        // ShellEject(prefab_shell_grenade, grenadeLauncherShell, Vector3.up*force);
    }

    public void Idle()
    {
        PlayerInventory.CurrentGun.primed = true;
        animator.SetInteger("state", 0);
    }

    public void Finish()
    {
        if(!PlayerAnimator.IsReloading) return;

        PlayerInventory.FinishReload();
        if(PlayerInventory._nextGun == PlayerInventory._currentGun && PlayerInventory.CurrentGun.shotgunReload && PlayerInventory.Ammo < PlayerInventory.CurrentGun.magSize && PlayerInventory.ReserveAmmo >= PlayerInventory.CurrentGun.ammoPerShot)
        {
            PlayerAnimator.instance.CheckReload(true);
        }
        else if(PlayerInventory.CurrentGun.animateBetweenShots)
        {
            animator.SetInteger("state", 2);
        }
        else
        {
            animator.SetInteger("state", 0);
        }
    }
}
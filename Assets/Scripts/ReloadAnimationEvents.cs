using UnityEngine;

public class ReloadAnimationEvents : MonoBehaviour
{
    // hierarchy
    public Transform[] doubleBarrelShells;
    public Transform pumpActionShell;
    public Transform[] grenadeLauncherShell;
    public Transform laserShell;
    public Animator animator;
    public GameObject prefab_shell;
    public GameObject prefab_shell_grenade;
    public GameObject prefab_shell_laser;

    public void Sound(AnimationEvent e)
    {
        if(PlayerAnimator.state != PlayerAnimator.RELOADING && PlayerAnimator.state != PlayerAnimator.EJECTING && PlayerAnimator.state != PlayerAnimator.PRIMING) return;
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

    public void ShellEjectLR(float force)
    {
        ShellEject(prefab_shell_laser, laserShell, (Vector3.right+Vector3.forward)*force);
    }

    public void ShellEjectGrenadeLauncher(float force)
    {
        // ShellEject(prefab_shell_grenade, grenadeLauncherShell, Vector3.up*force);
    }

    public void SetRaised() {
        if(PlayerAnimator.state == PlayerAnimator.SWAPPING) return;
        PlayerAnimator.SetState(PlayerAnimator.RAISED);
    }

    public void Primed() {
        if(PlayerAnimator.state == PlayerAnimator.SWAPPING) return;
        PlayerInventory.CurrentGun.chamber = Gun.Chamber.FULL;
    }

    public void Ejected() {
        if(PlayerAnimator.state == PlayerAnimator.SWAPPING) return;
        PlayerInventory.CurrentGun.chamber = Gun.Chamber.EMPTY;
    }

    public void UpdateAmmo()
    {
        if(PlayerAnimator.state != PlayerAnimator.RELOADING) return;
        PlayerInventory.FinishReload();
    }

    public void Finish()
    {
        if(PlayerAnimator.state != PlayerAnimator.RELOADING) return;

        if(PlayerInventory._nextGun == PlayerInventory._currentGun && PlayerInventory.CurrentGun.shotgunReload && PlayerInventory.CurrentGun.chamber == Gun.Chamber.FULL && PlayerInventory.Ammo < PlayerInventory.CurrentGun.magSize && PlayerInventory.ReserveAmmo >= PlayerInventory.CurrentGun.ammoPerShot)
        {
            PlayerAnimator.instance.CheckReload(true);
        }
        else if(PlayerInventory.CurrentGun.chamber == Gun.Chamber.EMPTY)
        {
            PlayerAnimator.SetState(PlayerAnimator.PRIMING);
        }
        else
        {
            PlayerAnimator.SetState(PlayerAnimator.RAISED);
        }
    }
}
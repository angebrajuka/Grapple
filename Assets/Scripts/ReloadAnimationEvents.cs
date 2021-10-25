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

    public void Finish()
    {
        animator.SetBool("reloading", false);
        PlayerShooting.FinishReload();
    }
}
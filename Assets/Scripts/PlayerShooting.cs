using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // hierarchy
    public Transform gunPosition;
    public float moveSpeed;

    public static bool ShootBullet(Gun gun)
    {
        var directionOffset = Vector3.zero;

        RaycastHit hit;
        if(PlayerEyes.Raycast(out hit, gun.range, gun.barrelLength*Vector3.forward, directionOffset))
        {
            // gun logic here

            return true;
        }

        return false;
    }

    public static bool Shoot(Gun gun)
    {
        AudioManager.PlayClip(PlayerInventory.CurrentGun.clip_shoot);
        PlayerMovement.m_rigidbody.AddForce(PlayerMovement.instance.t_camera.TransformPoint(0, 0, -gun.recoil));
        PlayerAnimator.instance.Recoil();
        PlayerInventory.CurrentGun.timeLastShot = Time.time;
        PlayerInventory.Ammo -= gun.ammoPerShot;

        bool hit = false;
        for(int i=0; i<gun.pellets; i++)
        {
            hit = ShootBullet(gun) || hit;
        }

        return hit;
    }

    public static bool CanShoot
    {
        get { return PlayerAnimator.state != PlayerAnimator.State.SWAPPING && 
            PlayerAnimator.instance.gunReloadAnimator.GetBool("reloading") == false && 
            PlayerInventory.CurrentGunName == PlayerAnimator.activeGun && 
            PlayerInventory.Ammo >= PlayerInventory.CurrentGun.ammoPerShot && 
            Time.time > PlayerInventory.CurrentGun.timeLastShot+PlayerInventory.CurrentGun.timeBetweenShots; }
    }

    void Update()
    {
        // var targetDirection = PlayerMovement.instance.t_camera.forward;

        // RaycastHit hit;
        // if(PlayerEyes.Raycast(out hit, Mathf.Infinity, Vector3.forward*2))
        // {
        //     targetDirection = (hit.point-gunPosition.position).normalized;
        // }

        // var newDirection = Vector3.MoveTowards(gunPosition.forward, targetDirection, moveSpeed*Time.deltaTime);
        // gunPosition.LookAt(gunPosition.position+newDirection);
        // var euler = gunPosition.localEulerAngles;
        // euler.z = 0;
        // gunPosition.localEulerAngles = euler;

        if(PlayerInventory.CurrentGun == null) return;

        if(CanShoot && PlayerInput.GetKey("shoot"))
        {
            Shoot(PlayerInventory.CurrentGun);
        }
    }
}
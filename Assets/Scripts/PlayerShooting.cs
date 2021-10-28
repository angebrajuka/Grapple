using UnityEngine;

using static PlayerInventory;

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
        PlayerAnimator.instance.gunReloadAnimator.SetInteger("state", 0);
        AudioManager.PlayClip(CurrentGun.clip_shoot);
        PlayerMovement.m_rigidbody.AddForce(PlayerMovement.instance.t_camera.TransformPoint(0, 0, -gun.recoil));
        PlayerAnimator.instance.Recoil();
        CurrentGun.timeLastShot = Time.time;
        Ammo -= gun.ammoPerShot;

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
            (!PlayerAnimator.IsReloading || CurrentGun.shotgunReload) && 
            (PlayerAnimator.IsIdle || CurrentGun.shotgunReload) && 
            CurrentGunName == PlayerAnimator.activeGun && 
            Ammo >= CurrentGun.ammoPerShot && 
            Time.time > CurrentGun.timeLastShot+CurrentGun.timeBetweenShots; }
    }

    public static void FinishReload()
    {
        if(CurrentGun.shotgunReload)
        {
            ReserveAmmo --;
            Ammo ++;
        }
        else
        {
            int _ammo = ReserveAmmo/CurrentGun.ammoPerShot;
            int _clip = CurrentGun.ammo/CurrentGun.ammoPerShot;
            int _clipSize = CurrentGun.magSize/CurrentGun.ammoPerShot;
            
            if(_ammo > _clipSize - _clip)
            {
                ReserveAmmo -= (_clipSize - _clip)*CurrentGun.ammoPerShot;
                Ammo = CurrentGun.magSize;
            }
            else if(_ammo > 0)
            {
                int num = _ammo*CurrentGun.ammoPerShot;
                Ammo += num;
                ReserveAmmo -= num;
            }
        }
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

        if(CurrentGun == null) return;

        if(CanShoot && PlayerInput.GetKey("shoot"))
        {
            Shoot(CurrentGun);
        }
    }
}
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // hierarchy
    public Transform gunPosition;
    public float moveSpeed;

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
    }

    public static bool ShootBullet(Gun gun)
    {
        var directionOffset = Vector3.zero;

        RaycastHit hit;
        if(PlayerEyes.Raycast(out hit, gun.range, gun.vec_barrelTip, directionOffset))
        {
            // gun logic here

            return true;
        }

        return false;
    }

    public static bool Shoot(Gun gun)
    {
        bool hit = false;

        for(int i=0; i<gun.pellets; i++)
        {
            hit = ShootBullet(gun) || hit;
        }

        return hit;
    }
}
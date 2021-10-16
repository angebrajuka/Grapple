using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // hierarchy
    public Transform gunPosition;
    public float moveSpeed;

    void Update()
    {
        var targetDirection = PlayerMovement.instance.t_camera.forward;

        RaycastHit hit;
        if(PlayerEyes.Raycast(out hit, Mathf.Infinity, Vector3.forward*2))
        {
            targetDirection = (hit.point-gunPosition.position).normalized;
        }

        var newDirection = Vector3.MoveTowards(gunPosition.forward, targetDirection, moveSpeed*Time.deltaTime);
        gunPosition.LookAt(gunPosition.position+newDirection);
        var euler = gunPosition.localEulerAngles;
        euler.z = 0;
        gunPosition.localEulerAngles = euler;
    }
}
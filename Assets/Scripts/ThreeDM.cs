using UnityEngine;
using System.Collections.Generic;
using static PlayerInput;

public class ThreeDM : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_hook;
    public float shootForce;
    public float autoRetractSpeedFast;
    public float autoRetractSpeedSlow;
    public float recoilForce;
    public float minDistance;
    public float destroyDistance;

    GrappleHook hook;

    public void ShootHook()
    {
        var obj = Instantiate(prefab_hook, transform.position, PlayerMovement.instance.t_camera.rotation, null);
        hook = obj.GetComponent<GrappleHook>();
        hook.threeDM = this;
        hook.configJoint.connectedBody = PlayerMovement.m_rigidbody;
        var rb = obj.GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward*shootForce);
        rb.AddForce(PlayerMovement.m_rigidbody.velocity);
        PlayerMovement.m_rigidbody.AddRelativeForce(0, 0, -recoilForce);
    }

    void Update()
    {
        if(GetKeyDown("grapple_shoot"))
        {
            if(hook == null) ShootHook();
            else hook.Retract();
        }
        if(hook != null)
        {
            if(GetKeyDown("grapple_end"))
            {
                hook.Retract();
                hook.configJoint.connectedMassScale = 0;
                Destroy(hook.fixedJoint);
            }
        }
    }
}
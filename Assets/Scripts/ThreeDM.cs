using UnityEngine;
using System.Collections.Generic;
using static PlayerInput;

public class ThreeDM : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_hook;
    public float shootSpeed;
    public float maxAdjustSpeed;
    public float autoRetractSpringForce;
    public float autoRetractSpringDamper;

    GrappleHook hook;

    public void ShootHook()
    {
        var obj = Instantiate(prefab_hook, transform.position, PlayerMovement.instance.t_camera.rotation, null);
        hook = obj.GetComponent<GrappleHook>();
        hook.threeDM = this;
        hook.configJoint.connectedBody = PlayerMovement.m_rigidbody;
        hook.springJoint.connectedBody = PlayerMovement.m_rigidbody;
        var rb = obj.GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward*shootSpeed);
    }

    public void DestroyHook()
    {
        Destroy(hook.gameObject);
        hook = null;
    }

    void Update()
    {
        if(GetKeyDown("grapple_shoot"))
        {
            if(hook == null) ShootHook();
            else hook.Retract();
        }
    }
}
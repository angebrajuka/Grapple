using UnityEngine;
using System.Collections.Generic;

public class ThreeDM : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_hook;
    public float shootSpeed;
    public float maxAdjustSpeed;
    public float autoRetractSpringForce;
    public float autoRetractSpringDamper;

    GrappleHook hook;
    float targetLength=0, length=0;

    public void DestroyHook()
    {
        Destroy(hook.gameObject);
        hook = null;
    }

    public void ShootHook()
    {
        if(hook == null)
        {
            var obj = Instantiate(prefab_hook, transform.position, PlayerMovement.instance.t_camera.rotation, null);
            hook = obj.GetComponent<GrappleHook>();
            hook.threeDM = this;
            hook.configJoint.connectedBody = PlayerMovement.m_rigidbody;
            hook.springJoint.connectedBody = PlayerMovement.m_rigidbody;
            var rb = obj.GetComponent<Rigidbody>();
            rb.AddRelativeForce(Vector3.forward*shootSpeed);
            targetLength = 0;
        }
        else
        {
            hook.Retract();
            // var ll = hook.configJoint.linearLimit;
            // ll.contactDistance = 0;
            // ll.limit = 0;
            // ll.bounciness = 0;
            // hook.configJoint.linearLimit = ll;
            // var lls = hook.configJoint.linearLimitSpring;
            // lls.spring = autoRetractSpringForce;
            // hook.configJoint.linearLimitSpring = lls;
        }
    }

    public void AdjustDistance(float amount)
    {
        if(hook == null) return;

        // targetLength += amount;
        // while(targetLength - length > 1)
        // {
        //     length += 1;
        //     var oldLast = cableSegments.Count == 0 ? hook.gameObject : cableSegments.Peek();
        //     var newLast = Instantiate(prefab_cableSegment, transform.position, PlayerMovement.instance.t_camera.rotation, null);
        //     cableSegments.Push(newLast);
        //     var newJoint = newLast.GetComponent<ConfigurableJoint>();
        //     newJoint.connectedBody = oldLast.GetComponent<Rigidbody>();
        // }
        // while(targetLength - length < 1)
        // {
        //     length -= 1;
        //     if(cableSegments.Count > 0)
        //     {
        //         Destroy(cableSegments.Pop());
        //         if(cableSegments.Count <= 1)
        //         {
        //             DestroyHook();
        //         }
        //         else
        //         {
        //             cableSegments.Peek().GetComponent<ConfigurableJoint>().connectedBody = PlayerMovement.m_rigidbody;
        //         }
        //     }
        //     else
        //     {
        //     }
        // }
        // if(cableSegments.Count > 0)
        //     joint.connectedBody = cableSegments.Peek().GetComponent<Rigidbody>();
    }

    public void Update()
    {
        // if(hook != null && hook.enabled && (cableSegments.Count == 0 || Vector3.Distance(cableSegments.Peek().transform.position, transform.position) > 1))
        // {
        //     AdjustDistance(1);
        // }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class ThreeDM : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_hook;
    public GameObject prefab_cableSegment;
    public ConfigurableJoint joint;
    public float shootSpeed;
    public float maxAdjustSpeed;
    public float autoRetractSpeed;

    GrappleHook hook;
    Stack<GameObject> cableSegments = new Stack<GameObject>();
    bool autoRetracting;
    float targetLength=0, length=0;

    public void SetJointMotion(ConfigurableJointMotion motionType)
    {
        joint.xMotion = motionType;
        joint.yMotion = motionType;
        joint.zMotion = motionType;
    }

    public void DestroyHook()
    {
        Destroy(hook.gameObject);
        while(cableSegments.Count > 0)
        {
            Destroy(cableSegments.Pop());
        }
        hook = null;
    }

    public void ShootHook()
    {
        if(hook == null)
        {
            var obj = Instantiate(prefab_hook, transform.position, PlayerMovement.instance.t_camera.rotation, null);
            hook = obj.GetComponent<GrappleHook>();
            hook.threeDM = this;
            var rb = obj.GetComponent<Rigidbody>();
            rb.AddRelativeForce(Vector3.forward*shootSpeed);
            autoRetracting = false;
            targetLength = 0;
        }
        else
        {
            hook.enabled = false;
            autoRetracting = true;
            hook.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void AdjustDistance(float amount)
    {
        if(hook == null) return;

        targetLength += amount;
        while(targetLength - length > 1)
        {
            length += 1;
            var oldLast = cableSegments.Count == 0 ? hook.gameObject : cableSegments.Peek();
            var newLast = Instantiate(prefab_cableSegment, transform.position, PlayerMovement.instance.t_camera.rotation, null);
            cableSegments.Push(newLast);
            var newJoint = newLast.GetComponent<ConfigurableJoint>();
            newJoint.connectedBody = oldLast.GetComponent<Rigidbody>();
        }
        while(targetLength - length < 1)
        {
            length -= 1;
            if(cableSegments.Count > 0)
            {
                Destroy(cableSegments.Pop());
                if(cableSegments.Count <= 1)
                {
                    DestroyHook();
                }
                else
                {
                    cableSegments.Peek().GetComponent<ConfigurableJoint>().connectedBody = PlayerMovement.m_rigidbody;
                }
            }
            else
            {
            }
        }
        if(cableSegments.Count > 0)
            joint.connectedBody = cableSegments.Peek().GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if(hook != null && hook.enabled && (cableSegments.Count == 0 || Vector3.Distance(cableSegments.Peek().transform.position, transform.position) > 1))
        {
            AdjustDistance(1);
        }
        if(autoRetracting)
        {
            AdjustDistance(-autoRetractSpeed*Time.deltaTime);
        }
    }
}
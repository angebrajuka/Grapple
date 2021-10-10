using UnityEngine;
using System.Collections.Generic;

public class Single3DM : MonoBehaviour
{
    // hierarchy
    public Player3DM p3dm;

    GrappleHook hook;
    Stack<GameObject> cableSegments = new Stack<GameObject>();
    bool autoRetracting;
    float targetLength=0, length=0;

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
            var obj = Instantiate(p3dm.prefab_hook, transform.position, transform.rotation, null);
            hook = obj.GetComponent<GrappleHook>();
            var rb = obj.GetComponent<Rigidbody>();
            rb.AddRelativeForce(Vector3.forward*p3dm.shootSpeed);
        }
        else
        {
            hook.enabled = false;
            autoRetracting = true;
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
            var newLast = Instantiate(p3dm.prefab_cableSegment, transform.position, transform.rotation, null);
            var oldJoint = oldLast.GetComponent<ConfigurableJoint>();
            oldJoint.connectedBody = newLast.GetComponent<Rigidbody>();
            cableSegments.Push(newLast);
            var newJoint = newLast.GetComponent<ConfigurableJoint>();
            newJoint.connectedBody = PlayerMovement.m_rigidbody;
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
    }

    public void Update()
    {
        if(hook != null && hook.enabled && (cableSegments.Count == 0 || Vector3.Distance(cableSegments.Peek().transform.position, transform.position) > 1))
        {
            AdjustDistance(1);
        }
        if(autoRetracting)
        {
            AdjustDistance(p3dm.autoRetractSpeed*Time.deltaTime);
        }
    }
}
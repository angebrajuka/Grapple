using UnityEngine;
using System.Collections.Generic;

public class Single3DM : MonoBehaviour
{
    // hierarchy
    public Player3DM p3dm;

    GrappleHook hook;
    Stack<GameObject> cableSegments = new Stack<GameObject>();
    bool autoRetracting;

    public void DeleteHook()
    {
        Destroy(hook.gameObject);
        while(cableSegments.Count > 0)
        {
            Destroy(cableSegments.Pop());
        }
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

    }

    public void Update()
    {
        if(hook != null && hook.enabled && (cableSegments.Count == 0 || Vector3.Distance(cableSegments.Peek().transform.position, transform.position) > 1))
        {
            var oldLast = cableSegments.Count == 0 ? hook.gameObject : cableSegments.Peek();
            var newLast = Instantiate(p3dm.prefab_cableSegment, transform.position, transform.rotation, null);
            var oldJoint = oldLast.GetComponent<ConfigurableJoint>();
            oldJoint.connectedBody = newLast.GetComponent<Rigidbody>();
            var pos = oldJoint.connectedAnchor;
            pos.Set(0, 0, 0);
            oldJoint.connectedAnchor = pos;
            cableSegments.Push(newLast);
            var newJoint = newLast.GetComponent<ConfigurableJoint>();
            newJoint.connectedBody = PlayerMovement.m_rigidbody;
            newJoint.connectedAnchor += transform.localPosition;
        }
        if(autoRetracting)
        {
            AdjustDistance(p3dm.autoRetractSpeed);
        }
    }
}
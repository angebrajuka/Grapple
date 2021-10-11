using UnityEngine;
using System.Collections.Generic;

public class RopeNode
{
    Rigidbody m_rigidbody;
    public bool Leading 
    { // leading node (hook) will have this set to false, all others will have it set to true
        get { return m_rigidbody.isKinematic; }
    }

    public void FixedUpdate(Vector3 gravity, RopeNode prev)
    {
        m_rigidbody.MovePosition(gravity);
    }
}

public class Rope : MonoBehaviour
{
    public LinkedList<RopeNode> nodes; // first will be lead (hook), last will be player

    void FixedUpdate()
    {
        if(nodes.Count <= 1) return;

        Vector3 gravity = Physics.gravity * 1;

        for(var node=nodes.First.Next; node != null; node=node.Next) // skip skips the leading node (hook)
        {
            node.Value.FixedUpdate(gravity, node.Previous.Value);
        }
    }
}
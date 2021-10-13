using UnityEngine;
using static GrappleHook.State;

public class GrappleHook : MonoBehaviour
{
    // hierarchy
    public ConfigurableJoint configJoint;
    public Rigidbody m_rigidbody;

    public FixedJoint fixedJoint;
    public ThreeDM threeDM;

    public enum State
    {
        SHOOTING,
        SWINGING,
        RETRACTING
    }
    public State state = SHOOTING;

    public void LockMotion(Rigidbody other)
    {
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = other;
    }

    public void Retract()
    {
        if(state == RETRACTING) return;

        state = RETRACTING;
        configJoint.SetDistance();
    }

    void OnCollisionEnter(Collision other)
    {
        if(state != SHOOTING) return;

        LockMotion(other.rigidbody);
        configJoint.SetDistance();
        state = SWINGING;
    }

    void FixedUpdate()
    {
        if(state == RETRACTING && configJoint.linearLimit.limit > threeDM.minDistance)
        {
            var ll = configJoint.linearLimit;
            ll.limit -= (fixedJoint == null ? threeDM.autoRetractSpeedFast : threeDM.autoRetractSpeedSlow);
            configJoint.linearLimit = ll;
            var quat = Quaternion.LookRotation(m_rigidbody.position-PlayerMovement.m_rigidbody.position);
            m_rigidbody.rotation = quat;
            // TODO  fix this patch mess of rotation
        }
    }

    void Update()
    {
        if(state == RETRACTING && fixedJoint == null && Vector3.Distance(configJoint.GetStart(), configJoint.GetEnd()) < threeDM.destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
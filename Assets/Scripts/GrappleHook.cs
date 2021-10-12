using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    // hierarchy
    public ConfigurableJoint configJoint;
    public SpringJoint springJoint;
    public Rigidbody m_rigidbody;

    public FixedJoint fixedJoint;
    public ThreeDM threeDM;

    public void SetJointMotion(bool active)
    {
        ConfigurableJointMotion motionType = active ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
        configJoint.xMotion = motionType;
        configJoint.yMotion = motionType;
        configJoint.zMotion = motionType;
    }

    public void LockMotion(Rigidbody other)
    {
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = other;
        SetJointMotion(true);
    }

    public void Retract()
    {
        if(fixedJoint != null && fixedJoint.connectedBody == null) Destroy(fixedJoint);

        SetJointMotion(false);

        springJoint.spring = threeDM.autoRetractSpringForce;
        springJoint.damper = threeDM.autoRetractSpringDamper;

        enabled = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if(!enabled) return;

        LockMotion(other.rigidbody);

        enabled = false;
    }
}
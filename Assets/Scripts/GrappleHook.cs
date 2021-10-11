using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public ThreeDM threeDM;

    void OnCollisionEnter(Collision other)
    {
        if(!enabled) return;

        threeDM.SetJointMotion(ConfigurableJointMotion.Limited);
        GetComponent<Rigidbody>().isKinematic = true;
        enabled = false;
    }
}
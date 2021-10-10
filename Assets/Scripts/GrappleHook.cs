using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if(!enabled) return;

        GetComponent<Rigidbody>().isKinematic = true;
        enabled = false;
    }
}
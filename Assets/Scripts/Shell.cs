using UnityEngine;

public class Shell : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Random.onUnitSphere*Random.Range(5f, 50f));
        rb.AddTorque(Vector3.right*Random.Range(100f, 200f));
        Destroy(gameObject, 0.4f);
    }

    void FixedUpdate()
    {
        // extra gravity
        rb.AddForce(0, -8, 0);
    }
}
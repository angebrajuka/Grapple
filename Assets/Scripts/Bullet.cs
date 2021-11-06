using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float range;
    public float distancePerTick;
    public LayerMask layermask;

    float distanceTravelled;

    void Start()
    {
        distanceTravelled = 0;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, distancePerTick, ~layermask))
        {
            Destroy(gameObject);
        }
        transform.position += transform.forward*distancePerTick;
        distanceTravelled += distancePerTick;
        if(distanceTravelled >= range)
        {
            Destroy(gameObject);
        }
    }
}
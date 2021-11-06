using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float range;
    public float damage;
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
            var target = hit.transform.gameObject.GetComponent<Target>();
            if(target != null)
            {
                target.Damage(damage, transform.forward);
            }

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
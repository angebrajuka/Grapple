using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float range;
    public float damage;
    public float distancePerTick;
    public LayerMask layermask;

    float distanceTravelled;

    public ObjectPool pool;

    void OnEnable()
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

            pool.Return(gameObject);
        }
        transform.position += transform.forward*distancePerTick;
        distanceTravelled += distancePerTick;
        if(distanceTravelled >= range)
        {
            pool.Return(gameObject);
        }
    }
}
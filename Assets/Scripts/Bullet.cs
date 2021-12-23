using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    // hierarchy
    public bool grenade;
    public GameObject mesh;
    public GameObject trail;

    public float range;
    public float damage;
    public float distancePerTick;
    public LayerMask layermask;

    float distanceTravelled;

    public ObjectPool<Bullet> pool;

    void OnEnable()
    {
        distanceTravelled = 0;
        mesh.SetActive(false);
        trail.SetActive(false);
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

            pool.Release(this);
        }
        transform.position += transform.forward*distancePerTick;
        distanceTravelled += distancePerTick;
        mesh.SetActive(true);
        trail.SetActive(true);
        if(distanceTravelled >= range)
        {
            pool.Release(this);
        }
    }
}
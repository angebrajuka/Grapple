using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    // hierarchy
    public bool grenade;
    public GameObject mesh;
    public GameObject trail;
    public Rigidbody rb;
    [Header("Grenade Specific")]
    public float detonationTime;
    public float randomRotation;
    public GameObject prefab_explosion;

    public float lifetime;
    public float damage;
    public float distancePerTick;
    public LayerMask layermask;
    // grenade specific
    bool firstBounce;

    float timeShot;

    public ObjectPool<Bullet> pool;

    public void SetRange(float range)
    {
        lifetime = range / rb.velocity.magnitude;
    }

    public void OnGet()
    {
        timeShot = Time.time;
        mesh.SetActive(false);
        if(trail != null) 
        {
            trail.GetComponent<TrailRenderer>().Clear();
            trail.SetActive(false);
        }

        if(grenade)
        {
            firstBounce = true;
            rb.AddTorque(new Vector3(Random.value, Random.value, Random.value)*randomRotation);
        }
    }

    public void DetonateGrenade()
    {
        Instantiate(prefab_explosion, transform.position, transform.rotation, transform.parent);
        pool.Release(this);
    }

    void OnCollisionEnter(Collision c)
    {
        var target = c.gameObject.GetComponent<Target>();
        if(target != null)
        {
            if(!grenade)
            {
                target.Damage(damage, transform.forward);
            }
            else if(firstBounce)
            {
                DetonateGrenade();
            }
        }
        firstBounce = false;

        if(!grenade) pool.Release(this);
    }

    void FixedUpdate()
    {
        mesh.SetActive(true);
        if(trail != null) trail.SetActive(true);
        if(Time.time >= timeShot + (grenade ? detonationTime : lifetime))
        {
            if(grenade) DetonateGrenade();
            pool.Release(this);
        }
    }
}
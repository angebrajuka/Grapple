using UnityEngine;

public class Grenade : MonoBehaviour
{
    // hierarchy
    public float detonationTime;
    public float randomRotation;
    public GameObject prefab_explosion;

    bool firstBounce;
    float time_primed;

    void Start()
    {
        firstBounce = true;
        time_primed = Time.time;
        GetComponent<Rigidbody>().AddTorque(new Vector3(Random.value, Random.value, Random.value)*randomRotation);
    }

    public void Detonate()
    {
        Instantiate(prefab_explosion, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        var target = other.gameObject.GetComponent<Target>();

        if(firstBounce && target != null)
        {
            Detonate();
        }

        firstBounce = false;
    }

    void Update()
    {
        if(Time.time > time_primed+detonationTime)
        {
            Detonate();
        }
    }
}
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    // hierarchy
    public Transform t_camera;

    // components
    public static Rigidbody m_rigidbody;

    // stats
    Vector3 normal;
    bool grounded;

    public void Init()
    {
        instance = this;
        
        m_rigidbody = GetComponent<Rigidbody>();

        normal = new Vector3(0, 0, 0);
    }

    void OnCollisionExit(Collision collision) {
        normal.Set(0, 0, 0);
        grounded = false;
    }

    void OnCollisionStay(Collision collision) {
        for(int i=0; i < collision.contactCount; i++) {
            Vector3 cnormal = collision.contacts[i].normal;
            if(cnormal.y > normal.y) {
                normal = cnormal;
            }
        }

        grounded = normal.y > PlayerStats.GROUND_NORMAL;
    }

    void FixedUpdate()
    {
        if(grounded)
        {
            // accellerate
            m_rigidbody.AddRelativeForce(PlayerInput.input_move.x*PlayerStats.WALK_ACCEL, 0, PlayerInput.input_move.z*PlayerStats.WALK_ACCEL);

            // get vel
            Vector3 vel = m_rigidbody.velocity;
            
            // speed cap
            if(vel.magnitude > PlayerStats.MAX_WALK_SPEED)
            {
                vel.Normalize();
                vel *= PlayerStats.MAX_WALK_SPEED;
            }

            // friction
            if(PlayerInput.input_move.x == 0 && PlayerInput.input_move.z == 0)
            {
                vel *= PlayerStats.FRICTION;
            }

            // set vel
            m_rigidbody.velocity = vel;
        }
    }

    void LateUpdate()
    {
        Vector3 rotation;
        if(PlayerInput.input_look.x != 0)
        {
            rotation = m_rigidbody.rotation.eulerAngles;
            rotation.y += PlayerInput.input_look.x;
            m_rigidbody.rotation = Quaternion.Euler(rotation);
        }
        if(PlayerInput.input_look.y != 0)
        {
            rotation = t_camera.localEulerAngles;
            rotation.x -= PlayerInput.input_look.y;
            if(rotation.x > 90 && rotation.x <= 180)        rotation.x = 90;
            else if(rotation.x < 270 && rotation.x >= 180)  rotation.x = 270;
            t_camera.localEulerAngles = rotation;
        }
    }
}
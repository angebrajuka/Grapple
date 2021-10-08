using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    // hierarchy
    public Transform t_camera;
    public float walkAccel;
    public float walkMaxSpeed;
    public float friction;
    public float groundNormal;
    public float jumpForce;

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

        grounded = normal.y > groundNormal;
    }

    void FixedUpdate()
    {
        if(grounded)
        {
            // accelerate
            m_rigidbody.AddRelativeForce(PlayerInput.input_move.x*walkAccel, 0, PlayerInput.input_move.z*walkAccel);
            
            m_rigidbody.AddForce(0, PlayerInput.input_move.y*jumpForce, 0);

            // get vel
            Vector3 vel = m_rigidbody.velocity;
            vel.y = 0; // dont care about y for speed cap & friction
            
            // speed cap
            if(vel.magnitude > walkMaxSpeed)
            {
                vel.Normalize();
                vel *= walkMaxSpeed;
            }

            // friction
            if(PlayerInput.input_move.x == 0 && PlayerInput.input_move.z == 0)
            {
                vel *= friction;
            }

            // set vel
            vel.y = m_rigidbody.velocity.y; // dont affect y for speed cap & friction
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
using UnityEngine;
using static PlayerInput;

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

    Vector3 normal;
    bool grounded;
    Vector3 input_move;
    Vector2 input_look;

    public void Init()
    {
        instance = this;

        m_rigidbody = GetComponent<Rigidbody>();

        normal = new Vector3(0, 0, 0);
        grounded = false;
        input_move = new Vector3(0, 0);
        input_look = new Vector2(0, 0);
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

    void Update()
    {
        input_move.Set(0, 0, 0);
        input_look.Set(0, 0);

        if(GetKey("walk_front"))    input_move.z ++;
        if(GetKey("walk_back"))     input_move.z --;
        if(GetKey("walk_left"))     input_move.x --;
        if(GetKey("walk_right"))    input_move.x ++;
        input_move.Normalize();

        if(GetKey("jump")) input_move.y ++;

        input_look.x = Input.GetAxis("Mouse X") * speed_look.x;
        input_look.y = Input.GetAxis("Mouse Y") * speed_look.y;
    }

    void FixedUpdate()
    {
        if(grounded)
        {
            // accelerate
            m_rigidbody.AddRelativeForce(
                Mathf.Abs(Vector3.Dot(m_rigidbody.velocity, m_rigidbody.transform.right)) < walkMaxSpeed ? input_move.x*walkAccel : 0,
                0,
                Mathf.Abs(Vector3.Dot(m_rigidbody.velocity, m_rigidbody.transform.forward)) < walkMaxSpeed ? input_move.z*walkAccel : 0
            );

            m_rigidbody.AddForce(0, input_move.y*jumpForce, 0);

            // friction
            Vector3 vel = m_rigidbody.velocity;
            vel *= friction;
            vel.y = m_rigidbody.velocity.y; // dont affect y for friction
            m_rigidbody.velocity = vel;
        }
    }

    void LateUpdate()
    {
        Vector3 rotation;
        if(input_look.x != 0)
        {
            rotation = m_rigidbody.rotation.eulerAngles;
            rotation.y += input_look.x;
            m_rigidbody.rotation = Quaternion.Euler(rotation);
        }
        if(input_look.y != 0)
        {
            rotation = t_camera.localEulerAngles;
            rotation.x -= input_look.y;
            if(rotation.x > 90 && rotation.x <= 180)        rotation.x = 90;
            else if(rotation.x < 270 && rotation.x >= 180)  rotation.x = 270;
            t_camera.localEulerAngles = rotation;
        }
    }
}
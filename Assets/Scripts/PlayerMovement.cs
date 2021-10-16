using UnityEngine;
using static PlayerInput;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    // hierarchy
    public Transform t_camera;
    public float walkAccel;
    public float airWalkAccel;
    public float walkMaxSpeed;
    public float friction_normal;
    public float friction_slide;
    public float groundNormal;
    public float jumpForce;
    public float slideForce;
    public float slideHeightAdjust;
    public float cameraHeightAdjustSpeed;

    // components
    public static Rigidbody m_rigidbody;
    public static CapsuleCollider m_collider;

    Vector3 normal;
    Vector3 cameraTargetPos;
    Vector3 cameraDefaultPos;
    bool grounded;
    bool p_sliding;
    bool sliding
    {
        get { return p_sliding; }
        set
        {
            bool wasSliding = p_sliding;
            p_sliding = value;
            if(sliding == wasSliding) return;

            int mult = sliding ? -1 : 1;
            m_collider.height += slideHeightAdjust * mult;
            var vec = Vector3.up*slideHeightAdjust/2 * mult;
            m_collider.center += vec;
            cameraTargetPos = vec;
        }
    }
    Vector3 input_move;
    Vector2 input_look;

    public void Init()
    {
        instance = this;

        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();

        cameraDefaultPos = t_camera.localPosition;

        normal = new Vector3(0, 0, 0);
        grounded = false;
        sliding = false;
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

        grounded = normal.y > groundNormal && Mathf.Abs(m_rigidbody.velocity.y) <= 1f;
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

        if(GetKeyDown("slide") && !sliding && grounded)
        {
            m_rigidbody.AddRelativeForce(0, 0, slideForce);
        }
        if(GetKeyUp("slide") && sliding && grounded && m_rigidbody.RelativeVelocity().z > slideForce)
        {
            m_rigidbody.AddRelativeForce(0, 0, -slideForce);
        }
        sliding = GetKey("slide");

        input_look.x = Input.GetAxis("Mouse X") * speed_look.x;
        input_look.y = Input.GetAxis("Mouse Y") * speed_look.y;

        t_camera.localPosition = Vector3.Lerp(t_camera.localPosition, cameraDefaultPos+cameraTargetPos, cameraHeightAdjustSpeed*Time.deltaTime);
    }

    void FixedUpdate()
    {
        // accelerate
        m_rigidbody.AddRelativeForce(
            Mathf.Abs(Vector3.Dot(m_rigidbody.velocity, m_rigidbody.transform.right)) < walkMaxSpeed ? (input_move.x*(grounded && !sliding ? walkAccel : airWalkAccel))*Time.fixedDeltaTime : 0,
            0,
            Mathf.Abs(Vector3.Dot(m_rigidbody.velocity, m_rigidbody.transform.forward)) < walkMaxSpeed ? (input_move.z*(grounded && !sliding ? walkAccel : airWalkAccel))*Time.fixedDeltaTime : 0
        );

        if(grounded)
        {
            // friction
            Vector3 vel = m_rigidbody.velocity;
            vel *= sliding ? friction_slide : friction_normal;
            vel.y = m_rigidbody.velocity.y; // dont affect y for friction
            m_rigidbody.velocity = vel;

            // jump
            m_rigidbody.AddForce(0, input_move.y*jumpForce, 0);
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
using UnityEngine;
using static PlayerInput;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    // hierarchy
    public Transform t_camera;
    public CapsuleCollider colliderDefault;
    public CapsuleCollider colliderCrouch;
    public SphereCollider colliderGrounded;
    public float walkAccelDefault;
    public float walkAccelCrouch;
    public float walkAccelAir;
    public float walkMaxSpeed;
    public float friction_normal;
    public float friction_slide;
    public float jumpForce;
    public float jumpTimeLeeway;
    public float timeBetweenJumps;
    public float slideForce;
    public float slideStartThreshhold;
    public float slideMaxSpeed;
    public float cameraHeightAdjustSpeed;

    // components
    public static Rigidbody rb;

    Vector3 cameraPosTarget;
    Vector3 cameraPosDefault;
    Vector3 cameraPosCrouch;
    float slideHeightAdjust;
    bool grounded;
    bool crouching;
    bool crouchKeyDown;
    Vector3 input_move;
    Vector2 input_look;
    bool input_crouch;
    float jumpInputTime;
    float timeLastJumped;

    public void Init()
    {
        instance = this;

        rb = GetComponent<Rigidbody>();


        slideHeightAdjust = colliderDefault.height - colliderCrouch.height;
        cameraPosTarget = Vector3.up*slideHeightAdjust/2;
        cameraPosDefault = t_camera.localPosition;
        cameraPosCrouch = cameraPosDefault - Vector3.up*slideHeightAdjust;

        grounded = false;
        crouching = false;
        input_move = new Vector3(0, 0);
        input_look = new Vector2(0, 0);
        input_crouch = false;
        crouchKeyDown = false;
        jumpInputTime = -1000;
        timeLastJumped = -1000;
    }

    void Update()
    {
        input_move.Set(0, 0, 0);
        input_look.Set(0, 0);

        if(PauseHandler.paused) return;

        if(GetKey("walk_front"))    input_move.z ++;
        if(GetKey("walk_back"))     input_move.z --;
        if(GetKey("walk_left"))     input_move.x --;
        if(GetKey("walk_right"))    input_move.x ++;
        input_move.Normalize();

        if(GetKeyDown("jump"))
        {
            jumpInputTime = Time.time;
        }
        if(Time.time <= jumpInputTime+jumpTimeLeeway && Time.time >= timeLastJumped+timeBetweenJumps)
        {
            input_move.y ++;
            timeLastJumped = Time.time;
        }

        crouchKeyDown = GetKeyDown("slide") || crouchKeyDown;
        input_crouch = GetKey("slide");

        input_look.x = Input.GetAxis("Mouse X") * speed_look.x;
        input_look.y = Input.GetAxis("Mouse Y") * speed_look.y;
    }

    void FixedUpdate()
    {
        bool wasGrounded = grounded;
        grounded = Physics.OverlapSphere(colliderGrounded.center+rb.position, colliderGrounded.radius, Layers.PLAYER_ALL, QueryTriggerInteraction.Ignore).Length > 0;

        if(grounded)
        {
            // friction
            Vector3 vel = rb.velocity;
            vel *= crouching ? friction_slide : friction_normal;
            vel.y = rb.velocity.y; // dont affect y for friction
            rb.velocity = vel;

            // jump
            rb.AddForce(0, input_move.y*jumpForce, 0);
        }

        if(PauseHandler.paused) return;

        // accelerate
        var accel = Time.fixedDeltaTime * (grounded ? (crouching ? walkAccelCrouch : walkAccelDefault) : walkAccelAir);

        rb.AddRelativeForce(
            Mathf.Abs(Vector3.Dot(rb.velocity, rb.transform.right)) < walkMaxSpeed ? input_move.x*accel : 0,
            0,
            Mathf.Abs(Vector3.Dot(rb.velocity, rb.transform.forward)) < walkMaxSpeed ? input_move.z*accel : 0
        );

        var center = colliderDefault.center+rb.position;
        var halfHeight = Vector3.up*(colliderDefault.height/2);
        var point0 = center + halfHeight;
        var point1 = center; // dont subtract half height for ignoring floor
        float threshhold = walkMaxSpeed*slideStartThreshhold;
        if(input_crouch)
        {
            bool wasCrouching = crouching;
            crouching = true;

            colliderDefault.enabled = false;
            colliderCrouch.enabled = true;
            cameraPosTarget = cameraPosCrouch;

            float zvel = rb.RelativeVelocity().z;
            if(crouchKeyDown && (!wasCrouching || !wasGrounded) && grounded && input_move.z > 0 && zvel < slideMaxSpeed)
            {
                rb.AddRelativeForce(0, 0, slideForce);
                crouchKeyDown = false;
            }
        }
        else if(Physics.OverlapCapsule(point0, point1, colliderDefault.radius, Layers.PLAYER_ALL, QueryTriggerInteraction.Ignore).Length == 0)
        {
            crouching = false;

            colliderDefault.enabled = true;
            colliderCrouch.enabled = false;
            cameraPosTarget = cameraPosDefault;
        }
    }

    void LateUpdate()
    {
        t_camera.localPosition = Vector3.Lerp(t_camera.localPosition, cameraPosTarget, cameraHeightAdjustSpeed*Time.deltaTime);

        Vector3 rotation;
        if(input_look.x != 0)
        {
            rotation = rb.rotation.eulerAngles;
            rotation.y += input_look.x;
            rb.rotation = Quaternion.Euler(rotation);
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
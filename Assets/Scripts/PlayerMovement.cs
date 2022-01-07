using UnityEngine;
using static PlayerInput;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    // hierarchy
    public PhysicMaterial physicsMaterial;
    public Transform t_camera;
    public CapsuleCollider colliderDefault;
    public CapsuleCollider colliderCrouch;
    public SphereCollider colliderGrounded;
    public float walkAccelDefault;
    public float walkAccelCrouch;
    public float walkAccelAir;
    public float walkMaxSpeed;
    public float crouchMaxSpeed;
    public float friction_normal;
    public float friction_slide;
    public float jumpVelocity;
    public float jumpForceForward;
    public float jumpTimeLeeway;
    public float timeBetweenJumps;
    public float groundedMagnet;
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
    bool grounded, wasGrounded;
    Vector3 groundedNormal;
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
        wasGrounded = false;
        groundedNormal = new Vector3(0, 0, 0);
        crouching = false;
        input_move = new Vector3(0, 0);
        input_look = new Vector2(0, 0);
        input_crouch = false;
        crouchKeyDown = false;
        jumpInputTime = -1000;
        timeLastJumped = -1000;
    }

    public void Reset()
    {
        var pos = rb.position;
        pos.Set(0, 40, 0);
        rb.position = pos;
        rb.velocity *= 0;
    }

    void Update()
    {
        input_move.Set(0, 0, 0);
        input_look.Set(0, 0);

        if(PauseHandler.paused) return;

        if(GetKey("walk forward"))  input_move.z ++;
        if(GetKey("walk back"))     input_move.z --;
        if(GetKey("walk left"))     input_move.x --;
        if(GetKey("walk right"))    input_move.x ++;
        input_move.Normalize();

        if(GetKeyDown("jump"))
        {
            jumpInputTime = Time.time;
        }
        if(Time.time <= jumpInputTime+jumpTimeLeeway && Time.time >= timeLastJumped+timeBetweenJumps)
        {
            input_move.y ++;
        }

        crouchKeyDown = GetKeyDown("slide") || crouchKeyDown;
        input_crouch = GetKey("slide");

        input_look.x = Input.GetAxis("Mouse X") * speed_look.x;
        input_look.y = Input.GetAxis("Mouse Y") * speed_look.y;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        // foreach(ContactPoint contact in collisionInfo.contacts)
        // {
        //     Vector3 normal = (colliderGrounded.center+rb.position - contact.point).normalized;
        //     if(normal.y > groundedNormal.y)
        //     {
        //         groundedNormal = normal;
        //     }
        // }
    }

    void FixedUpdate()
    {
        if(PauseHandler.paused) return;

        wasGrounded = grounded;

        var colliderGroundedCenter = colliderGrounded.center+rb.position;
        var colliders = Physics.OverlapSphere(colliderGroundedCenter, colliderGrounded.radius, Layers.PLAYER_ALL, QueryTriggerInteraction.Ignore);
        grounded = colliders.Length > 0;
        groundedNormal.Set(0, 0, 0);
        foreach(var collider in colliders)
        {
            if(Physics.ComputePenetration(colliderGrounded, rb.position, Quaternion.identity, collider, collider.transform.position, collider.transform.rotation, out Vector3 normal, out _))
            {
                if(normal.y > groundedNormal.y)
                {
                    groundedNormal = normal;
                }
            }
        }

        if(grounded && input_move.y > 0)
        {
            // jump

            timeLastJumped = Time.time;

            var vel = rb.velocity;
            vel.y = jumpVelocity;
            rb.velocity = vel;

            if(input_move.z > 0)
            {
                rb.AddRelativeForce(Vector3.forward*jumpForceForward);
            }
        }

        var center = colliderDefault.center+rb.position;
        var halfHeight = Vector3.up*(colliderDefault.height/2);
        var point0 = center + halfHeight;
        var point1 = center; // dont subtract half height for ignoring floor
        float threshhold = walkMaxSpeed*slideStartThreshhold;
        bool slideForward = false;
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
                slideForward = true;
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

        // accelerate
        if(input_move.x != 0 || input_move.z != 0)
        {
            var accel = Time.fixedDeltaTime * (grounded ? (crouching ? walkAccelCrouch : walkAccelDefault) : walkAccelAir);
            var maxSpeed = crouching ? crouchMaxSpeed : walkMaxSpeed;

            Vector3 f = grounded ? Vector3.Cross(rb.transform.right, groundedNormal) : rb.transform.forward;
            Vector3 r = grounded ? Vector3.Cross(groundedNormal, rb.transform.forward) : rb.transform.right;

            rb.AddForce((Mathf.Abs(Vector3.Dot(rb.velocity, r)) < maxSpeed ? input_move.x*accel : 0)*r);
            rb.AddForce(((Mathf.Abs(Vector3.Dot(rb.velocity, f)) < maxSpeed ? input_move.z*accel : 0) + (slideForward ? slideForce : 0))*f);

            // rb.AddRelativeForce(
            //     Mathf.Abs(Vector3.Dot(rb.velocity, rb.transform.right)) < walkMaxSpeed ? input_move.x*accel : 0,
            //     0,
            //     Mathf.Abs(Vector3.Dot(rb.velocity, rb.transform.forward)) < walkMaxSpeed ? input_move.z*accel : 0
            // );

            // rb.AddForce((Time.time - timeLastJumped > timeBetweenJumps ? -groundedMagnet : 0) * groundedNormal);
        }

        // friction
        physicsMaterial.staticFriction = crouching ? friction_slide : friction_normal;
        physicsMaterial.dynamicFriction = crouching ? friction_slide : friction_normal;
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
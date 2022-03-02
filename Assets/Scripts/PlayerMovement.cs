using UnityEngine;
using static PlayerInput;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    // hierarchy
    public Transform t_camera;
    public Transform t_upperBody;
    public CapsuleCollider colliderDefault;
    public CapsuleCollider colliderCrouch;
    public SphereCollider colliderGrounded;
    public float walkAccelDefault;
    public float walkAccelCrouch;
    public float walkAccelAir;
    public float walkMaxSpeed;
    public float crouchMaxSpeed;
    public float friction_normal;
    public float friction_crouch;
    public float jumpVelocity;
    public float jumpForceForward;
    public float jumpTimeLeeway;
    public float timeBetweenJumps;
    public float groundedMagnet;
    public float minGroundedNormalY;
    public float slideForce;
    public float slideStartThreshhold;
    public float slideEndThreshhold;
    public float slideMaxSpeed;
    public float slideJumpDelay;
    public float minSlideTime;
    public float cameraHeightAdjustSpeed;

    // components
    public static Rigidbody rb;

    Vector3 upperPosTarger;
    Vector3 upperPosDefault;
    Vector3 upperPosCrouch;
    float slideHeightAdjust;
    bool grounded, wasGrounded;
    Vector3 groundedNormal;
    bool sliding;
    bool crouching;
    bool crouchKeyDown;
    Vector3 input_move;
    Vector2 input_look;
    bool input_crouch;
    float jumpInputTime;
    float timeLastJumped;
    float timeLastSlid;

    public void Init()
    {
        instance = this;

        rb = GetComponent<Rigidbody>();


        slideHeightAdjust = colliderDefault.height - colliderCrouch.height;
        upperPosTarger = Vector3.up*slideHeightAdjust/2;
        upperPosDefault = t_upperBody.localPosition;
        upperPosCrouch = upperPosDefault - Vector3.up*slideHeightAdjust;

        grounded = false;
        wasGrounded = false;
        groundedNormal = new Vector3(0, 0, 0);
        crouching = false;
        sliding = false;
        input_move = new Vector3(0, 0);
        input_look = new Vector2(0, 0);
        input_crouch = false;
        crouchKeyDown = false;
        jumpInputTime = -1000;
        timeLastJumped = -1000;
        timeLastSlid = -1000;
    }

    public void Reset()
    {
        var pos = rb.position;
        pos.Set(0, 300, 0);
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

        if(GetKey("jump"))
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

    void FixedUpdate()
    {
        if(PauseHandler.paused) return;

        wasGrounded = grounded;

        var colliderGroundedCenter = colliderGrounded.center+rb.position;
        var colliders = Physics.OverlapSphere(colliderGroundedCenter, colliderGrounded.radius, Layers.PLAYER_ALL, QueryTriggerInteraction.Ignore);
        grounded = false;
        groundedNormal.Set(0, 0, 0);
        foreach(var collider in colliders)
        {
            if(Physics.ComputePenetration(colliderGrounded, rb.position, Quaternion.identity, collider, collider.transform.position, collider.transform.rotation, out Vector3 normal, out _))
            {
                if(normal.y > minGroundedNormalY && normal.y > groundedNormal.y)
                {
                    groundedNormal = normal;
                    grounded = true;
                }
            }
        }

        if(grounded && input_move.y > 0 && Time.time > timeLastSlid + slideJumpDelay)
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

        if(grounded) {
            rb.AddForce(-groundedNormal*groundedMagnet);
        }

        Vector3 f = grounded ? Vector3.Cross(rb.transform.right, groundedNormal) : rb.transform.forward;
        Vector3 r = grounded ? Vector3.Cross(groundedNormal, rb.transform.forward) : rb.transform.right;

        var center = colliderDefault.center+rb.position;
        var halfHeight = Vector3.up*(colliderDefault.height/2);
        var point0 = center + halfHeight;
        var point1 = center; // dont subtract half height for ignoring floor
        float threshhold = walkMaxSpeed*slideStartThreshhold;
        if(input_crouch)
        {
            colliderDefault.enabled = false;
            colliderCrouch.enabled = true;
            upperPosTarger = upperPosCrouch;

            float zvel = rb.RelativeVelocity().z;
            if(crouchKeyDown && Time.time - timeLastJumped > 0.01f && (!crouching || !wasGrounded) && !sliding && grounded && input_move.z > threshhold)
            {
                if(zvel < slideMaxSpeed) {
                    rb.AddForce(slideForce*f);
                    if(rb.velocity.magnitude > slideMaxSpeed) {
                        var vel = rb.velocity;
                        vel.Normalize();
                        vel *= slideMaxSpeed;
                        rb.velocity = vel;
                    }
                }

                sliding = true;
                crouchKeyDown = false;
                timeLastSlid = Time.time;
            }
            else if(sliding && zvel < slideEndThreshhold*walkMaxSpeed) {
                sliding = false;
            }

            crouching = true;
        }
        else if(Time.time - timeLastSlid > minSlideTime && Physics.OverlapCapsule(point0, point1, colliderDefault.radius, Layers.PLAYER_ALL, QueryTriggerInteraction.Ignore).Length == 0)
        {
            crouching = false;
            sliding = false;

            colliderDefault.enabled = true;
            colliderCrouch.enabled = false;
            upperPosTarger = upperPosDefault;
        }

        // accelerate
        if(!sliding && (input_move.x != 0 || input_move.z != 0))
        {
            var accel = Time.fixedDeltaTime * (grounded ? (crouching ? walkAccelCrouch : walkAccelDefault) : walkAccelAir);
            var maxSpeed = crouching ? crouchMaxSpeed : walkMaxSpeed;

            rb.AddForce((Mathf.Abs(Vector3.Dot(rb.velocity, r)) < maxSpeed ? input_move.x*accel : 0)*r);
            rb.AddForce((Mathf.Abs(Vector3.Dot(rb.velocity, f)) < maxSpeed ? input_move.z*accel : 0)*f);
        }

        // friction
        if(grounded && !sliding) {
            var vel = rb.velocity;
            float friction = crouching ? friction_crouch : friction_normal;
            vel.x *= friction;
            vel.z *= friction;
            rb.velocity = vel;
        }
    }

    void LateUpdate()
    {
        t_upperBody.localPosition = Vector3.Lerp(t_upperBody.localPosition, upperPosTarger, cameraHeightAdjustSpeed*Time.deltaTime);

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
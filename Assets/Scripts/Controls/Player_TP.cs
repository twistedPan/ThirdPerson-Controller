using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_TP : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField]
    private float speed = 12.0f;
    [SerializeField]
    private float jumpForce = 13.0f;
    [SerializeField]
    private float lowJumpGrav = 3.0f;
    [SerializeField]
    private float bigJumpGrav = 5.0f;
    [SerializeField]
    private float stepInterval = 5.0f;
    [SerializeField]
    private float runstepInterval = 0.5f;
    [SerializeField]
    private float sprintMuliplyer = 1.5f;
    [SerializeField]
    private bool isGrounded = true;
    [Space]
    public float PlayerHeight = 1.8f;
    public Vector3 BodyVelocity; // current velocity of player
    public Vector3 CamForwardDirection; // forward direction of camera

    private Rigidbody rb;    
    private PlayerController controls;
    private ThirdPersonCamera cameraSc;
    private Transform cam;
    private Vector2 movement;
    private bool isMoving;
    private bool isSprinting = false;
    private bool isHighJump;
    private bool playerIsStopped;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVeloc;
    private float stepCycle = 0.0f; 
    private float nextStep = 0.0f;
    private Vector3 origin;
    //private AudioSource audioSource;
    //private AudioClip footstep;


    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        cameraSc = cam.GetComponent<ThirdPersonCamera>();
        
        origin = transform.position;

        controls = new PlayerController();
        controls.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => movement = new Vector2(0,0);

        controls.Player.Sprint.performed += _ => Sprint();
        controls.Player.Sprint.canceled += _ => Sprint();

        controls.Player.Look.started += ctx => cameraSc.Look(ctx.ReadValue<Vector2>());
        controls.Player.Zoom.performed += ctx => cameraSc.ZoomCam(ctx.ReadValue<Vector2>());

        controls.Player.Move.performed += _ => isMoving = true; 
        controls.Player.Move.canceled += _ => isMoving = false;

        controls.Player.Jump.performed += _ => Jump(); // sJumping = true; // Jump(); // 

        origin = transform.position;


        Cursor.lockState = CursorLockMode.Locked;
    }


    void FixedUpdate()
    { 
        // Player Body Forward
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 3, Color.blue);
        // Camera/Head look direction
        Debug.DrawRay(transform.position + Vector3.up*PlayerHeight, CamForwardDirection * 3, Color.green, 0f);

        UpdatePositionAndRotation();

        BodyVelocity = rb.velocity;
    }

    // Stop player inputs
    public void TogglePlayerMovement()
    {
        if (playerIsStopped) playerIsStopped = false;
        else playerIsStopped = true;
    }


    void UpdatePositionAndRotation()
    {
        // movement input and direction
        float horizontal = movement.x;
        float vertical = movement.y;
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical);
        if (playerIsStopped) direction = Vector3.zero;

        isGrounded = GroundCheck();
        //Debug.Log("Input Direction: " + direction);


        // Jumping & Falling
        if (rb.velocity.y < 0) // -> big Jump
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (bigJumpGrav - 1) * Time.deltaTime;
        } 
        else if (rb.velocity.y > 0) // -> Fall      //  && !isJumping // check in Player_FP Script 
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpGrav - 1) * Time.deltaTime;
        }


        // Walking & Orientation
        if (direction.magnitude == 0) // -> stand still & look with cam & do not move body
        {
            // turn body along direction of cam 
            /* 
            Vector3 relativPos = cam.position - transform.position;
            relativPos.y = 0.0f;
            Quaternion rotation = Quaternion.LookRotation(-relativPos, Vector3.up);
            transform.rotation = rotation;   
            */  

            rb.velocity = new Vector3(0f, rb.velocity.y, 0f); // stops gliding 
            rb.angularVelocity = Vector3.zero; // stops spinning 

        }
        else if (direction.magnitude >= 0.1f) // -> move & free cam
        {
            // Look at along movement
            float targetAngle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVeloc, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            Vector3 moveDir = Quaternion.Euler(0f,targetAngle, 0f) * Vector3.forward;
            rb.velocity = new Vector3(moveDir.x * speed, rb.velocity.y, moveDir.z * speed);
        }

        // simulation of physical steps
        if (isGrounded && isMoving) StepCycle(speed);

        // if fall of map spawn at origin xxx testing
        if (transform.position.y <= -10) transform.position = origin;
    }


    void Jump()
    {
        if (GroundCheck())
        {
            rb.velocity = Vector3.up * jumpForce;
        }
    }


    bool GroundCheck()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, PlayerHeight+0.5f);
        return hit.collider != null;
    }


    void Sprint() 
    {
        if (isSprinting) 
        {
            speed/=sprintMuliplyer;
            isSprinting = false;
        } else 
        {
            speed*=sprintMuliplyer; 
            isSprinting = true;
        }
    }

    // simulate physical steps according to player speed and step interval
    void StepCycle(float _speed)
    {
        if (movement.magnitude > 0) 
        {
            stepCycle += (movement.magnitude + (_speed*(isSprinting ? runstepInterval : 1f))) * Time.fixedDeltaTime;
        }
        else if (stepCycle > 1)
        {
            stepCycle = 0;
            nextStep = 0;
        }

        float stepProgressAmount = (nextStep - stepCycle) / stepInterval;
        
        // if first person is enabled move cam according to steps
        cameraSc.HeadMove(stepProgressAmount, isSprinting); // FirstPerson only
        
        if ((stepCycle > nextStep) == false)
        {
            return;
        }

        nextStep = stepCycle + stepInterval;
        
        PlayFootStepSound();
    }


    void PlayFootStepSound()
    {
        // Play Footstep Audio
    }



    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}

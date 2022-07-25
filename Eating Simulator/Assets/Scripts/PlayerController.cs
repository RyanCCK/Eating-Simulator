using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveForce = 20f;
    [SerializeField] public float maxSpeed = 4f;
    [SerializeField] public float jumpForce = 200f;
    [SerializeField, Range(0f, 1f)] public float midAirDampingCoeff = 0.3f;

    private GameManager gameManager;
    private Rigidbody rb;

    private bool isDead;
    private bool isProgressing;
    private bool isGrounded;
    private bool isSpeedBoostApplied;
    private bool jumpAvailable;
    private bool jumpKeyPressed;
    private bool isJumping = false;
    private float speedBoostTimer = 0f;
    private float currentSpeedBoostAcceleration;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isDead = false;
        isProgressing = false;
        isSpeedBoostApplied = false;
    }


    private void OnEnable()
    {
        GameManager.onLevelAdvance += OnLevelProgression;
        GameManager.onDeath += OnDeath;
    }


    private void OnDisable()
    {
        GameManager.onLevelAdvance -= OnLevelProgression;
        GameManager.onDeath -= OnDeath;
    }


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }


    void Update()
    {
        if (!isDead && !isProgressing)
        {
            if (Input.GetAxis("Jump") == 1) jumpKeyPressed = true;
            else jumpKeyPressed = false;
        }
    }


    void FixedUpdate()
    {
        if (!isDead && !isProgressing)
        {
            //Player movement and jump logic
            PlayerMovement();
            if (jumpAvailable && jumpKeyPressed && !isJumping)
            {
                if (isSpeedBoostApplied) SpeedBoostJump();
                else PlayerJump();
                StartCoroutine(IsJumping());
            }
            //Speed boost logic
            if (isSpeedBoostApplied)
            {
                rb.useGravity = false;
                if (speedBoostTimer > 0)
                {
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, currentSpeedBoostAcceleration);
                    speedBoostTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    rb.useGravity = true;
                    isSpeedBoostApplied = false;
                    speedBoostTimer = 0;
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            if (!isJumping) jumpAvailable = true;
        }
        if (other.gameObject.tag == "Speed Boost")
        {
            isSpeedBoostApplied = true;
            currentSpeedBoostAcceleration = other.gameObject.GetComponent<SpeedBoost>().acceleration;
            speedBoostTimer = other.gameObject.GetComponent<SpeedBoost>().duration;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        // Player is grounded if colliding with ground 
        if (other.gameObject.tag == "Ground")
            isGrounded = true;

        if (other.gameObject.tag == "Oscillating")
            transform.parent = other.gameObject.transform;
    }


    private void OnTriggerExit(Collider other)
    {
        // Player is no longer grounded if not colliding with ground
        if (other.gameObject.tag == "Ground")
            isGrounded = false;

        if (other.gameObject.tag == "Oscillating")
            transform.parent = null;
    }


    void PlayerJump()
    {
        if (rb.velocity.y < 0)
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
        jumpAvailable = false; 
        //Debug.Log("Jump Registered");
    }


    void SpeedBoostJump()
    {
        if (rb.velocity.y < 0)
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        jumpAvailable = false;
        StartCoroutine(SpeedBoostJumpRoutine());
    }


    private IEnumerator SpeedBoostJumpRoutine()
    {
        float jumpVelocity = 5f;
        float jumpDuration = 1f;
        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        yield return new WaitForSeconds(jumpDuration);
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }


    private IEnumerator IsJumping()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.25f);
        isJumping = false;
    }
    
    
    // Handles player movement based on player input
    void PlayerMovement()
    {
        float xForce = Input.GetAxis("Horizontal") * moveForce;
        float zForce = Input.GetAxis("Vertical") * moveForce;

        //Add damping to changes in direction made while mid-air
        if (!isGrounded)
        {
            xForce *= midAirDampingCoeff;
            zForce *= midAirDampingCoeff;
        }

        //Apply horizontal movement force
        rb.AddForce(xForce, 0f, zForce, ForceMode.Force);

        //If net horizontal velocity exceeds maxSpeed, AND there is no speed boost being applied, 
        //  clamp to maxSpeed
        if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2f) + Mathf.Pow(rb.velocity.z, 2f)) > maxSpeed
            && !isSpeedBoostApplied)
            ConstrainHorizontalVelocity();
    }


    // Constrains x and z components of velocity such that net horizontal velocity equals maxSpeed.
    // Does not change movement direction.
    void ConstrainHorizontalVelocity()
    {
        Vector3 adjustedVelocity = rb.velocity;
        float yComp = rb.velocity.y;
        adjustedVelocity.y = 0f;

        adjustedVelocity = Vector3.ClampMagnitude(adjustedVelocity, maxSpeed);
        adjustedVelocity.y = yComp;

        rb.velocity = adjustedVelocity;
    }


    // Handles what happens to the player on death
    private void OnDeath()
    {
        isDead = true;
    }


    // Handles what happens to the player on level progression
    private void OnLevelProgression()
    {
        isProgressing = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}

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
    [SerializeField] public float gravity;

    private GameManager gameManager;
    private Rigidbody rb;

    private bool isDead;
    private bool isProgressing;
    private bool isGrounded;
    private bool canJump;
    private bool isJumping = false;
    private bool isSpeedBoostApplied;


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
        if (!isDead && !isProgressing && isGrounded)
        {
            if (Input.GetAxis("Jump") == 1) canJump = true;
        }
        else canJump = false;
    }


    void FixedUpdate()
    {
        if (!isDead && !isProgressing)
        {
            PlayerMovement();
            if (canJump && !isJumping)
            {
                PlayerJump();
                isJumping = true;
                StartCoroutine(IsJumping());
            }
        }

        // Apply custom gravity
        //rb.AddForce(0f, -1f * gravity, 0f, ForceMode.Acceleration);
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Speed Boost")
        {
            float speedBoostAcceleration = other.gameObject.GetComponent<SpeedBoost>().acceleration;
            float speedBoostDuration = other.gameObject.GetComponent<SpeedBoost>().duration;
            StartCoroutine(SpeedBoost(speedBoostAcceleration, speedBoostDuration));
        }
    }

    
    void OnCollisionStay(Collision other)
    {
        // Player is grounded if colliding with ground 
        if (other.gameObject.tag == "Ground")
            isGrounded = true;
    }


    // NOTE: This will NOT be called once the terrain the player is standing on is
    //       destroyed; this is because the ground tile collider will be destroyed.
    void OnCollisionExit(Collision other)
    {
        // Player is no longer grounded if not colliding with ground
        if (other.gameObject.tag == "Ground")
            isGrounded = false;
    }

     
    void PlayerJump()
    {
        rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
        //Debug.Log("Jump Registered");
        isGrounded = false;
    }


    private IEnumerator IsJumping()
    {
        yield return new WaitForSeconds(0.2f);
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


    // Handles application of speed boost from speed boost tile
    private IEnumerator SpeedBoost(float acceleration, float duration)
    {
        isSpeedBoostApplied = true;
        rb.AddForce(0f, 0f, acceleration, ForceMode.Impulse);
        yield return new WaitForSeconds(duration);
        isSpeedBoostApplied = false;
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

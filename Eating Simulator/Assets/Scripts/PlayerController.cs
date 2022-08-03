using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveForce = 50f;
    [SerializeField] public float maxSpeed = 60f;
    [SerializeField] public float jumpForce = 20f;
    [SerializeField, Range(0f, 1f)] public float midAirDampingCoeff = 0.3f;
    public bool canReceiveKnockback = true;

    public enum State
    {
        Default, 
        Dead,
        Advancing,
        PowerUp1,
        PowerUp2,
        UsingPowerUp1,
        UsingPowerUp2
    };  

    private State currentState;
    private State nextState;

    private GameManager gameManager;
    private Rigidbody rb;
    private bool isDead;
    private bool isProgressing;
    private bool isGrounded;
    private float horizontalInputAxis;
    private float verticalInputAxis;
    private float jumpInputAxis;
    private bool jumpAvailable;
    private bool isJumping = false;
    private float jumpAvailabilityDelay = 0.25f;
    private bool isSpeedBoostApplied = false;
    private Vector3 speedBoostForceVector;
    private float speedBoostMaxSpeed;
    private float speedBoostForceApplicationTimer = 0f;
    private float speedBoostTotalTimer = 0f;





    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentState = State.Default;
        isDead = false;
        isProgressing = false;
    }


    private void OnEnable()
    {
        GameManager.OnLevelAdvance += OnLevelAdvance;
        GameManager.OnDeath += OnDeath;
    }


    private void OnDisable()
    {
        GameManager.OnLevelAdvance -= OnLevelAdvance;
        GameManager.OnDeath -= OnDeath;
    }

    
    private void Start()
    {
        gameManager = GameManager.Instance;
    }





    void Update()
    {
        if (!isDead && !isProgressing)
        {
            horizontalInputAxis = Input.GetAxis("Horizontal");
            verticalInputAxis = Input.GetAxis("Vertical");
            jumpInputAxis = Input.GetAxis("Jump");
        }
    }


    void FixedUpdate()
    {
        if (!isDead && !isProgressing)
        {
            // Handle horizontal player movement
            HorizontalMovement();

            // Handle player jump
            if (jumpInputAxis > 0 && jumpAvailable)
            {
                Jump();
                jumpAvailable = false;
                StartCoroutine(IsJumping());
            }

            // Handle speed boost
            if (isSpeedBoostApplied)
            {
                MaintainSpeedBoost();
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
            ApplySpeedBoost(other.gameObject);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground")
            isGrounded = true;

        if (other.gameObject.tag == "Moving Platform")
            transform.parent = other.gameObject.transform;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground")
            isGrounded = false;

        if (other.gameObject.tag == "Moving Platform")
            transform.parent = null;
    }





    // Applies the default horizontal player movement.
    private void HorizontalMovement()
    {
        float xForce = horizontalInputAxis * moveForce;
        float zForce = verticalInputAxis * moveForce;

        // Add damping to changes in direction made while mid-air
        if (!isGrounded)
        {
            xForce *= midAirDampingCoeff;
            zForce *= midAirDampingCoeff;
        }

        // Apply horizontal movement force
        rb.AddForce(xForce, 0f, zForce, ForceMode.Force);

        // Clamp horizontal speed to maxSpeed if no speed boost is being applied.
        if (!isSpeedBoostApplied)
        {
            if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2f) + Mathf.Pow(rb.velocity.z, 2f)) > maxSpeed)
                ConstrainHorizontalVelocity();
        }
    }


    // Applies the default player jump.
    private void Jump()
    {
        if (rb.velocity.y < 0)
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
    }


    // Makes jump unavailable for jumpAvailabilityDelay seconds after jump is used.
    private IEnumerator IsJumping()
    {
        isJumping = true;
        yield return new WaitForSeconds(jumpAvailabilityDelay);
        isJumping = false;
    }
    
    
    // Calculates relevant values for application of speed boost, and applies initial impulse.
    private void ApplySpeedBoost(GameObject speedBoost)
    {
        isSpeedBoostApplied = true;
        // Set speed boost timer durations
        speedBoostTotalTimer = speedBoost.GetComponent<SpeedBoost>().totalDuration;
        speedBoostForceApplicationTimer = speedBoost.GetComponent<SpeedBoost>().forceDuration;
        // Set maxSpeed
        speedBoostMaxSpeed = speedBoost.GetComponent<SpeedBoost>().maxSpeed;
        // Calculate force vector to be applied throughout speedBoostForceApplicationTimer duration
        speedBoostForceVector = Vector3.Normalize((speedBoost.GetComponent<SpeedBoost>().forceDirection.normalized 
                                                 * speedBoost.GetComponent<SpeedBoost>().forceDirectionPower)
                                                 + rb.velocity.normalized)
                              * speedBoost.GetComponent<SpeedBoost>().continuouslyAppliedForce;
        // Enforce axis restrictions on application of force
        if (speedBoost.GetComponent<SpeedBoost>().restrictPosX)
        {
            if (speedBoostForceVector.x > 0) speedBoostForceVector.x = 0;
        }
        if (speedBoost.GetComponent<SpeedBoost>().restrictNegX)
        {
            if (speedBoostForceVector.x < 0) speedBoostForceVector.x = 0;
        }
        if (speedBoost.GetComponent<SpeedBoost>().restrictPosY)
        {
            if (speedBoostForceVector.y > 0) speedBoostForceVector.y = 0;
        }
        if (speedBoost.GetComponent<SpeedBoost>().restrictNegY)
        {
            if (speedBoostForceVector.y < 0) speedBoostForceVector.y = 0;
        }
        if (speedBoost.GetComponent<SpeedBoost>().restrictPosZ)
        {
            if (speedBoostForceVector.z > 0) speedBoostForceVector.z = 0;
        }
        if (speedBoost.GetComponent<SpeedBoost>().restrictNegZ)
        {
            if (speedBoostForceVector.z < 0) speedBoostForceVector.z = 0;
        }
        // Apply initial impulse 
        rb.AddForce(speedBoostForceVector.normalized * speedBoost.GetComponent<SpeedBoost>().impulseForce, ForceMode.Impulse);
    }


    // Applies continuous force to player based on most recent speed boost values. 
    // Also manages speed boost timers and ends speed boost.
    private void MaintainSpeedBoost()
    {
        if (speedBoostTotalTimer > 0)
        {
            if (speedBoostForceApplicationTimer > 0)
            {
                rb.AddForce(speedBoostForceVector, ForceMode.Force);
            }
            // Clamp magnitude of velocity if necessary
            if (rb.velocity.sqrMagnitude > Mathf.Pow(speedBoostMaxSpeed,2))
            {
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, speedBoostMaxSpeed);
            }
            speedBoostTotalTimer -= Time.fixedDeltaTime;
            speedBoostForceApplicationTimer -= Time.fixedDeltaTime;
        }
        else
        {
            isSpeedBoostApplied = false;
            speedBoostTotalTimer = 0;
            speedBoostForceApplicationTimer = 0;
        }
    }


    // Clamps horizontal velocity to maxSpeed, if no speed-related buffs are being applied.
    private void ConstrainHorizontalVelocity()
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


    // Handles what happens to the player on level advancement
    private void OnLevelAdvance()
    {
        isProgressing = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }





    // Returns the player's current state.
    public State GetState()
    {
        return currentState;
    }


    // Changes the player's state to a given new state, if valid. Otherwise resets player to default state.
    private void ChangeState(State nextState = State.Default)
    {
        //Based on current state and new state, call appropriate methods, and set currentState = newState.
    }
}

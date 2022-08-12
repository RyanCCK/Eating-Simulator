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
    private Quaternion initialRotation;
    private Quaternion targetRotation = Quaternion.identity;
    private float rotationSpeed;
    private float rotationTimeCount = 0f;
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
        switch (currentState)
        {
            case State.Default:
            {
                // Read player input
                horizontalInputAxis = Input.GetAxis("Horizontal");
                verticalInputAxis = Input.GetAxis("Vertical");
                jumpInputAxis = Input.GetAxis("Jump");

                // If there is a power-up waiting in nextState, use it
                if (nextState == State.PowerUp1 || nextState == State.PowerUp2)
                    ChangeState(nextState);

                break;
            }
        }
    }


    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Default:
            {
                // Rotate player if necessary
                if (targetRotation != transform.rotation)
                {
                    transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, rotationTimeCount * rotationSpeed * 0.1f);
                    rotationTimeCount += Time.fixedDeltaTime;
                }

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

                break;
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
        if (other.gameObject.tag == "Power Up")
        {
            // Set nextState to the correct PowerUp state
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground")
            isGrounded = true;

        if (other.gameObject.tag == "Moving Platform")
            transform.parent = other.gameObject.transform;

        if (other.gameObject.tag == "Player Rotation Zone")
        {
            initialRotation = transform.rotation;
            targetRotation = other.GetComponent<PlayerRotationZone>().rotation;
            rotationSpeed = other.GetComponent<PlayerRotationZone>().rotationSpeed;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground")
            isGrounded = false;

        if (other.gameObject.tag == "Moving Platform")
            transform.parent = null;

        if (other.gameObject.tag == "Player Rotation Zone")
        {
            initialRotation = transform.rotation;
        }
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
        rb.AddRelativeForce(xForce, 0f, zForce, ForceMode.Force);

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
        // Calculate force direction vector
        speedBoostForceVector = Vector3.Normalize((speedBoost.GetComponent<SpeedBoost>().forceDirection.normalized
                                                 * speedBoost.GetComponent<SpeedBoost>().forceDirectionPower)
                                                 + rb.velocity.normalized);
        // If there is a continuously applied force, multiply it by force direction vector to get correct magnitude.
        if (speedBoost.GetComponent<SpeedBoost>().continuouslyAppliedForce != 0)
        {
            speedBoostForceVector *= speedBoost.GetComponent<SpeedBoost>().continuouslyAppliedForce;
        }
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


    // Clamps horizontal velocity to maxSpeed 
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
        ChangeState(State.Dead);
        StopAllCoroutines();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }


    // Handles what happens to the player on level advancement
    private void OnLevelAdvance()
    {
        ChangeState(State.Advancing);
        StopAllCoroutines();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }





    // Returns the player's current state.
    public State GetState()
    {
        return currentState;
    }


    // Changes the player's state to a given new state, if valid. 
    // Will not change the player's state if the given nextState is invalid.
    // If no nextState is given, will set player to Default state.

    /* LIST OF VALID STATE TRANSITIONS:    
     *  
     * Default -> {Default, Dead, Advancing, PowerUp1, PowerUp2}
     * Dead -> {Dead}
     * Advancing -> {Advancing}
     * PowerUp1 -> {Default, Dead, Advancing, UsingPowerUp1}
     * PowerUp2 -> {Default, Dead, Advancing, UsingPowerUp2}
     * UsingPowerUp1 -> {Default, Dead, Advancing}
     * UsingPowerUp2 -> {Default, Dead, Advancing}
     * 
     */

    private void ChangeState(State nextState = State.Default)
    {
        switch (currentState)
        {
            case State.Default:
            {
                switch (nextState)
                {
                    case State.Default:
                    {
                        break;
                    }

                    case State.Dead:
                    {
                        currentState = State.Dead;
                        nextState = State.Default;
                        break;
                    }

                    case State.Advancing:
                    {
                        currentState = State.Advancing;
                        nextState = State.Default;
                        break;
                    }

                    default:
                        Debug.LogError("Invalid next state");
                        break;
                }
                break;
            }

            case State.Dead:
            {
                switch (nextState)
                {
                    case State.Dead:
                    {
                        break;
                    }

                    default:
                        Debug.LogError("Invalid next state");
                        break;
                }
                break;
            }

            case State.Advancing:
            {
                switch (nextState)
                {
                    case State.Advancing:
                    {
                        break;
                    }

                    default:
                        Debug.LogError("Invalid next state");
                        break;
                }
                break;
            }
        }
    }
}

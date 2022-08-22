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
    public float rocketBoostDuration = 3f;
    public float rocketBoostSpeed = 100f;
    public bool canReceiveKnockback = true;
    [Tooltip("Index 0 is for Rocket Boost, 1 for Constructor, 2 for Detonator, 3 for Bouncy Ball, 4 for Seagull Morph")]
    [SerializeField] public Material[] powerUpMaterials;
    [Tooltip("Index 0 is for Rocket Boost, 1 for Constructor, 2 for Detonator, 3 for Bouncy Ball")]
    [SerializeField] public GameObject[] powerUpParticles;

    public enum State
    {
        Default, 
        Dead,
        Advancing,
        RocketBoostEquipped,
        UsingRocketBoost,
        ConstructorEquipped, 
        UsingConstructor,
        DetonatorEquipped,
        UsingDetonator,
        BouncyBallEquipped,
        UsingBouncyBall,
        SeagullMorphEquipped,
        UsingSeagullMorph
    };  

    private State currentState;
    private State nextState;

    private GameManager gameManager;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private Material defaultMaterial;
    // May need to change this to an array of effects, 
    //  if multiple effects need to be destroyed on a state change.
    private GameObject currentPowerUpParticles;
    private Quaternion initialRotation;
    private Quaternion targetRotation = Quaternion.identity;
    private float rotationSpeed;
    private float rotationTimeCount = 0f;
    private bool isGrounded;
    private float horizontalInputAxis;
    private float verticalInputAxis;
    private float jumpInputAxis;
    private bool jumpAvailable = true;
    private bool isJumping = false;
    private float jumpAvailabilityDelay = 0.25f;
    private bool stateChangeAvailable = true;
    private float stateChangeDelay = 0.1f;
    private bool isSpeedBoostApplied = false;
    private Vector3 speedBoostForceVector;
    private float speedBoostMaxSpeed;
    private float speedBoostForceApplicationTimer = 0f;
    private float speedBoostTotalTimer = 0f;

    public delegate void RocketBoostAction();
    public delegate void DefaultStateAction();

    public static event RocketBoostAction OnRocketBoostPowerUp;
    public static event DefaultStateAction OnDefaultState;





    private void Awake()
    {
        currentState = State.Default;
        nextState = State.Default;
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        defaultMaterial = meshRenderer.material;
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
        // Read player input
        horizontalInputAxis = Input.GetAxis("Horizontal");
        verticalInputAxis = Input.GetAxis("Vertical");
        jumpInputAxis = Input.GetAxis("Jump");

        // Check if power up is toggled/used
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && stateChangeAvailable)
        {
            ChangeState();
        }

        // Check if a stored power up needs to be applied (do not wait for stateChangeAvailable)
        if (currentState == State.Default && nextState != State.Default)
        {
            ChangeState(nextState);
            nextState = State.Default;
        }
    }


    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Default:
            case State.RocketBoostEquipped:
            case State.ConstructorEquipped:
            case State.DetonatorEquipped:
            case State.BouncyBallEquipped:
            case State.SeagullMorphEquipped:
            case State.UsingConstructor:
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

            case State.UsingRocketBoost:
            {
                // Handle powered-up rocket boost player movement
                RocketBoostMovement();
                break;
            }

            case State.UsingDetonator:
            {
                // Freeze player movement temporarily, while shaking player around as if "charging up"
                break;
            }

            case State.UsingBouncyBall:
            {
                // Enable double-jump, cause player to bounce against surfaces,
                //      apply custom increased gravity, increase jump power
                break;
            }

            case State.UsingSeagullMorph:
            {
                // Enable free movement and rotation, decrease movement speed while in contact with any surface,
                //      enable unlimited upward flight, enable slow fall, disable player-received knockback,
                //      do not respond to player roation zones
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
            if (currentState != State.Default)
            {
                nextState = other.GetComponent<PowerUp>().induceState;
            }
            else
            {
                ChangeState(other.GetComponent<PowerUp>().induceState);
            }
        }

        if (other.gameObject.tag == "Player Rotation Zone")
        {
            rotationTimeCount = 0;
            initialRotation = transform.rotation;
            targetRotation = other.GetComponent<PlayerRotationZone>().rotation;
            rotationSpeed = other.GetComponent<PlayerRotationZone>().rotationSpeed;
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

    
    // Applies player movement during rocket boost power-up 
    private void RocketBoostMovement()
    {
        Vector3 directionVector;
        directionVector.x = horizontalInputAxis * 0.25f;
        directionVector.y = jumpInputAxis * 0.25f;
        if (verticalInputAxis != 0) directionVector.y = verticalInputAxis * 0.25f;
        directionVector.z = 1;
        rb.velocity = directionVector * rocketBoostSpeed;
    }





    // Timer to end the rocket boost power-up
    // (Note that if another rocket boost is equipped and used within (duration) seconds 
    // after the first one is activated, then the second one will be ended by this routine with this implementation)
    // POSSIBLE FIX: every time a rocket boost is activated, first stop all instances of RocketBoostRoutine, then start a new one
    // NEED TO TEXT FIX ABOVE (IS IMPLEMENTED)
    private IEnumerator RocketBoostTimer()
    {
        yield return new WaitForSeconds(rocketBoostDuration);
        if (currentState == State.UsingRocketBoost)
            ChangeState();
    }





    // Enforces brief delay between state changes, so that the player cannot accidentally skip past a desired state.
    private IEnumerator IsChangingState()
    {
        stateChangeAvailable = false;
        yield return new WaitForSeconds(stateChangeDelay);
        stateChangeAvailable = true;
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

    // TODO:
    // Build in brief delay to prevent switching between states too quickly 
    private void ChangeState(State newState = State.Default)
    {
        StartCoroutine(IsChangingState());

        if (newState == State.Dead)
        {
            currentState = State.Dead;
            return;
        }
        else if (newState == State.Advancing)
        {
            currentState = State.Advancing;
            return;
        }

        switch (currentState)
        {
            case State.Default:
            {
                switch (newState)
                {
                    case State.RocketBoostEquipped:
                    {
                        currentState = State.RocketBoostEquipped;
                        meshRenderer.material = powerUpMaterials[0];
                        break;
                    }

                    case State.ConstructorEquipped:
                    {
                        currentState = State.ConstructorEquipped;
                        meshRenderer.material = powerUpMaterials[1];
                        break;
                    }

                    case State.DetonatorEquipped:
                    {
                        currentState = State.DetonatorEquipped;
                        meshRenderer.material = powerUpMaterials[2];
                        break;
                    }

                    case State.BouncyBallEquipped:
                    {
                        currentState = State.BouncyBallEquipped;
                        meshRenderer.material = powerUpMaterials[3];
                        break;
                    }

                    case State.SeagullMorphEquipped:
                    {
                        currentState = State.SeagullMorphEquipped;
                        // Also add wings to player 
                        meshRenderer.material = powerUpMaterials[4];
                        break;
                    }
                }
                break;
            }

            case State.RocketBoostEquipped:
            {
                currentState = State.UsingRocketBoost;
                OnRocketBoostPowerUp(); //Event
                currentPowerUpParticles = Instantiate(powerUpParticles[0], transform);
                StopCoroutine(RocketBoostTimer());
                StartCoroutine(RocketBoostTimer());
                break;
            }

            case State.UsingRocketBoost:
            {
                currentState = State.Default;
                OnDefaultState();   //Event
                meshRenderer.material = defaultMaterial;
                jumpAvailable = true;
                Destroy(currentPowerUpParticles);
                break;
            }

            case State.ConstructorEquipped:
            {
                currentState = State.UsingConstructor;
                break;
            }

            case State.UsingConstructor:
            {
                currentState = State.Default;
                OnDefaultState();   //Event
                meshRenderer.material = defaultMaterial;
                jumpAvailable = true;
                break;
            }

            case State.DetonatorEquipped:
            {
                currentState = State.UsingDetonator;
                break;
            }

            case State.UsingDetonator:
            {
                currentState = State.Default;
                OnDefaultState();   //Event
                meshRenderer.material = defaultMaterial;
                jumpAvailable = true;
                break;
            }

            case State.BouncyBallEquipped:
            {
                currentState = State.UsingBouncyBall;
                break;
            }

            case State.UsingBouncyBall:
            {
                currentState = State.Default;
                OnDefaultState();   //Event
                meshRenderer.material = defaultMaterial;
                jumpAvailable = true;
                break;
            }

            case State.SeagullMorphEquipped:
            {
                currentState = State.UsingSeagullMorph;
                break;
            }

            case State.UsingSeagullMorph:
            {
                currentState = State.Default;
                OnDefaultState();   //Event
                meshRenderer.material = defaultMaterial;
                jumpAvailable = true;
                break;
            }
        }
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
}


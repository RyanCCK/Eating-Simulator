using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to control the player character

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 20f;
    [SerializeField] public float maxSpeed = 4f;
    [SerializeField] public float jumpForce = 200f;
    [SerializeField, Range(0f, 1f)] public float midAirDampingCoeff = 0.3f;
    public bool isGrounded;
    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        
    }


    void FixedUpdate()
    {
        PlayerMovement();
    }


    void OnCollisionStay(Collision other)
    {
        // Player is grounded if colliding with ground 
        if (other.gameObject.tag == "Ground")
            isGrounded = true;
    }


    void OnCollisionExit(Collision other)
    {
        // Player is no longer grounded if not colliding with ground
        if(other.gameObject.tag == "Ground")
            isGrounded = false;
    }


    /////////////TODO/////////////TODO/////////////TOOD/////////////
    /*
        TODO: Fix:
                    - Loss of Velocity on Landing
                    [FIXED]     - Catching on corners
                    [FIXED]     - Inconsistent Jump Height
                    [FIXED]     - Sticking to Corners & Walls
                    [FIXED]     - Infinite Wall Jump
    */
    /////////////TODO/////////////TODO/////////////TOOD/////////////


    // Handles player movement based on player input
    void PlayerMovement()
    {
        float xForce = Input.GetAxis("Horizontal") * moveSpeed;
        float zForce = Input.GetAxis("Vertical") * moveSpeed;
        float yForce = isGrounded ? Input.GetAxis("Jump") * jumpForce: 0f;

        //Add damping to changes in direction made while mid-air
        if (!isGrounded)
        {
            xForce *= midAirDampingCoeff;
            zForce *= midAirDampingCoeff;
        }

        //Apply movement force
        rb.AddForce(xForce, yForce, zForce, ForceMode.Force);

        //If net horizontal velocity exceeds maxSpeed, set it to maxSpeed
        if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2f) + Mathf.Pow(rb.velocity.z, 2f)) > maxSpeed)
            ConstrainHorizontalVelocity();
    }


    // Reduces x and z components of velocity such that net horizontal velocity equals maxSpeed.
    // Does not change movement direction.
    void ConstrainHorizontalVelocity()
    {
        Vector3 adjustedVelocity = rb.velocity;
        adjustedVelocity.y = 0f;
        float yComp = rb.velocity.y;

        adjustedVelocity = Vector3.ClampMagnitude(adjustedVelocity, maxSpeed);
        adjustedVelocity.y = yComp;

        rb.velocity = adjustedVelocity;
    }
}

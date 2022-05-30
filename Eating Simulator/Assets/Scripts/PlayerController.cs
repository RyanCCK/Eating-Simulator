using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to control the player character

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float maxSpeed = 8f;
    [SerializeField] public float jumpForce = 3.5f;
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
        PlayerMovement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }

    void FixedUpdate()
    {
        
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
    //TODO: Fix slow fall, sticking to walls, and instant vertical position change when jumping
    /////////////TODO/////////////TODO/////////////TOOD/////////////

    // Handles player movement based on player input
    void PlayerMovement()
    {
        float xForce = Input.GetAxis("Horizontal") * moveSpeed;
        float zForce = Input.GetAxis("Vertical") * moveSpeed;
        float yForce = isGrounded ? Input.GetAxis("Jump") * jumpForce: 0;

        //Apply movement force
        rb.AddForce(xForce, yForce, zForce, ForceMode.Force);

        // If not grounded, restrict lateral movement
        if (!isGrounded)
        {
            //rb.AddForce(0f, -2f, 0f);
        }

        //Do not exceed max speed
        if (Mathf.Abs(rb.velocity.x) > maxSpeed || Mathf.Abs(rb.velocity.z) > maxSpeed)
        {
            if (Mathf.Abs(rb.velocity.x) > maxSpeed && rb.velocity.x > 0)
                rb.velocity = new Vector3(maxSpeed, rb.velocity.y, rb.velocity.z);
            else if (Mathf.Abs(rb.velocity.x) > maxSpeed && rb.velocity.x < 0)
                rb.velocity = new Vector3(-maxSpeed, rb.velocity.y, rb.velocity.z);

            if (Mathf.Abs(rb.velocity.z) > maxSpeed && rb.velocity.z > 0)
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxSpeed);
            else if (Mathf.Abs(rb.velocity.z) > maxSpeed && rb.velocity.z < 0)
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -maxSpeed);
        }
    }
}

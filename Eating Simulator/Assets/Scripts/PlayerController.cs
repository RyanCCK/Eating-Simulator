using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to control the player character

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    // Global variables BAD?
    [SerializeField] public float playerSpeed = 5f;
    [SerializeField] public float jumpForce = 3.5f;
    public Vector3 jump;
    public bool isGrounded;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0f, 2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    void OnCollisionExit()
    {
        isGrounded = false;
    }

    // Handles lateral movement using transform.translate based on player input 
    void PlayerMovement()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;
        transform.Translate(x, 0, z);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
}

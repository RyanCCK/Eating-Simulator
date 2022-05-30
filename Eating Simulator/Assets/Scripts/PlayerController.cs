using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to control the player character

public class PlayerController : MonoBehaviour
{
    // Global variable BAD?
    [SerializeField] float playerSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    // Handles movement based on player input
    void PlayerMovement()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;

        transform.Translate(x, 0, z);
    }
}

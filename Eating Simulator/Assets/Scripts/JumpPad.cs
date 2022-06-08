using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] public float jumpForce = 20f;

    GameManager gameManager;
    Rigidbody rb;


    private void Start()
    {
        gameManager = GameManager.Instance;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb = collision.rigidbody;
            rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
        }
    }
}

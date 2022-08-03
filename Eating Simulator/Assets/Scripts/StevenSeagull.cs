using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class StevenSeagull : MonoBehaviour
{
    [SerializeField] Vector3[] points;
    [SerializeField] float speed;
    [SerializeField] float collisionKnockback;
    public bool movementEnabled = false;

    private Vector3 currentPos;
    private Vector3 newPos;
    private int index = 0;
    private float step;


    private void Awake()
    {
        if(movementEnabled)
        {
            currentPos = transform.position;
            if (currentPos == points[index]) ++index;
            newPos = points[index];
        }
    }

    
    void Update()
    {
        if(movementEnabled)
        {
            step = speed * Time.deltaTime;
            if (currentPos == newPos)
            {
                ++index;
                if (index >= points.Length)
                    index = 0;
                newPos = points[index];
            }
            currentPos = Vector3.MoveTowards(currentPos, newPos, step);
            transform.position = currentPos;
        }
    }

    
    /*
     * POSSIBLE TO-DO:
     * Build functionality for player to deal knockback to seagull, under certain conditions
     */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerController>().canReceiveKnockback)
            collision.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(collision.contacts[0].normal * collisionKnockback, 
                                                                              collision.gameObject.transform.position, ForceMode.Impulse);
    }


    /*
     * TO-DO:
     * Add death method, to kill seagull and play an animation or particle effect when this happens.
     */ 
}

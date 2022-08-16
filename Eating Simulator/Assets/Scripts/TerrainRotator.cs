using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRotator : MonoBehaviour
{
    [SerializeField] public Vector3 xAxis = Vector3.right;
    [SerializeField] public Vector3 yAxis = Vector3.up;
    [SerializeField] public Vector3 zAxis = Vector3.forward;
    [SerializeField] public float xSpeed = 0f;
    [SerializeField] public float ySpeed = 0f;
    [SerializeField] public float zSpeed = 0f;
    [SerializeField] public bool waitForPlayerContact = false;
    [SerializeField] public float waitTime = 0f;
    [SerializeField] public bool useCustomCenter = false;
    [SerializeField] public Vector3 customCenter;

    private Vector3 center;
    private bool waiting = false;
    private bool playerContact = true;


    private void Awake()
    {
        // If no custom center is being used,
        // calculate actual center of gameobject from average position of all children
        if (!useCustomCenter)
        {
            Component[] childTransforms = GetComponentsInChildren<Transform>();
            foreach (Transform child in childTransforms)
            {
                center += child.position;
            }
            center /= childTransforms.Length;
        }
        else center = customCenter;

        // Wait for player contact if necessary
        if (waitForPlayerContact) playerContact = false;
    }


    private void Start()
    {
        if (waitTime > 0)
        {
            waiting = true;
            StartCoroutine(Waiting());
        }
    }

    
    void FixedUpdate()
    {
        if (!waiting && playerContact)
        {
            //x rotation
            gameObject.transform.RotateAround(center, xAxis, xSpeed * Time.fixedDeltaTime);
            //y rotation
            gameObject.transform.RotateAround(center, yAxis, ySpeed * Time.fixedDeltaTime);
            //z rotation
            gameObject.transform.RotateAround(center, zAxis, zSpeed * Time.fixedDeltaTime);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            playerContact = true;
    }


    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(waitTime);
        waiting = false;
    }
}

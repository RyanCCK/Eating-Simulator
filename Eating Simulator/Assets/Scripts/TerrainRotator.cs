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

    private Vector3 center;
    private bool waiting = false;
    private bool playerContact = true;


    private void Awake()
    {
        // Calculate actual center of gameobject from average position of all children
        Component[] childTransforms = GetComponentsInChildren<Transform>();
        foreach(Transform child in childTransforms)
        {
            center += child.position;
        }
        center /= childTransforms.Length;

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

    
    void Update()
    {
        if (!waiting && playerContact)
        {
            //x rotation
            gameObject.transform.RotateAround(center, xAxis, xSpeed * Time.deltaTime);
            //y rotation
            gameObject.transform.RotateAround(center, yAxis, ySpeed * Time.deltaTime);
            //z rotation
            gameObject.transform.RotateAround(center, zAxis, zSpeed * Time.deltaTime);
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

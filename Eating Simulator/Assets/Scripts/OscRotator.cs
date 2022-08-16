using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscRotator : MonoBehaviour
{
    [SerializeField] public float xRotation;
    [SerializeField] public float xSpeed;
    [SerializeField] public float yRotation;
    [SerializeField] public float ySpeed;
    [SerializeField] public float zRotation;
    [SerializeField] public float zSpeed;
    [SerializeField] public bool waitForPlayerContact = false;
    [SerializeField] public float waitTime = 0f;

    private Vector3 center;
    private float timeCount = 0f;
    private float xRot = 0f;
    private float yRot = 0f;
    private float zRot = 0f;
    private bool waiting = false;
    private bool playerContact = true;
    


    private void Awake()
    {
        // Calculate actual center of gameobject from average position of all children
        Component[] childTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform child in childTransforms)
        {
            center += child.position;
        }
        center /= childTransforms.Length;

        // Wait for player contact if necessary
        if (waitForPlayerContact) playerContact = false;
    }


    private void Start()
    {
        timeCount = 0f;
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
            xRot = (Mathf.Sin((timeCount + Time.fixedDeltaTime) * xSpeed) - Mathf.Sin((timeCount) * xSpeed)) * xRotation;
            yRot = (Mathf.Sin((timeCount + Time.fixedDeltaTime) * ySpeed) - Mathf.Sin((timeCount) * ySpeed)) * yRotation;
            zRot = (Mathf.Sin((timeCount + Time.fixedDeltaTime) * zSpeed) - Mathf.Sin((timeCount) * zSpeed)) * zRotation;

            //x rotation
            if (xRotation != 0)
                gameObject.transform.RotateAround(center, Vector3.right, xRot);
            //y rotation
            if (yRotation != 0)
                gameObject.transform.RotateAround(center, Vector3.up, yRot);
            //z rotation
            if (zRotation != 0)
                gameObject.transform.RotateAround(center, Vector3.forward, zRot);

            timeCount += Time.fixedDeltaTime;
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

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
    [SerializeField] public float waitTime = 0f;
    [SerializeField] public bool waitForPlayerContact = false;

    private Vector3 center;
    private float xRotThisFrame;
    private float yRotThisFrame;
    private float zRotThisFrame;
    private float currentXRot = 0f;
    private float currentYRot = 0f;
    private float currentZRot = 0f;
    private float xDir = 1f;
    private float yDir = 1f;
    private float zDir = 1f;
    private bool waiting = true;
    private bool playerContact = false;


    private void Awake()
    {
        Component[] childTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform child in childTransforms)
        {
            center += child.position;
        }
        center /= childTransforms.Length;
    }


    private void Start()
    {
        StartCoroutine(Waiting());
    }


    // Update is called once per frame
    void Update()
    {
        if (!waiting && (!waitForPlayerContact || (waitForPlayerContact && playerContact)))
        {
            /*
            xRotThisFrame = (Mathf.Sin(Time.time * xSpeed) * xRotation) - (Mathf.Sin((Time.time - Time.deltaTime) * xSpeed) * xRotation);
            yRotThisFrame = (Mathf.Sin(Time.time * ySpeed) * yRotation) - (Mathf.Sin((Time.time - Time.deltaTime) * ySpeed) * yRotation);
            zRotThisFrame = (Mathf.Sin(Time.time * zSpeed) * zRotation) - (Mathf.Sin((Time.time - Time.deltaTime) * zSpeed) * zRotation);
            */

            if (currentXRot > xRotation / 2.0)
                xDir = -1;
            else if (currentXRot < -1 * (xRotation / 2.0))
                xDir = 1;
            if (currentYRot > yRotation / 2.0)
                yDir = -1;
            else if (currentYRot < -1 * (yRotation / 2.0))
                yDir = 1;
            if (currentZRot > zRotation / 2.0)
                zDir = -1;
            else if (currentZRot < -1 * (zRotation / 2.0))
                zDir = 1;

            xRotThisFrame = ((xRotation / (1/xSpeed)) * Time.deltaTime) * xDir;
            yRotThisFrame = ((yRotation / (1/ySpeed)) * Time.deltaTime) * yDir;
            zRotThisFrame = ((zRotation / (1/zSpeed)) * Time.deltaTime) * zDir;

            currentXRot += xRotThisFrame;
            currentYRot += yRotThisFrame;
            currentZRot += zRotThisFrame;

            //x rotation
            if(xRotation != 0)
                gameObject.transform.RotateAround(center, Vector3.right, xRotThisFrame);
            //y rotation
            if(yRotation != 0)
                gameObject.transform.RotateAround(center, Vector3.up, yRotThisFrame);
            //z rotation
            if(zRotation != 0)
                gameObject.transform.RotateAround(center, Vector3.forward, zRotThisFrame);
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

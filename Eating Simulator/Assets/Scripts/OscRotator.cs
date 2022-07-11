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

    private Vector3 center;
    private float xRotThisFrame;
    private float yRotThisFrame;
    private float zRotThisFrame;
    private bool waiting = true;


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
        if(!waiting)
        {
            xRotThisFrame = (Mathf.Sin(Time.time * xSpeed) * xRotation) - (Mathf.Sin((Time.time - Time.deltaTime) * xSpeed) * xRotation);
            yRotThisFrame = (Mathf.Sin(Time.time * ySpeed) * yRotation) - (Mathf.Sin((Time.time - Time.deltaTime) * ySpeed) * yRotation);
            zRotThisFrame = (Mathf.Sin(Time.time * zSpeed) * zRotation) - (Mathf.Sin((Time.time - Time.deltaTime) * zSpeed) * zRotation);

            //x rotation
            gameObject.transform.RotateAround(center, Vector3.right, xRotThisFrame);
            //y rotation
            gameObject.transform.RotateAround(center, Vector3.up, yRotThisFrame);
            //z rotation
            gameObject.transform.RotateAround(center, Vector3.forward, zRotThisFrame);
        }
    }


    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(waitTime);
        waiting = false;
    }
}

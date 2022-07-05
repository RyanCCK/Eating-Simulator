using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRotator : MonoBehaviour
{
    [SerializeField] public float xSpeed = 0f;
    [SerializeField] public float ySpeed = 0f;
    [SerializeField] public float zSpeed = 0f;
    private Vector3 center;


    private void Awake()
    {
        //compute actual midpoint of parent object from all child object positions
        foreach(Transform child in transform)
        {
            center += child.position;
        }
        center /= transform.childCount;
    }


    // Update is called once per frame
    void Update()
    {
        //x rotation
        gameObject.transform.RotateAround(center, Vector3.right, xSpeed * Time.deltaTime);
        //y rotation
        gameObject.transform.RotateAround(center, Vector3.up, ySpeed * Time.deltaTime);
        //z rotation
        gameObject.transform.RotateAround(center, Vector3.forward, zSpeed * Time.deltaTime);
    }
}

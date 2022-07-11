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
        Component[] childTransforms = GetComponentsInChildren<Transform>();
        foreach(Transform child in childTransforms)
        {
            center += child.position;
        }
        center /= childTransforms.Length;
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

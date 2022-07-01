using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRotator : MonoBehaviour
{
    [SerializeField] public float xSpeed = 0f;
    [SerializeField] public float ySpeed = 0f;
    [SerializeField] public float zSpeed = 0f;
    private Vector3 rotationVector;
    

    // Update is called once per frame
    void Update()
    {
        rotationVector.x = xSpeed;
        rotationVector.y = ySpeed;
        rotationVector.z = zSpeed;
        transform.Rotate(rotationVector * Time.deltaTime, Space.Self);
    }
}

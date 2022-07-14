using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingAnimation : MonoBehaviour
{
    [SerializeField] Vector3 hinge;
    [SerializeField] public float maxAngle;
    [SerializeField] public float degreesPerSecond;
    [SerializeField] public float rotationDirection = 1f;

    private float currentAngle = 0.1f;
    private float rotationThisFrame;


    private void Awake()
    {
        hinge = transform.position;
        hinge.y += 0.5f;
    }


    // Update is called once per frame
    void Update()
    {
        if (currentAngle >= maxAngle || currentAngle <= 0)
            rotationDirection *= -1;
        rotationThisFrame = degreesPerSecond * Time.deltaTime * rotationDirection;
        transform.RotateAround(hinge, Vector3.forward, rotationThisFrame);
        currentAngle += rotationThisFrame;
    }
}

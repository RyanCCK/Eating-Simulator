using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainOscillator : MonoBehaviour
{
    [SerializeField] public float xSpeed = 0f;
    [SerializeField] public float xAmp = 1f;
    [SerializeField] public float ySpeed = 0f;
    [SerializeField] public float yAmp = 1f;
    [SerializeField] public float zSpeed = 0f;
    [SerializeField] public float zAmp = 1f;
    private float xTrans = 0f;
    private float yTrans = 0f;
    private float zTrans = 0f;
    

    // Update is called once per frame
    void Update()
    {
        xTrans = (Mathf.Sin(Time.time * xSpeed) * xAmp) - (Mathf.Sin((Time.time - Time.deltaTime) * xSpeed) * xAmp);
        yTrans = (Mathf.Sin(Time.time * ySpeed) * yAmp) - (Mathf.Sin((Time.time - Time.deltaTime) * ySpeed) * yAmp);
        zTrans = (Mathf.Sin(Time.time * zSpeed) * zAmp) - (Mathf.Sin((Time.time - Time.deltaTime) * zSpeed) * zAmp);
        transform.Translate(xTrans, yTrans, zTrans);
    }
}

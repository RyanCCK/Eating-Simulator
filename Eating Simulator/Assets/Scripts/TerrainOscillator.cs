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
    private Vector3 initialPos;
    private Vector3 updatedPos;


    private void Awake()
    {
        initialPos = gameObject.transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        updatedPos.x = initialPos.x + Mathf.Sin(Time.time * xSpeed) * xAmp;
        updatedPos.y = initialPos.y + Mathf.Sin(Time.time * ySpeed) * yAmp;
        updatedPos.z = initialPos.z + Mathf.Sin(Time.time * zSpeed) * zAmp;
        gameObject.transform.position = updatedPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRotator : MonoBehaviour
{
    [SerializeField] public float xSpeed = 0f;
    [SerializeField] public float ySpeed = 0f;
    [SerializeField] public float zSpeed = 0f;
    [SerializeField] public float waitTime = 0f;

    private Vector3 center;
    private bool waiting = true;


    private void Awake()
    {
        Component[] childTransforms = GetComponentsInChildren<Transform>();
        foreach(Transform child in childTransforms)
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
            //x rotation
            gameObject.transform.RotateAround(center, Vector3.right, xSpeed * Time.deltaTime);
            //y rotation
            gameObject.transform.RotateAround(center, Vector3.up, ySpeed * Time.deltaTime);
            //z rotation
            gameObject.transform.RotateAround(center, Vector3.forward, zSpeed * Time.deltaTime);
        }
    }


    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(waitTime);
        waiting = false;
    }
}

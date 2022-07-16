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
    [SerializeField] public float waitTime = 0f;
    [SerializeField] public bool waitForPlayerContact = false;

    private float xTrans = 0f;
    private float yTrans = 0f;
    private float zTrans = 0f;
    private bool waiting = true;
    private bool playerContact = false;


    private void Start()
    {
        StartCoroutine(Waiting());
    }


    // Update is called once per frame
    void Update()
    {
        // Only move the terrain once the waiting period is over, 
        // AND either no player contact is required, or player contact is required and has been made
        if(!waiting && (!waitForPlayerContact || (waitForPlayerContact && playerContact)))
        {
            xTrans = (Mathf.Sin(Time.time * xSpeed) * xAmp) - (Mathf.Sin((Time.time - Time.deltaTime) * xSpeed) * xAmp);
            yTrans = (Mathf.Sin(Time.time * ySpeed) * yAmp) - (Mathf.Sin((Time.time - Time.deltaTime) * ySpeed) * yAmp);
            zTrans = (Mathf.Sin(Time.time * zSpeed) * zAmp) - (Mathf.Sin((Time.time - Time.deltaTime) * zSpeed) * zAmp);
            transform.Translate(xTrans, yTrans, zTrans);
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

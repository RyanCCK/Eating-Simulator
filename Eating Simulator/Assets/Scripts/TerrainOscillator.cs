using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainOscillator : MonoBehaviour
{
    [SerializeField] public Vector3 initialPosition;
    [SerializeField] public float xSpeed = 0f;
    [SerializeField] public float xAmp = 1f;
    [SerializeField] public float ySpeed = 0f;
    [SerializeField] public float yAmp = 1f;
    [SerializeField] public float zSpeed = 0f;
    [SerializeField] public float zAmp = 1f;
    [SerializeField] public bool waitForPlayerContact = false;
    [SerializeField] public float waitTime = 0f;
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;
    public AnimationCurve zCurve;

    private float xTrans = 0f;
    private float yTrans = 0f;
    private float zTrans = 0f;
    private bool waiting = false;
    private bool playerContact = true;


    private void Awake()
    {
        // Wait for player contact if necessary
        if (waitForPlayerContact) playerContact = false;
    }


    private void Start()
    {
        if (waitTime > 0)
        {
            waiting = true;
            StartCoroutine(Waiting());
        }
    }

    
    void Update()
    {
        if(!waiting && playerContact)
        {
            xTrans = (initialPosition.x + Mathf.Sin((Time.time * xSpeed)) * xAmp) - transform.position.x;
            yTrans = (initialPosition.y + Mathf.Sin((Time.time * ySpeed)) * yAmp) - transform.position.y;
            zTrans = (initialPosition.z + Mathf.Sin((Time.time * zSpeed)) * zAmp) - transform.position.z;

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

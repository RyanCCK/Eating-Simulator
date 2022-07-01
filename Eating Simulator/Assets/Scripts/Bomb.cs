using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] public float explosionRadius = 3f;

    private Collider[] colliders;
    private GameObject[] gameObjects;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            colliders = Physics.OverlapSphere(gameObject.transform.position, explosionRadius);
            foreach (var collider in colliders)
            {
                if(collider.gameObject.tag == "Destructible")
                    Destroy(collider.gameObject);
            }
            Destroy(gameObject);
        }
    }
}

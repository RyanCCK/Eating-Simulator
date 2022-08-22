using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] public PlayerController.State induceState;
    [SerializeField] public GameObject pickUpEffect;

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<ParticleSystem>().Stop();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            Instantiate(pickUpEffect, transform);
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }
}

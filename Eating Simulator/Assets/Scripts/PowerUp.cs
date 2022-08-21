using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        RocketBoost,
        Detonator,
        BouncyBall,
        Constructor,
        SeagullMorph
    }
    [SerializeField] public PowerUpType powerUpType;
    [SerializeField] public GameObject pickUpEffect;

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<ParticleSystem>().Stop();
            Instantiate(pickUpEffect, transform);
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }
}

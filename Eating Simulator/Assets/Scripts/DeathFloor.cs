using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    private GameManager gameManager;

    
    private void Start()
    {
        gameManager = GameManager.Instance;
    }


    // When player passes through death floor, call death event in game manager
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            gameManager.PlayerDeath();
    }
}

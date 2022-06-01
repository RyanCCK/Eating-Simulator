using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton class to control all high-level game events and functions
public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;
    public static GameManager Instance
    {
        get
        {
            if (!gameManager)
                Debug.LogError("Game Manager is null!");

            return gameManager;
        }
    }

    private void Awake()
    {
        gameManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

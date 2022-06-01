using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

// Singleton class to control all high-level game events and functions
public class GameManager : MonoBehaviour
{
    [SerializeField] public float deathScreenTime = 2f;

    private static GameManager gameManager;
    private GameObject player;
    private CameraDynamics cams;


    public static GameManager Instance
    {
        get
        {
            if (gameManager is null)
                Debug.LogError("Game Manager is null!");

            return gameManager;
        }
    }


    ////////////TODO////////////TODO////////////TODO////////////
    // FIX DUPLICATION ERROR
    // Issue where a new GameManager is created every time the scene is loaded
    ////////////TODO////////////TODO////////////TODO////////////

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameManager = this;
    }


    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cams = Camera.main.GetComponent<CameraDynamics>();
    }


    // Handles processing of events on player death
    public void PlayerDeath()
    {
        player.GetComponent<PlayerController>().isDead = true;
        StartCoroutine(DeathRoutine());
    }


    // Coroutine for timed death events
    private IEnumerator DeathRoutine()
    {
        ////////////TODO////////////TODO////////////TODO////////////
        // USE EVENTS
        ////////////TODO////////////TODO////////////TODO////////////
        
        //Issue here is that my GameManager has to save a reference to CameraDynamics script on my main camera gameobject.
        //  I could, instead, save a reference to the gameobject itself, then use GetComponent<CameraDynamics>.DeathView(),
        //  but this is basically the same issue, with an adedd step.
        //I think the best way to handle this would be with events. I could create a delegate for death events (such as camera events and scene loading),
        //then create an event from this delegate, and subscribe DeathView() to it. Then trigger the event from this script. 
        //  I should use events for all similarly called methods.

        cams.DeathView();   //Currently does noting
        yield return new WaitForSeconds(deathScreenTime);
        cams.RestoreToDefaults();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

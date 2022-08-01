using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

// Singleton class to control all high-level game events and functions
public class GameManager : MonoBehaviour
{
    [SerializeField] public float deathScreenTime = 2f;
    [SerializeField] public float levelLoadTime = 2f;

    public delegate void ResetAllObjectDefaults();
    public delegate void DeathAction();
    public delegate void LevelAction();
    public delegate void DeathCamera();
    
    public static event DeathAction onDeath;
    public static event LevelAction onLevelAdvance;
    public static event DeathCamera DeathCameraEvent;

    private int currentSceneIndex = 0;
    private int nextLevelIndex = 1;

    private static GameManager gameManager;


    public static GameManager Instance
    {
        get
        {
            if (gameManager is null)
                Debug.LogError("Game Manager is null!");

            return gameManager;
        }
    }
    

    private void Awake()
    {
        if (gameManager is null)
        {
            gameManager = this;
            DontDestroyOnLoad(gameManager);
        }
        else if (gameManager != this)
            Destroy(gameObject);
    }


    // Handles progression to the next level
    public void LoadNextLevel()
    {
        onLevelAdvance();   //Event
        StartCoroutine(NextLevelRoutine());
    }


    // Coroutine used for time level transition event
    private IEnumerator NextLevelRoutine()
    {
        yield return new WaitForSeconds(levelLoadTime);

        // Check if a next level exists. If so, load. If not, GAME WON!
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevelIndex);
            ++nextLevelIndex; ++currentSceneIndex;
        }
        else Debug.Log("YOU WON THE GAME!");

    }


    // Handles processing of events on player death
    public void PlayerDeath()
    {
        onDeath();      //Event
        StartCoroutine(DeathRoutine());
    }


    // Coroutine for timed death event
    private IEnumerator DeathRoutine()
    {
        DeathCameraEvent();     //Event
        yield return new WaitForSeconds(deathScreenTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

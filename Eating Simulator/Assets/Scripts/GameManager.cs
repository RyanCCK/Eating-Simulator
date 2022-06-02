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

    public static event ResetAllObjectDefaults resetAllObjectDefaults;
    public static event DeathAction onDeath;
    public static event LevelAction onLevelAdvance;
    public static event DeathCamera deathCameraEvent;

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


    // Start is called before the first frame update
    private void Start()
    {

    }


    // Handles progression to the next level
    public void LoadNextLevel()
    {
        onLevelAdvance();
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
        deathCameraEvent();     //Event
        yield return new WaitForSeconds(deathScreenTime);
        
        resetAllObjectDefaults();   //Event
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

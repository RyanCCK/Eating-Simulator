using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraDynamics : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField]  private CinemachineVirtualCamera deathCam;
    private CinemachineBrain brain;
    private GameManager gameManager;
    private GameObject player;


    // Require that virtualCam and deathCam have been assigned in inspector
    private void Awake()
    {
        brain = gameObject.GetComponent<CinemachineBrain>();
        if (virtualCam is null) Debug.LogError("virtualCam is null");
        if (deathCam is null) Debug.LogError("deathCam is null");
    }


    private void OnEnable()
    {
        GameManager.DeathCameraEvent += DeathCamera;
    }


    private void OnDisable()
    {
        GameManager.DeathCameraEvent -= DeathCamera;
    }


    // Initialize values for other gameObjects, and set default virtual camera values
    private void Start()
    {
        gameManager = GameManager.Instance;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    
    // Change active virtual camera to deathCam
    public void DeathCamera()
    {
        deathCam.Priority = virtualCam.Priority + 1;
    }
}

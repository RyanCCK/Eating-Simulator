using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraDynamics : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private CinemachineVirtualCamera deathCam;
    [SerializeField] private CinemachineVirtualCamera rocketBoostCam;
    private CinemachineBrain brain;
    private GameManager gameManager;
    private GameObject player;
    private CinemachineVirtualCamera activeCam;


    // Require that virtualCam and deathCam have been assigned in inspector
    private void Awake()
    {
        brain = gameObject.GetComponent<CinemachineBrain>();
        if (virtualCam is null) Debug.LogError("virtualCam is null");
        if (deathCam is null) Debug.LogError("deathCam is null");
        if (rocketBoostCam is null) Debug.LogError("rocketBoostCam is null");
        activeCam = virtualCam;
    }


    private void OnEnable()
    {
        GameManager.DeathCameraEvent += DeathCamera;
        PlayerController.OnRocketBoostPowerUp += RocketBoostCamera;
        PlayerController.OnDefaultState += VirtualCamera;
    }


    private void OnDisable()
    {
        GameManager.DeathCameraEvent -= DeathCamera;
        PlayerController.OnRocketBoostPowerUp -= RocketBoostCamera;
        PlayerController.OnDefaultState -= VirtualCamera;
    }


    // Initialize values for other gameObjects, and set default virtual camera values
    private void Start()
    {
        gameManager = GameManager.Instance;
        player = GameObject.FindGameObjectWithTag("Player");
    }


    // Change active virtual camera to virtualCam
    public void VirtualCamera()
    {
        virtualCam.Priority = activeCam.Priority + 1;
        activeCam = virtualCam;
    }

    
    // Change active virtual camera to deathCam
    public void DeathCamera()
    {
        deathCam.Priority = activeCam.Priority + 1;
        activeCam = deathCam;
    }


    // Change active virtual camera to rocketBoostCam
    public void RocketBoostCamera()
    {
        rocketBoostCam.Priority = activeCam.Priority + 1;
        activeCam = rocketBoostCam;
    }
}

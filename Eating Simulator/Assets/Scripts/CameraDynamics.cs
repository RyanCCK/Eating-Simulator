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

    private float camDistanceVirt = 8f;
    private float xDampingVirt = 1.5f;
    private float yDampingVirt = 0f;
    private float zDampingVirt = 1.5f;
    private bool unlimitedSoftVirt = true;
    private bool targetMovementEnableVirt = false;


    // Require that virtualCam and deathCam have been assigned in inspector
    private void Awake()
    {
        brain = gameObject.GetComponent<CinemachineBrain>();
        if (virtualCam is null) Debug.LogError("virtualCam is null");
        if (deathCam is null) Debug.LogError("deathCam is null");
    }


    private void OnEnable()
    {
        GameManager.resetAllObjectDefaults += SetToDefaults;
        GameManager.deathCameraEvent += DeathView;
    }


    private void OnDisable()
    {
        GameManager.resetAllObjectDefaults -= SetToDefaults;
        GameManager.deathCameraEvent -= DeathView;
    }


    // Initialize values for other gameObjects, and set default virtual camera values
    private void Start()
    {
        gameManager = GameManager.Instance;
        player = GameObject.FindGameObjectWithTag("Player");
        SetToDefaults();
    }


    ////////////TODO////////////TODO////////////TODO////////////
    // Set defaults for deathCam also
    ////////////TODO////////////TODO////////////TODO////////////

    // Sets all camera values to default values
    public void SetToDefaults()
    {
        //Set follow target
        virtualCam.Follow = player.transform;

        //Set all Framing Transposer component values
        /*
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = camDistanceVirt;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = xDampingVirt;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = yDampingVirt;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = zDampingVirt;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_UnlimitedSoftZone = unlimitedSoftVirt;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TargetMovementOnly = targetMovementEnableVirt;
        */
    }

    
    // Change active virtual camera to deathCam
    public void DeathView()
    {
        deathCam.Priority = virtualCam.Priority + 1;
    }
}

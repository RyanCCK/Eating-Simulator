using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraDynamics : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCam;
    private CinemachineBrain brain;
    private GameManager gameManager;
    private GameObject player;
    private float camDistance = 8f;


    private void Awake()
    {
        brain = gameObject.GetComponent<CinemachineBrain>(); 
    }

    // Declared as a coroutine to allow initialization of camera values.
    // ActiveVirtualCamera does not work without first waiting for a frame.
    private IEnumerator Start()
    {
        gameManager = GameManager.Instance;
        player = GameObject.FindGameObjectWithTag("Player");

        //Necessary to allow the CinemachineBrain to select an active virtual camera (takes 1 frame)
        yield return null;
        virtualCam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        ////////////TODO////////////TODO////////////TODO////////////
        // Set Default Camera Values
        ////////////TODO////////////TODO////////////TODO////////////
        
        //Set follow target
        virtualCam.Follow = player.transform;
        
        //Set body mode to "Framing Transposer"
        virtualCam.TryGetComponent<CinemachineFramingTransposer>(out var frameTrans);
        if (frameTrans is null) virtualCam.AddCinemachineComponent<CinemachineFramingTransposer>();

        //Set camera follow distance
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = camDistance;
    }


    // Resets all camera values to default values
    public void RestoreToDefaults()
    {
        ////////////TODO////////////TODO////////////TODO////////////
        //  Restore any camera values that are modified in any of the member functions herein
        //  to their rightful default values (as determined by inspector-enabled variables)
        ////////////TODO////////////TODO////////////TODO////////////

        //Set body mode to "Framing Transposer" and clear any aim mode
        virtualCam.TryGetComponent<CinemachineFramingTransposer>(out var frameTrans);
        if (frameTrans is null) virtualCam.AddCinemachineComponent<CinemachineFramingTransposer>();
        virtualCam.DestroyCinemachineComponent<CinemachineComposer>();

        virtualCam.Follow = player.transform;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = camDistance;

        virtualCam.Follow = player.transform;
        virtualCam.LookAt = player.transform;
    }


    // Cause camera translation to stop and begin rotating to watch player fall to their death
    public void DeathView()
    {
        virtualCam.LookAt = player.transform;
        virtualCam.AddCinemachineComponent<CinemachineComposer>();
        virtualCam.DestroyCinemachineComponent<CinemachineFramingTransposer>();

        //Create postprocessing effect
    }
}

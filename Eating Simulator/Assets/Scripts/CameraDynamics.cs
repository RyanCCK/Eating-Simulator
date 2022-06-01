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
    private float xDamping = 1.5f;
    private float yDamping = 0f;
    private float zDamping = 1.5f;
    private bool unlimitedSoft = true;
    private bool targetMovementEnable = false;


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
        
        ////////////TODO////////////TODO////////////TODO////////////
        // FIX INITIAL FRAME BUG
        // A glitched black frame appears at the moment of creation.
        // Find a way to fix this.
        ////////////TODO////////////TODO////////////TODO////////////

        //Set follow and look_at target
        virtualCam.Follow = player.transform;
        virtualCam.LookAt = player.transform;
        
        //Set body mode to "Framing Transposer"
        virtualCam.TryGetComponent<CinemachineFramingTransposer>(out var frameTrans);
        if (frameTrans is null) virtualCam.AddCinemachineComponent<CinemachineFramingTransposer>();

        //Set all Framing Transposer component values
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = camDistance;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = xDamping;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = yDamping;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = zDamping;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_UnlimitedSoftZone = unlimitedSoft;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TargetMovementOnly = targetMovementEnable;
    }

    //TODO: Change to "SetToDefaults" and call from Start (avoid duplicate code)

    // Resets all camera values to default values
    public void RestoreToDefaults()
    {
        ////////////TODO////////////TODO////////////TODO////////////
        //  Restore any camera values that are modified in any of the member functions herein
        //  to their rightful default values (as determined by inspector-enabled variables)
        ////////////TODO////////////TODO////////////TODO////////////

        //Destroy aim component
        virtualCam.DestroyCinemachineComponent<CinemachineComposer>();

        //Set follow and look_at target
        virtualCam.Follow = player.transform;
        virtualCam.LookAt = player.transform;

        //Set body mode to "Framing Transposer"
        virtualCam.TryGetComponent<CinemachineFramingTransposer>(out var frameTrans);
        if (frameTrans is null) virtualCam.AddCinemachineComponent<CinemachineFramingTransposer>();

        //Set all Framing Transposer component values
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = camDistance;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = xDamping;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = yDamping;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = zDamping;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_UnlimitedSoftZone = unlimitedSoft;
        virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TargetMovementOnly = targetMovementEnable;
    }


    ////////////TODO////////////TODO////////////TODO////////////
    // Smooth out the transition between camera modes.
    ////////////TODO////////////TODO////////////TODO////////////

    // Cause camera translation to stop and begin rotating to watch player fall to their death
    public void DeathView()
    {
        virtualCam.AddCinemachineComponent<CinemachineComposer>();
        virtualCam.DestroyCinemachineComponent<CinemachineFramingTransposer>();

        //Create postprocessing effect
    }
}

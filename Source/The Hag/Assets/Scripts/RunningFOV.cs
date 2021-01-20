using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningFOV : MonoBehaviour
{
    public Camera mainCamera;
    public PlayerMovement playerMovement;

    float defaultFov;

    public float distortionSensitivity = 0.0225f;

    void Start()
    {
        defaultFov = mainCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.isRunning || (playerMovement.isSliding && playerMovement.slopeSpeed > 4.5f)) 
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, (defaultFov + 15f), distortionSensitivity);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFov, distortionSensitivity);
        }
    }
}

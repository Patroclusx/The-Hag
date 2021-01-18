using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningFOV : MonoBehaviour
{
    public Camera mainCamera;
    public PlayerMovement playerMovement;

    float defaultFov;

    public float distortionSensitivity = 0.2f;

    void Start()
    {
        defaultFov = mainCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.isRunning) 
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, (defaultFov + 15f), distortionSensitivity);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFov, distortionSensitivity);
        }
    }
}

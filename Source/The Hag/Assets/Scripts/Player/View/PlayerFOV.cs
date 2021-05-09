using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    public Camera mainCamera;
    public PlayerMovement playerMovement;

    float defaultFov;

    public float distortionSensitivity = 3f;

    void Start()
    {
        defaultFov = mainCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.isRunning || playerMovement.verticalVelocity.y < -10f) 
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, (defaultFov + 15f), distortionSensitivity * Time.deltaTime);
        }
        else
        {
            if (mainCamera.fieldOfView > 60.01f)
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFov, distortionSensitivity * Time.deltaTime);
            }
        }
    }
}

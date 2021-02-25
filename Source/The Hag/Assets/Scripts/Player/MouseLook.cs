using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Range(0.3f, 2f)]
    public float mouseSens = 1f;
    public Transform playerBody;

    [HideInInspector]
    public bool isEnabled = true;
    [HideInInspector] 
    public float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            mouseLook();
        }
    }

    void mouseLook()
    {
        //Looking logic
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 70f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

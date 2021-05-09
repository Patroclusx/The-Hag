using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public PlayerMovement playerMovement;

    [Range(0.3f, 2f)]
    public float mouseSens = 1f;

    [HideInInspector]
    public bool isInteracting = false;
    [HideInInspector]
    public bool isInInventory = false;
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
        if (!isInteracting && !isInInventory)
        {
            mouseLook();
        }
    }

    void mouseLook()
    {
        //Looking logic
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

        if (mouseX != 0 || mouseY != 0)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -85f, 70f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerMovement.transform.Rotate(Vector3.up * mouseX);
        }
    }

    public void toggleInventoryCursor(bool isItemSelected)
    {
        isInInventory = !isInInventory;

        if (isInInventory)
        {
            PlayerStats.canInteract = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if(!isItemSelected)
                PlayerStats.canInteract = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

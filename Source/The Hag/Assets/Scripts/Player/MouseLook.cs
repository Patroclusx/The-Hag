using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Range(0.3f, 2f)]
    public float mouseSens = 1f;
    public Transform player;

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

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 70f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }

    public void toggleInventoryCursor()
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
            PlayerStats.canInteract = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInteraction : MonoBehaviour
{
    private GameObject windowObject;
    public bool isLocked;
    MouseLook mouseLook;

    bool isPlayerNearby = false;
    bool isWindowGrabbed = false;

    float yPosMotion;
    float lastYPosMotion;
    Vector3 defaultClosedPosition;
    Vector3 fromPosition;
    Vector3 toPosition;

    void Reset()
    {
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Window");
    }

    void Awake()
    {
        windowObject = gameObject;

        SphereCollider CC = windowObject.AddComponent<SphereCollider>();
        CC.isTrigger = true;
        CC.radius = PlayerStats.reachDistance + 0.2f;
    }

    void Start()
    {
        mouseLook = Camera.main.GetComponent<MouseLook>();
        defaultClosedPosition = windowObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWindowGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || !isPlayerNearby)
            {
                mouseLook.isInteracting = false;
                isWindowGrabbed = false;
                PlayerStats.canInteract = true;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (isWindowGrabbed)
            {
                yPosMotion = windowObject.transform.position.y;
                lastYPosMotion = yPosMotion;
                calcYDegMotion();

                if (lastYPosMotion != yPosMotion)
                {
                    fromPosition = windowObject.transform.position;
                    toPosition = new Vector3(fromPosition.x, yPosMotion, fromPosition.z);
                    windowObject.transform.position = Vector3.MoveTowards(fromPosition, toPosition, 1f);
                }
            }
            else
            {
                if (!isLocked)
                {
                    if (PlayerStats.canInteract && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        grabWindow();
                    }
                }
                else
                {
                    //TODO: Tell player window is locked
                    //      Make window unlockable with key, item or world object
                }
            }
        }
    }

    //Check if the player is near the window
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            isPlayerNearby = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerNearby = false;
        }
    }

    void calcYDegMotion()
    {
        float sensitivity = 0.01f;
        float mouseY = Input.GetAxis("Mouse Y") * (mouseLook.mouseSens * sensitivity);

        yPosMotion = Mathf.Clamp(yPosMotion + mouseY, defaultClosedPosition.y, defaultClosedPosition.y + 1.2f);
    }

    //Check if looking at window and grab it
    void grabWindow()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, LayerMask.GetMask("Window"), QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.transform.gameObject.Equals(windowObject))
            {
                mouseLook.isInteracting = true;
                PlayerStats.canInteract = false;
                isWindowGrabbed = true;
            }
        }
    }

    //EVENT HANDLERS FOR OTHER SCRIPTS

    public void eventLockWindow() {
        isLocked = true;
    }
    public void eventUnlockWindow()
    {
        isLocked = false;
    }
}

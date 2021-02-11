using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject doorObject;
    public bool isLocked;
    public bool isLeftWing;
    public LayerMask doorMask;
    MouseLook mouseLook;
    Animator doorHandleAnimator;

    bool isPlayerNearby = false;
    bool isDoorGrabbed = false;
    bool isSlammed = false;

    float zDegMotion;
    float physicsVelocity = 0f;
    Quaternion defaultClosedRotation;
    Quaternion fromRotation;
    Quaternion toRotation;
    IEnumerator prevCoroutine;
    float lerpTimer;

    //Vector dot products
    float mouseDotProduct = 0f;
    float walkDotProduct = 0f;

    void Awake()
    {
        CapsuleCollider CC = doorObject.AddComponent<CapsuleCollider>();
        CC.isTrigger = true;
        CC.direction = 0;
        CC.radius = PlayerStats.reachDistance * 0.7f;
        CC.height = PlayerStats.reachDistance * 2f;
    }

    void Start()
    {
        mouseLook = Camera.main.GetComponent<MouseLook>();
        doorHandleAnimator = doorObject.GetComponent<Animator>();
        defaultClosedRotation = doorObject.transform.parent.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) || !isPlayerNearby || isSlammed)
        {
            if (isDoorGrabbed)
            {
                mouseLook.isEnabled = true;

                if (physicsVelocity != 0f)
                {
                    calcVelocityToRotation(1f);

                    if (prevCoroutine == null)
                    {
                        if (isSlammed)
                        {
                            prevCoroutine = applyDoorVelocity(false);
                        }
                        else
                        {
                            prevCoroutine = applyDoorVelocity(true);
                        }
                        StartCoroutine(prevCoroutine);
                    }
                }

                isDoorGrabbed = false;
                PlayerStats.canInteract = true;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (isDoorGrabbed)
            {
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    slamDoor();
                    return;
                }

                zDegMotion = doorObject.transform.localEulerAngles.z;
                float lastZDeg = zDegMotion;
                calcZDegMotion();

                if (Mathf.Abs(lastZDeg - zDegMotion) > 0.04f)
                {
                    if (prevCoroutine != null)
                    {
                        StopCoroutine(prevCoroutine);
                        prevCoroutine = null;
                        physicsVelocity = 0f;
                    }

                    fromRotation = doorObject.transform.localRotation;
                    toRotation = Quaternion.Euler(fromRotation.eulerAngles.x, fromRotation.eulerAngles.y, zDegMotion);
                    doorObject.transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, 1f);

                    //Calc highest velocity applied by player
                    float tempVelocity = (lastZDeg - zDegMotion) * 45f;
                    if (Mathf.Abs(physicsVelocity) < Mathf.Abs(tempVelocity) && Mathf.Abs(lastZDeg - zDegMotion) > 0.12f) {
                        physicsVelocity = tempVelocity;
                    }
                }
                else if(physicsVelocity != 0)
                {
                    calcVelocityToRotation(0.4f);

                    if (prevCoroutine == null)
                    {
                        prevCoroutine = applyDoorVelocity(true);
                        StartCoroutine(prevCoroutine);
                    }
                }
            }
            else
            {
                if (!isLocked && !isSlammed)
                {
                    if (PlayerStats.canInteract && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        grabDoor();
                    }
                }
                else
                {
                    //TODO: Tell player door is locked
                    //      Make door unlockable with key, item or world object
                }
            }
        }

        if (!isDoorGrabbed && prevCoroutine == null && !isDoorClosed(true) && isDoorClosed(false))
        {
            closeDoor();
        }
    }

    bool isDoorClosed(bool isDeadShut)
    {
        if (isLeftWing)
        {
            if (doorObject.transform.localEulerAngles.z < (isDeadShut ? defaultClosedRotation.eulerAngles.z : defaultClosedRotation.eulerAngles.z + 358.5f))
            {
                return false;
            }
        }
        else
        {
            if (doorObject.transform.localEulerAngles.z > (isDeadShut ? defaultClosedRotation.eulerAngles.z : defaultClosedRotation.eulerAngles.z + 1.5f))
            {
                return false;
            }
        }

        return true;
    }

    //Shuts the door instantly
    void closeDoor()
    {
        doorObject.transform.localRotation = Quaternion.Euler(doorObject.transform.localEulerAngles.x, doorObject.transform.localEulerAngles.y, defaultClosedRotation.eulerAngles.z);
    }

    //Check of the player is near the door
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

    //Clamps rotation to min and max door openings
    float clampRotation(float rotation)
    {
        if (isLeftWing)
        {
            return Mathf.Clamp(rotation, defaultClosedRotation.eulerAngles.z + 240f, defaultClosedRotation.eulerAngles.z + 360f);
        }
        else
        {
            return Mathf.Clamp(rotation, defaultClosedRotation.eulerAngles.z, defaultClosedRotation.eulerAngles.z + 120f);
        }
    }

    //Calculate the rotation from the velocity after player stops moving door
    void calcVelocityToRotation(float multiplier)
    {
        fromRotation = doorObject.transform.localRotation;
        float toRotationZVelocity = clampRotation(fromRotation.eulerAngles.z - physicsVelocity * multiplier);
        if (toRotationZVelocity != fromRotation.eulerAngles.z)
        {
            toRotation = Quaternion.Euler(fromRotation.eulerAngles.x, fromRotation.eulerAngles.y, toRotationZVelocity);
        }

        physicsVelocity = 0f;
        lerpTimer = 0f;
    }

    //Apply calculated velocity to simulate physics
    IEnumerator applyDoorVelocity(bool useSmoothing)
    {
        while (lerpTimer < 1f)
        {
            lerpTimer += Time.deltaTime / (useSmoothing ? 1.4f : 1f);
            doorObject.transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, 1-Mathf.Pow(1-lerpTimer, 3));

            yield return null;
        }

        isSlammed = false;
        prevCoroutine = null;
    }

    void calcZDegMotion()
    {
        float sensitivity = 0.7f;
        float mouseX = Input.GetAxis("Mouse X") * (mouseLook.mouseSens * sensitivity);
        float mouseY = Input.GetAxis("Mouse Y") * (mouseLook.mouseSens * sensitivity);
        float walkX = Input.GetAxis("Vertical") * sensitivity;
        float walkY = Input.GetAxis("Horizontal") * sensitivity;

        calcMouseDotProduct();
        calcWalkDotProduct();

        //Player movement
        if (walkDotProduct < -0.5f)
        {
            //Front Side
            zDegMotion = clampRotation(isLeftWing ? zDegMotion + walkX : zDegMotion - walkX);
        }
        else if (walkDotProduct > 0.5f)
        {
            //Back Side
            zDegMotion = clampRotation(isLeftWing ? zDegMotion - walkX : zDegMotion + walkX);
        }
        else
        {
            //In between
            zDegMotion = clampRotation(zDegMotion - walkY);
        }

        //Mouse movement
        if(walkDotProduct < 0f)
        {
            //Front side
            if(mouseDotProduct < 0.46f && mouseDotProduct > -0.46f)
            {
                //Middle look
                zDegMotion = clampRotation(isLeftWing ? zDegMotion + mouseY : zDegMotion - mouseY);
            }
            if(mouseDotProduct > 0.43f || mouseDotProduct < -0.43f)
            {
                //Angled look
                if (mouseDotProduct > 0.455f)
                {
                    zDegMotion = clampRotation(isLeftWing ? zDegMotion + mouseX : zDegMotion - mouseX);
                }
                else if (mouseDotProduct < -0.455f)
                {
                    zDegMotion = clampRotation(isLeftWing ? zDegMotion - mouseX : zDegMotion + mouseX);
                }
            }
        }
        else
        {
            //Back side
            if (mouseDotProduct < 0.46f && mouseDotProduct > -0.46f)
            {
                //Middle look
                zDegMotion = clampRotation(isLeftWing ? zDegMotion - mouseY : zDegMotion + mouseY);
            }
            if (mouseDotProduct > 0.43f || mouseDotProduct < -0.43f)
            {
                //Angled look
                if (mouseDotProduct > 0.455f)
                {
                    zDegMotion = clampRotation(isLeftWing ? zDegMotion + mouseX : zDegMotion - mouseX);
                }
                else if (mouseDotProduct < -0.455f)
                {
                    zDegMotion = clampRotation(isLeftWing ? zDegMotion - mouseX : zDegMotion + mouseX);
                }
            }
        }
    }

    //Calculates direction of walking applied in relation to door's facing
    float calcWalkDotProduct()
    {
        Vector3 doorVector = doorObject.transform.up;
        Vector3 cameraVector = Camera.main.transform.forward;
        return walkDotProduct = Vector3.Dot(cameraVector, doorVector);
    }

    //Calculates direction of mouse movement applied in relation to door's facing
    float calcMouseDotProduct()
    {
        Vector3 doorVector1 = doorObject.transform.up;
        Vector3 doorVector2 = doorObject.transform.forward;
        Vector3 doorLook = Vector3.Cross(doorVector2, doorVector1);
        Vector3 cameraVector = Camera.main.transform.forward;
        return mouseDotProduct = Vector3.Dot(cameraVector, doorLook);
    }

    //Check if looking at door and grab it
    void grabDoor()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, doorMask, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.transform.gameObject.Equals(doorObject))
            {
                //Stop any door motion
                if (prevCoroutine != null)
                {
                    StopCoroutine(prevCoroutine);
                    prevCoroutine = null;
                    physicsVelocity = 0f;
                }

                //Play door handle animation
                if (doorHandleAnimator != null)
                {
                    if (isDoorClosed(true))
                    {
                        doorHandleAnimator.SetTrigger("isOpening");
                    }
                }

                mouseLook.isEnabled = false;
                PlayerStats.canInteract = false;
                isDoorGrabbed = true;
            }
        }
    }

    //Applies huge velocity to quickly close or open the door
    void slamDoor()
    {
        isSlammed = true;

        if (isLeftWing)
        {
            if (walkDotProduct > 0.02f)
            {
                physicsVelocity = 1000f;
            }
            else if (walkDotProduct < -0.02f)
            {
                physicsVelocity = -1000f;
            }
        }
        else
        {
            if (walkDotProduct > 0.02f)
            {
                physicsVelocity = -1000f;
            }
            else if (walkDotProduct < -0.02f)
            {
                physicsVelocity = 1000f;
            }
        }
    }
}

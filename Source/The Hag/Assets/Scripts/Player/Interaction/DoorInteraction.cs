using System.Collections;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    private GameObject doorObject;
    [Range(80f, 140f)]
    public float maxOpeningDegrees = 120f;
    public bool isLeftSided;
    public bool isLocked;
    GameObject player;
    MouseLook mouseLook;
    Animator doorHandleAnimator;

    bool isDoorGrabbed = false;
    bool isSlammed = false;

    float yDegMotion;
    float lastYDegMotion;
    float motionVelocity;
    float physicsVelocity = 0f;
    Quaternion defaultClosedRotation;
    Quaternion fromRotation;
    Quaternion toRotation;
    IEnumerator prevCoroutine;
    float lerpTimer;

    //Vector dot products
    float mouseDotProduct = 0f;
    float walkDotProduct = 0f;

    void Reset()
    {
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Door");
    }

    void Start()
    {
        doorObject = gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        mouseLook = Camera.main.GetComponent<MouseLook>();
        doorHandleAnimator = doorObject.GetComponentInChildren<Animator>();
        defaultClosedRotation = doorObject.transform.parent.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDoorGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || !isPlayerNearby() || isSlammed)
            {
                mouseLook.isInteracting = false ;

                applyVelocityToDoor(1f);

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

                yDegMotion = doorObject.transform.eulerAngles.y;
                lastYDegMotion = yDegMotion;
                calcYDegMotion();

                if (Mathf.Abs(motionVelocity) > 0.04f)
                {
                    if (prevCoroutine != null)
                    {
                        StopCoroutine(prevCoroutine);
                        prevCoroutine = null;
                        physicsVelocity = 0f;
                    }

                    fromRotation = doorObject.transform.rotation;
                    toRotation = Quaternion.Euler(doorObject.transform.eulerAngles.x, yDegMotion, doorObject.transform.eulerAngles.z);
                    doorObject.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, 1f);

                    //Calc highest velocity applied by player
                    float calcVelocity = motionVelocity * 25f;
                    if (Mathf.Abs(physicsVelocity) < Mathf.Abs(calcVelocity) && Mathf.Abs(motionVelocity) > 0.12f) {
                        physicsVelocity = Mathf.Clamp(calcVelocity, -maxOpeningDegrees, maxOpeningDegrees);
                    }
                }
                else
                {
                    applyVelocityToDoor(0.8f);
                }
            }
            else
            {
                if (PlayerStats.canInteract && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    grabDoor();
                }
            }
        }

        if (!isDoorGrabbed && prevCoroutine == null && !isDoorClosed(0f) && isDoorClosed(1.5f))
        {
            closeDoor();
        }
    }

    //Check if player is near the door
    bool isPlayerNearby()
    {
        Vector3 doorOrigin = transform.position;
        Vector3 doorEdgeRight = doorOrigin + transform.right * (transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x - 0.1f);
        Vector3 doorEdgeLeft = doorOrigin - transform.right * (transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x - 0.1f);
        bool playerNear = Physics.CheckCapsule(doorOrigin, isLeftSided ? doorEdgeLeft : doorEdgeRight, PlayerStats.reachDistance - 0.1f, LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        return playerNear;
    }

    //Check if door is near to closed position by specified ammount
    //Passing 0f will check for fully closed state
    bool isDoorClosed(float deviationInDegrees)
    {
        float angle = Quaternion.Angle(doorObject.transform.rotation, defaultClosedRotation);

        if (angle <= deviationInDegrees)
        {
            return true;
        }

        return false;
    }

    //Shuts the door
    void closeDoor()
    {
        fromRotation = doorObject.transform.rotation;
        toRotation = Quaternion.Euler(doorObject.transform.eulerAngles.x, defaultClosedRotation.eulerAngles.y, doorObject.transform.eulerAngles.z);

        doorObject.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, 0.1f);
    }

    //Clamps rotation to min and max door openings
    float clampRotation(float rotation)
    {
        //Calculate motion velocity
        motionVelocity = rotation - lastYDegMotion;

        //Check and clamp rotation
        if (isLeftSided)
        {
            float fromRotation = defaultClosedRotation.eulerAngles.y;
            float calcMaxRotation = fromRotation - maxOpeningDegrees;
            float toRotation = calcMaxRotation < 0f ? 360f + calcMaxRotation : calcMaxRotation;

            //Negative rotation
            if (fromRotation > toRotation)
            {
                if (rotation <= fromRotation && rotation >= toRotation)
                {
                    return rotation;
                }
                else
                {
                    return Mathf.Clamp(rotation, toRotation, fromRotation);
                }
            }
            else
            {
                float wrappedRotation = rotation < 0f ? 360f + rotation : rotation >= 360f ? rotation - 360f : rotation;

                if (!(wrappedRotation > fromRotation && wrappedRotation < toRotation))
                {
                    return wrappedRotation;
                }
                else if (rotation > fromRotation && motionVelocity > 0)
                {
                    return fromRotation;
                }
                else if (rotation < toRotation && motionVelocity < 0)
                {
                    return toRotation;
                }
            }
        }
        else
        {
            float fromRotation = defaultClosedRotation.eulerAngles.y;
            float calcMaxRotation = fromRotation + maxOpeningDegrees;
            float toRotation = calcMaxRotation >= 360f ? calcMaxRotation - 360f : calcMaxRotation;

            //Positive rotation
            if (fromRotation > toRotation)
            {
                float wrappedRotation = rotation < 0f ? 360f + rotation : rotation >= 360f ? rotation - 360f : rotation;

                if (!(wrappedRotation < fromRotation && wrappedRotation > toRotation))
                {
                    return wrappedRotation;
                }
                else if (rotation < fromRotation && motionVelocity < 0)
                {
                    return fromRotation;
                }
                else if (rotation > toRotation && motionVelocity > 0)
                {
                    return toRotation;
                }
            }
            else
            {
                if (rotation >= fromRotation && rotation <= toRotation)
                {
                    return rotation;
                }
                else
                {
                    return Mathf.Clamp(rotation, fromRotation, toRotation);
                }
            }
        }

        return rotation;
    }

    //Apply velocity to door
    void applyVelocityToDoor(float multiplier)
    {
        if (physicsVelocity != 0)
        {
            calcVelocityToRotation(multiplier);

            if (prevCoroutine == null)
            {
                if (isSlammed)
                {
                    prevCoroutine = moveDoorByVelocity(false);
                }
                else
                {
                    prevCoroutine = moveDoorByVelocity(true);
                }
                StartCoroutine(prevCoroutine);
            }
        }
    }

    //Calculate the rotation from the velocity
    void calcVelocityToRotation(float multiplier)
    {
        fromRotation = doorObject.transform.rotation;
        float toRotationYVelocity = clampRotation(fromRotation.eulerAngles.y + physicsVelocity * multiplier);
        if (toRotationYVelocity != fromRotation.eulerAngles.y)
        {
            toRotation = Quaternion.Euler(doorObject.transform.eulerAngles.x, toRotationYVelocity, doorObject.transform.eulerAngles.z);
        }

        physicsVelocity = 0f;
        lerpTimer = 0f;
    }

    //Moves the door by the applied velocity
    IEnumerator moveDoorByVelocity(bool useSmoothing)
    {
        while (lerpTimer < 1f)
        {
            lerpTimer += Time.deltaTime / (useSmoothing ? 1.4f : 1f);
            doorObject.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, 1-Mathf.Pow(1-lerpTimer, 3));

            yield return null;
        }

        isSlammed = false;
        prevCoroutine = null;
    }

    void calcYDegMotion()
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
            yDegMotion = clampRotation(isLeftSided ? yDegMotion - walkX : yDegMotion + walkX);
        }
        else if (walkDotProduct > 0.5f)
        {
            //Back Side
            yDegMotion = clampRotation(isLeftSided ? yDegMotion + walkX : yDegMotion - walkX);
        }
        else
        {
            //In between
            yDegMotion = clampRotation(yDegMotion - walkY);
        }

        //Mouse movement
        if(walkDotProduct < 0f)
        {
            //Front side
            if(mouseDotProduct < 0.46f && mouseDotProduct > -0.46f)
            {
                //Middle look
                yDegMotion = clampRotation(isLeftSided ? yDegMotion - mouseY : yDegMotion + mouseY);
            }
            if(mouseDotProduct > 0.43f || mouseDotProduct < -0.43f)
            {
                //Angled look
                if (mouseDotProduct > 0.455f)
                {
                    yDegMotion = clampRotation(isLeftSided ? yDegMotion + mouseX : yDegMotion - mouseX);
                }
                else if (mouseDotProduct < -0.455f)
                {
                    yDegMotion = clampRotation(isLeftSided ? yDegMotion - mouseX : yDegMotion + mouseX);
                }
            }
        }
        else
        {
            //Back side
            if (mouseDotProduct < 0.46f && mouseDotProduct > -0.46f)
            {
                //Middle look
                yDegMotion = clampRotation(isLeftSided ? yDegMotion + mouseY : yDegMotion - mouseY);
            }
            if (mouseDotProduct > 0.43f || mouseDotProduct < -0.43f)
            {
                //Angled look
                if (mouseDotProduct > 0.455f)
                {
                    yDegMotion = clampRotation(isLeftSided ? yDegMotion + mouseX : yDegMotion - mouseX);
                }
                else if (mouseDotProduct < -0.455f)
                {
                    yDegMotion = clampRotation(isLeftSided ? yDegMotion - mouseX : yDegMotion + mouseX);
                }
            }
        }
    }

    //Calculates direction of walking applied in relation to door's facing
    float calcWalkDotProduct()
    {
        Vector3 doorVector = doorObject.transform.forward;
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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, LayerMask.GetMask("Door"), QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.transform.gameObject.Equals(doorObject))
            {
                if (!isLocked)
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
                        if (isDoorClosed(0f))
                        {
                            switch (isLeftSided)
                            {
                                case true:
                                    doorHandleAnimator.SetTrigger("triggerHandleLeft");
                                    break;
                                case false:
                                    doorHandleAnimator.SetTrigger("triggerHandleRight");
                                    break;
                            }
                        }
                    }

                    mouseLook.isInteracting = true;
                    PlayerStats.canInteract = false;
                    isSlammed = false;
                    isDoorGrabbed = true;
                }
                else
                {
                    HintUI.instance.displayHintMessage("Door is locked!");
                }
            }
        }
    }

    //Applies max velocity to close or open the door
    void slamDoor()
    {
        isSlammed = true;

        if (walkDotProduct > 0.02f)
        {
            physicsVelocity = isLeftSided ? maxOpeningDegrees : -maxOpeningDegrees;
        }
        else if (walkDotProduct < -0.02f)
        {
            physicsVelocity = isLeftSided ? -maxOpeningDegrees : maxOpeningDegrees;
        }
    }

    //EVENT HANDLERS FOR OTHER SCRIPTS

    public void eventLockDoor() {
        isLocked = true;
    }
    public void eventUnlockDoor()
    {
        isLocked = false;
    }

    public void eventApplyVelocityToDoor(float velocity)
    {
        if (velocity != 0)
        {
            physicsVelocity = velocity;
            applyVelocityToDoor(1f);
        }
    }

    public void eventSlamDoor(bool isOpening)
    {
        isSlammed = true;

        if (isOpening)
        {
            physicsVelocity = isLeftSided ? maxOpeningDegrees : -maxOpeningDegrees;
        }
        else
        {
            physicsVelocity = isLeftSided ? maxOpeningDegrees : -maxOpeningDegrees;
        }
    }
}

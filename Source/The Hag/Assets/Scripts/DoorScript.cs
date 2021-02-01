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

    bool isPlayerNearby = false;
    bool isDoorGrabbed = false;
    bool isSlammed = false;

    float xDegMotion;
    float physicsVelocity = 0f;
    Quaternion fromRotation;
    Quaternion toRotation;
    IEnumerator prevCoroutine;
    float lerpTimer;

    float mouseDotProduct = 0f;
    float walkDotProduct = 0f;

    void Start()
    {
        mouseLook = Camera.main.GetComponent<MouseLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (isDoorGrabbed)
            {
                if (Input.GetButton("Fire2"))
                {
                    isSlammed = true;

                    if (isLeftWing)
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
                    else
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
                }

                xDegMotion = doorObject.transform.rotation.eulerAngles.y;
                float lastXDeg = xDegMotion;

                float mouseX = Input.GetAxis("Mouse X") * (mouseLook.mouseSens / 2f);
                float walkX = Input.GetAxis("Vertical") * 0.7f;

                calcWalkDotProduct();

                if (walkDotProduct > 0.3f)
                {
                    xDegMotion = clampRotation(isLeftWing ? xDegMotion + walkX : xDegMotion - walkX);
                }
                else if (walkDotProduct < -0.3f)
                {
                    xDegMotion = clampRotation(isLeftWing ? xDegMotion - walkX : xDegMotion + walkX);
                }

                if (isLeftWing && ((walkDotProduct > 0f && mouseDotProduct > -0.3f) || (walkDotProduct < 0f && mouseDotProduct > 0.3f)))
                {
                    xDegMotion = clampRotation(isLeftWing ? xDegMotion - mouseX : xDegMotion + mouseX);
                }
                else if (!isLeftWing && ((walkDotProduct > 0f && mouseDotProduct > 0.3f) || (walkDotProduct < 0f && mouseDotProduct > -0.3f)))
                {
                    xDegMotion = clampRotation(isLeftWing ? xDegMotion - mouseX : xDegMotion + mouseX);
                }
                else
                {
                    xDegMotion = clampRotation(isLeftWing ? xDegMotion + mouseX : xDegMotion - mouseX);
                }

                if (Mathf.Abs(lastXDeg - xDegMotion) > 0.04f && !isSlammed)
                {
                    if (prevCoroutine != null)
                    {
                        StopCoroutine(prevCoroutine);
                        prevCoroutine = null;
                        physicsVelocity = 0f;
                    }

                    fromRotation = doorObject.transform.rotation;
                    toRotation = Quaternion.Euler(fromRotation.eulerAngles.x, xDegMotion, fromRotation.eulerAngles.z);
                    doorObject.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, 1f);

                    float tempVelocity = (lastXDeg - xDegMotion) * 45f;
                    if (Mathf.Abs(physicsVelocity) < Mathf.Abs(tempVelocity) && Mathf.Abs(lastXDeg - xDegMotion) > 0.12f) {
                        physicsVelocity = tempVelocity;
                    }
                }
                else if(physicsVelocity != 0 && !isSlammed)
                {
                    calcVelocity();

                    if (prevCoroutine == null)
                    {
                        prevCoroutine = applyDoorVelocity();
                        StartCoroutine(prevCoroutine);
                    }
                }
            }
            else
            {
                if (!isSlammed)
                {
                    if (PlayerStats.canInteract && !isLocked)
                    {
                        grabDoor();
                    }
                }
            }
        }
        if (Input.GetButtonUp("Fire1") || !isPlayerNearby || isSlammed)
        {
            if (isDoorGrabbed)
            {
                mouseLook.isEnabled = true;

                if (physicsVelocity != 0f)
                {
                    calcVelocity();

                    if (prevCoroutine == null)
                    {
                        prevCoroutine = applyDoorVelocity();
                        StartCoroutine(prevCoroutine);
                    }
                }

                isDoorGrabbed = false;
                PlayerStats.canInteract = true;
            }
        }

        if (!isDoorGrabbed)
        {
            tryToCloseDoor();
        }
    }

    bool tryToCloseDoor()
    {
        if((isLeftWing && doorObject.transform.eulerAngles.y > 179f) || (!isLeftWing && doorObject.transform.eulerAngles.y < 179f))
        {
            doorObject.transform.rotation = Quaternion.Euler(doorObject.transform.eulerAngles.x, 180f, doorObject.transform.eulerAngles.z);

            return true;
        }

        return false;
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
            return Mathf.Clamp(rotation, 60f, 180f);
        }
        else
        {
            return Mathf.Clamp(rotation, 180f, 300f);
        }
    }

    void calcVelocity()
    {
        fromRotation = doorObject.transform.rotation;
        float toRotationYVelocity = clampRotation(fromRotation.eulerAngles.y - physicsVelocity);
        if (toRotationYVelocity != fromRotation.eulerAngles.y)
        {
            toRotation = Quaternion.Euler(fromRotation.eulerAngles.x, toRotationYVelocity, fromRotation.eulerAngles.z);
        }

        physicsVelocity = 0f;
        lerpTimer = 0f;
    }

    IEnumerator applyDoorVelocity()
    {
        while (lerpTimer < 1f)
        {
            lerpTimer += Time.deltaTime / 1.5f;
            doorObject.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, 1-Mathf.Pow(1-lerpTimer, 3));

            yield return null;
        }

        isSlammed = false;
        prevCoroutine = null;
    }

    //Calculates direction of walking applied in relation to door's facing
    void calcWalkDotProduct()
    {
        Vector3 doorVector = doorObject.transform.up;
        Vector3 cameraVector = Camera.main.transform.forward;
        walkDotProduct = Vector3.Dot(cameraVector, doorVector);
    }

    void calcMouseDotProduct()
    {
        Vector3 doorVector1 = doorObject.transform.up;
        Vector3 doorVector2 = doorObject.transform.forward;
        Vector3 doorLook = Vector3.Cross(doorVector2, doorVector1);
        Vector3 cameraVector = Camera.main.transform.forward;
        mouseDotProduct = Vector3.Dot(cameraVector, doorLook);
    }

    //Check if looking at door and grab it
    void grabDoor()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, doorMask, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.transform.gameObject.Equals(doorObject))
            {
                if (prevCoroutine != null)
                {
                    StopCoroutine(prevCoroutine);
                    prevCoroutine = null;
                    physicsVelocity = 0f;
                }

                mouseLook.isEnabled = false;
                PlayerStats.canInteract = false;
                isDoorGrabbed = true;
                calcMouseDotProduct();
            }
        }
    }
}

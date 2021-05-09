﻿using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Camera mainCamera;
    public MouseLook mouseLook;
    public LayerMask ignoredLayer;

    Transform defaultParent;
    Vector3 defaultScale;
    float defaultDrag = 0f;
    float defaultAngularDrag = 0f;
    float defaultPlayerWalkSpeed;

    [HideInInspector]
    public bool carryingObject = false;
    [HideInInspector]
    public bool carryingHeavyObject = false;
    bool isObjectGrabbed = false;
    float maxObjectWeight = 5f;
    float objDistanceBySize;
    Vector3 lastPosition;
    Vector3 lastVelocity;
    GameObject objectInHand;
    Rigidbody objectInHandRB;

    void Start()
    {
        defaultPlayerWalkSpeed = playerMovement.playerStats.walkSpeed;
    }

    void Update()
    {
        if (isObjectGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || !carryingObject)
            {
                if (carryingObject)
                {
                    dropObj();
                }

                isObjectGrabbed = false;
                mouseLook.isInteracting = false;
                PlayerStats.canInteract = true;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!carryingObject)
            {
                if (PlayerStats.canInteract && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    pickUpObject();
                }
            }
            else
            {
                if (objectInHandRB.mass < maxObjectWeight)
                {
                    carryObject();

                    //Object rotation in hand
                    if (Input.GetKey(KeyCode.R))
                    {
                        mouseLook.isInteracting = true;

                        float mouseX = Input.GetAxis("Mouse X") * 2f;
                        float mouseY = Input.GetAxis("Mouse Y") * 2f;

                        objectInHand.transform.Rotate(mainCamera.transform.up, -mouseX, Space.World);
                        objectInHand.transform.Rotate(mainCamera.transform.right, mouseY, Space.World);
                    }
                    else if (Input.GetKeyUp(KeyCode.R))
                    {
                        mouseLook.isInteracting = false;
                    }
                }
                else
                {
                    mouseLook.isInteracting = true;
                    dragObject();
                }
            }
        }
    }

    void calcDistanceBySize()
    {
        Vector3 objBoundsSize = Vector3.Scale(objectInHand.transform.localScale, objectInHand.GetComponent<MeshFilter>().sharedMesh.bounds.size);
        objDistanceBySize = Mathf.Clamp(objBoundsSize.magnitude - 0.75f, 0f, 0.4f);

        gameObject.transform.position = gameObject.transform.position + gameObject.transform.forward * objDistanceBySize;
    }

    void pickUpObject()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore))
        {
            objectInHand = hitInfo.transform.gameObject;
            objectInHandRB = objectInHand.GetComponent<Rigidbody>();

            if (objectInHand.tag == "Interactable" && objectInHandRB != null)
            {
                if (!checkIfObjUnderPlayer())
                {
                    //Get object defaults
                    defaultParent = objectInHand.transform.parent;
                    defaultScale = objectInHand.transform.localScale;
                    defaultDrag = objectInHandRB.drag;
                    defaultAngularDrag = objectInHandRB.angularDrag;

                    //Set object params
                    calcDistanceBySize();
                    if (objectInHandRB.mass < maxObjectWeight)
                    {
                        objectInHand.transform.parent = gameObject.transform;
                        objectInHandRB.useGravity = false;
                    }
                    else
                    {
                        playerMovement.playerStats.walkSpeed = playerMovement.playerStats.walkSpeed * 0.7f - (1f - 5f / objectInHandRB.mass);
                        playerMovement.playerStats.setCanRun(false);
                        carryingHeavyObject = true;
                    }
                    stopObjectForces();
                    objectInHand.layer = LayerMask.NameToLayer("ObjectCarried");

                    carryingObject = true;
                    isObjectGrabbed = true;
                    PlayerStats.canInteract = false;
                }
            }
        }
    }

    void carryObject()
    {
        //Get throw direction
        getVelocityDirection();

        //Try to move object to center of camera
        centerHandOject();
        lastVelocity = getVelocity();

        //If object cannot be held anymore drop it
        checkObjDrop();

        //Throw object
        if (Input.GetMouseButtonDown(1))
        {
            dropObj();
            objectInHandRB.AddForce(mainCamera.transform.forward * PlayerStats.throwForce);
        }
    }

    void dragObject()
    {
        //Move object with player
        Vector3 objPosition = objectInHand.transform.position;
        Vector3 toPosition = objPosition + playerMovement.moveVelocity * playerMovement.playerSpeed;

        if(playerMovement.isPlayerMoving())
            objectInHand.transform.position = Vector3.Lerp(objPosition, toPosition, Time.deltaTime);

        //If object out of hands than let go
        bool heavyHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, PlayerStats.reachDistance, LayerMask.GetMask("ObjectCarried"), QueryTriggerInteraction.Ignore);
        if (!heavyHit)
        {
            dropObj();
        }

        //Push object away
        if (Input.GetMouseButtonDown(1))
        {
            float force = 6f;

            dropObj();
            objectInHandRB.AddForce(playerMovement.transform.forward * force * PlayerStats.throwForce);
        }
    }

    void dropObj()
    {
        //If velocity persists, apply that to the item
        objectInHandRB.AddForce(getVelocityDirection() * Mathf.Clamp(lastVelocity.magnitude * 2f, 0f, 30f), ForceMode.Force);

        //Reset object back to default
        objectInHand.transform.parent = defaultParent;
        objectInHand.layer = LayerMask.NameToLayer("Object");
        objectInHand.transform.localScale = defaultScale;
        objectInHandRB.drag = defaultDrag;
        objectInHandRB.angularDrag = defaultAngularDrag;
        objectInHandRB.useGravity = true;

        //Reset player hand and stats
        gameObject.transform.position = gameObject.transform.position - gameObject.transform.forward * objDistanceBySize;
        carryingObject = false;
        if (carryingHeavyObject)
        {
            playerMovement.playerStats.walkSpeed = defaultPlayerWalkSpeed;
            playerMovement.playerStats.setCanRun(true);
        }
        carryingHeavyObject = false;
    }

    void centerHandOject()
    {
        Vector3 movementVector = Vector3.MoveTowards(objectInHand.transform.position, gameObject.transform.position, 1f);

        if (Vector3.Distance(objectInHand.transform.position, movementVector) > 0.001f)
        {
            objectInHandRB.drag = 40f;
            objectInHandRB.angularDrag = 40f;
            objectInHandRB.AddForce((movementVector - objectInHand.transform.position) * objectInHandRB.drag * 9f, ForceMode.Force);
        }
        else
        {
            objectInHandRB.drag = defaultDrag;
            objectInHandRB.angularDrag = defaultAngularDrag;
            stopObjectForces();
        }
    }

    Vector3 getVelocityDirection()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 xDirection = objectInHand.transform.position + mainCamera.transform.forward + (mainCamera.transform.right * mouseX);
        Vector3 yDirection = objectInHand.transform.position + mainCamera.transform.forward + (mainCamera.transform.up * mouseY);
        Vector3 xyDirection = (xDirection + yDirection) / 2f;

        return xyDirection - objectInHand.transform.position;
    }

    Vector3 getVelocity()
    {
        Vector3 velocity = (objectInHand.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = objectInHand.transform.position;

        return velocity;
    }

    void stopObjectForces()
    {
        objectInHandRB.velocity = Vector3.zero;
        objectInHandRB.angularVelocity = Vector3.zero;
        objectInHandRB.Sleep();
    }

    void checkObjDrop()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 objPosition = objectInHand.transform.position;
        Vector3 handPosition = gameObject.transform.position;

        float objDistance = Vector3.Distance(objPosition, cameraPosition);
        float handDistance = Vector3.Distance(handPosition, cameraPosition);

        //Too far from player
        if (objDistance > PlayerStats.reachDistance + 0.45f)
        {
            dropObj();
        }

        //Object inbetween
        RaycastHit hitInfo1;
        RaycastHit hitInfo2;
        bool hit1 = Physics.Raycast(cameraPosition, objPosition - cameraPosition, out hitInfo1, objDistance, ~ignoredLayer, QueryTriggerInteraction.Ignore);
        bool hit2 = Physics.Raycast(cameraPosition, handPosition - cameraPosition, out hitInfo2, handDistance, ~ignoredLayer, QueryTriggerInteraction.Ignore);
        if (hit1 && hit2)
        {
            if (!hitInfo1.transform.gameObject.Equals(objectInHand) && !hitInfo2.transform.gameObject.Equals(objectInHand))
            {
                dropObj();
            }
        }

        //Object under player
        if (checkIfObjUnderPlayer())
        {
            dropObj();
        }

        //Too far off screen
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (!GeometryUtility.TestPlanesAABB(planes, objectInHand.GetComponent<Collider>().bounds))
        {
            dropObj();
        }
    }

    bool checkIfObjUnderPlayer()
    {
        if(playerMovement.gameObjectUnderPlayer != null && playerMovement.gameObjectUnderPlayer == objectInHand)
        {
            return true;
        }

        return false;
    }
}

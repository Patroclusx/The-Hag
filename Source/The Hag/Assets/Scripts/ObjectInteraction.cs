using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public Transform player;
    public Camera mainCamera;
    public MouseLook mouseLook;
    public LayerMask layerMask;

    Transform defaultParent;
    float defaultDrag = 0f;
    float defaultAngularDrag = 0f;

    bool beingCarried = false;
    bool dropObject = false;
    GameObject objectInHand;
    Rigidbody objectInHandRB;

    void Start()
    {
        gameObject.transform.position = mainCamera.transform.position + mainCamera.transform.forward;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!beingCarried)
            {
                if (PlayerStats.canInteract && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    pickUpObject();
                }
            }
            else
            {
                carryObject();

                //Object rotation in hand
                if (Input.GetKey(KeyCode.R))
                {
                    mouseLook.isEnabled = false;

                    float mouseX = Input.GetAxis("Mouse X") * 2f;
                    float mouseY = Input.GetAxis("Mouse Y") * 2f;

                    objectInHand.transform.Rotate(mainCamera.transform.up, -mouseX, Space.World);
                    objectInHand.transform.Rotate(mainCamera.transform.right, mouseY, Space.World);
                }
                else if (Input.GetKeyUp(KeyCode.R))
                {
                    mouseLook.isEnabled = true;
                }
            }
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (beingCarried)
            {
                dropObj();
            }

            mouseLook.isEnabled = true;
            PlayerStats.canInteract = true;
        }
    }

    void pickUpObject()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, -1, QueryTriggerInteraction.Ignore))
        {
            objectInHand = hitInfo.transform.gameObject;
            objectInHandRB = objectInHand.GetComponent<Rigidbody>();

            if (objectInHand.tag == "Interactable" && objectInHandRB != null)
            {
                defaultParent = objectInHand.transform.parent;
                defaultDrag = objectInHandRB.drag;
                defaultAngularDrag = objectInHandRB.angularDrag;

                objectInHand.transform.parent = gameObject.transform;
                objectInHandRB.useGravity = false;
                stopObjectForces();

                dropObject = false;
                beingCarried = true;
                PlayerStats.canInteract = false;
            }
        }
    }

    void carryObject()
    {
        //Try to move object to center of camera
        centerHandOject();

        //If object cannot be held anymore drop it
        if (dropObject)
        {
            dropObj();
        }
        else
        {
            checkObjDrop();
        }

        //Throw object
        if (Input.GetButtonDown("Fire2"))
        {
            dropObj();
            objectInHandRB.AddForce(mainCamera.transform.forward * PlayerStats.throwForce);
        }
    }

    void dropObj()
    {
        objectInHand.transform.parent = defaultParent;
        objectInHandRB.drag = defaultDrag;
        objectInHandRB.angularDrag = defaultAngularDrag;
        objectInHandRB.useGravity = true;

        beingCarried = false;
        dropObject = false;
    }

    void centerHandOject()
    {
        Vector3 movementVector = Vector3.MoveTowards(objectInHand.transform.position, gameObject.transform.position, 1f);

        if (Vector3.Distance(objectInHand.transform.position, movementVector) > 0f)
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

        //Too far from player
        float dist = Vector3.Distance(objPosition, cameraPosition);
        if (dist > PlayerStats.reachDistance + 0.25f)
        {
            dropObject = true;
        }

        //Impact too high
        //TODO

        //Object inbetween
        RaycastHit hitInfo;
        if (Physics.Raycast(cameraPosition, objPosition - cameraPosition, out hitInfo, PlayerStats.reachDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (!hitInfo.transform.gameObject.Equals(objectInHand)) 
            {
                dropObject = true;
            }
        }

        //Too far off screen
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(objPosition);
        if (screenPoint.x < 0.2f || screenPoint.x > 0.8f || screenPoint.y < 0.15f || screenPoint.y > 0.95f)
        {
            dropObject = true;
        }
    }
}

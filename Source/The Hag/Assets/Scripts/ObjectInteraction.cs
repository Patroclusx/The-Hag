using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public Transform player;
    public Camera mainCamera;
    public MouseLook mouseLook;

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

                objectInHandRB.useGravity = false;
                objectInHand.transform.parent = gameObject.transform;
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

        //If object too far drop it
        if (dropObject)
        {
            dropObj();
        }
        else
        {
            checkPlayerDistance();
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

    public void OnCollisionEnterCustom(Collision collision, GameObject senderObject)
    {
        if (senderObject == objectInHand)
        {
            //TODO: Play impact sound
            //      Drop item on big impact
        }
    }

    void checkPlayerDistance()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 objPosition = objectInHand.transform.position;
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(objPosition);

        float dist = Vector3.Distance(objPosition, cameraPosition);
        if (dist > PlayerStats.reachDistance + 0.2f)
        {
            dropObject = true;
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(cameraPosition, objPosition- cameraPosition, out hitInfo, PlayerStats.reachDistance, -1, QueryTriggerInteraction.Ignore))
        {
            if (!hitInfo.transform.gameObject.Equals(objectInHand)) 
            { 
                dropObject = true;
            }
        }

        if(screenPoint.x < 0.2f || screenPoint.x > 0.8f || screenPoint.y < 0.15f || screenPoint.y > 0.95f)
        {
            dropObject = true;
        }
    }
}

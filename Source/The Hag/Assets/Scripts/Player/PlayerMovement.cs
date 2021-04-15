using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    public PlayerStats playerStats;
    public AudioManager audioManager;

    private Vector3 defaultCameraPosition;

    [HideInInspector]
    public bool isWalking = false;
    [HideInInspector]
    public bool isRunning = false;
    [HideInInspector]
    public float playerSpeed;

    [HideInInspector]
    public Vector3 moveVelocity;

    [HideInInspector]
    public bool isJumping = false;
    [HideInInspector]
    public bool hasJumped = false;
    [Header("Movement Attributes")]
    public float gravityForce = -25f;
    public float jumpHeight = 0.8f;
    [HideInInspector]
    public Vector3 verticalVelocity;

    [HideInInspector]
    public bool isCrouching = false;
    public float crouchHeight = 1.1f;

    [HideInInspector]
    public bool isGrounded;
    public LayerMask groundMask;
    float defaultStepOffset;

    [HideInInspector]
    public bool isClimbing = false;

    private void Start()
    {
        defaultCameraPosition = Camera.main.transform.position;
        defaultStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = characterController.isGrounded;

        //Movement logic 
        bool move = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
        if (move && !isClimbing)
        {
            MovePlayer();
        }
        else
        {
            moveVelocity.Set(0f, 0f, 0f);
            playerSpeed = 0f;
            isWalking = false;
            isRunning = false;
        }

        //Climbing logic
        if (move && isClimbing)
        {
            Climb();
        }

        //Crouching logic
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isClimbing)
        {
            ToggleCrouch();
        }

        //Jumping logic
        if (Input.GetKeyDown(KeyCode.Space) && !hasJumped && !isCrouching && !isClimbing)
        {
            Jump();
        }

        //Gravity logic
        if (!isClimbing)
        {
            ApplyGravity();
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity.y += gravityForce * Time.deltaTime;

            if (hasJumped)
            {
                isJumping = true;
            }
        }
        else
        {
            if(verticalVelocity.y < -4.4f)
            {
                PlayImpactSound();
            }
            if (verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -4f;
            }

            if (isJumping)
            {
                characterController.stepOffset = defaultStepOffset;
                hasJumped = false;
                isJumping = false;
            }
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (isGrounded)
        {
            moveVelocity = gameObject.transform.right * x + gameObject.transform.forward * z;

            //Strafe running speed fix
            if (x != 0f && z != 0f)
            {
                moveVelocity *= 0.75f;
            }
        }

        if (!checkForWallHit() || hasJumped)
        {
            //Sprinting logic (An Ultimatees original comment)
            if (Input.GetKey(KeyCode.LeftShift) && z >= 0.5f && !isCrouching && playerStats.canRun)
            {
                playerSpeed = playerStats.sprintSpeed;
                isRunning = true;
                isWalking = false;
            }
            else
            {
                if (!isCrouching)
                {
                    playerSpeed = playerStats.walkSpeed;
                }
                else
                {
                    playerSpeed = playerStats.walkSpeed * 0.5f;
                }

                isWalking = true;
                isRunning = false;
            }

            characterController.Move(moveVelocity * playerSpeed * Time.deltaTime);
            PlayStepSound();
        }
        else
        {
            isRunning = false;
            isWalking = false;
        }
    }

    public bool isPlayerMoving()
    {
        return isWalking || isRunning;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        pushRigidBodyObjects(hit);
    }

    void pushRigidBodyObjects(ControllerColliderHit hit)
    {
        Rigidbody objectBody = hit.collider.attachedRigidbody;

        // Not a rigidbody
        if (objectBody == null || objectBody.isKinematic)
            return;

        // We dont want to push heavy objects or objects below us
        if (hit.moveDirection.y < -0.3f || objectBody.mass >= 5f)
            return;

        // Calculate push direction from move direction,
        // We only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Apply the push
        objectBody.velocity = pushDir * 2f;
    }

    //Returns true if player is walking into a wall
    bool checkForWallHit()
    {
        float rayHeightOffset = 0.1f;
        float rayLenghtOffset = 0.14f;
        Vector3 checkPosition = gameObject.transform.position - new Vector3(0f, characterController.bounds.extents.y - characterController.stepOffset - rayHeightOffset, 0f);

        RaycastHit rayHit;
        bool checkWall = Physics.Raycast(checkPosition, moveVelocity, out rayHit, (characterController.radius / 2f) + rayLenghtOffset, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        return checkWall;
    }

    void Climb()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        playerSpeed = playerStats.climbSpeed;

        if (isCrouching)
        {
            ToggleCrouch();
            ApplyGravity();
        }

        moveVelocity = gameObject.transform.up * z + gameObject.transform.right * x;
        if ((!isGrounded && z > 0f) || (isGrounded && z < 0f))
            moveVelocity += gameObject.transform.forward * z;

        characterController.Move(moveVelocity * playerSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (playerStats.canJump)
        {
            Ray ray = new Ray();
            ray.origin = gameObject.transform.position;
            ray.direction = Vector3.up;
            if (!Physics.Raycast(ray, characterController.height - 1.1f, -1, QueryTriggerInteraction.Ignore))
            {
                characterController.stepOffset = 0f;
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -1f * gravityForce);
                hasJumped = true;

                audioManager.playCollectionSound3D("Sound_Player_Jump", true, 0f, gameObject);
            }
        }
    }

    void ToggleCrouch()
    {
        if (!isCrouching)
        {
            Camera.main.transform.localPosition = new Vector3(0f, Camera.main.transform.localPosition.y - (characterController.height - crouchHeight) * 0.5f, -0.072f);
            characterController.height = crouchHeight;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y * 0.5f, gameObject.transform.position.z);
            isCrouching = true;

            audioManager.playCollectionSound2D("Sound_Player_Crouch", true, 0f);
        }
        else
        {
            Ray ray = new Ray();
            ray.origin = gameObject.transform.position;
            ray.direction = Vector3.up;
            if (!Physics.Raycast(ray, characterController.height - 0.05f, -1, QueryTriggerInteraction.Ignore))
            {
                Camera.main.transform.localPosition = defaultCameraPosition; //Default camera pos
                characterController.height = 2f; //Default player height
                isCrouching = false;

                audioManager.playCollectionSound2D("Sound_Player_Crouch", true, 0f);
            }
        }

        Camera.main.GetComponent<HeadBobbing>().updateDefaultPosY(Camera.main.transform.localPosition.y);
    }

    void PlayStepSound()
    {
        if (isGrounded && moveVelocity.magnitude > 0.35f)
        {
            if (isWalking && !isCrouching)
            {
                audioManager.playCollectionSound3D("Sound_Step_Walk_Dirt", true, 0.45f, gameObject);
            }
            else if (isRunning)
            {
                audioManager.playCollectionSound3D("Sound_Step_Run_Dirt", true, 0.275f, gameObject);
            }

        }
    }

    void PlayImpactSound()
    {
        if (isGrounded) { 
            audioManager.playCollectionSound3D("Sound_Step_Walk_Dirt", false, 0f, gameObject);
        }
    }
}

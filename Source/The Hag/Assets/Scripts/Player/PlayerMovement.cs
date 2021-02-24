using System.Collections.Generic;
using UnityEngine;

/* TODO
 * Material based step sliding and impact sounds
 */

public class PlayerMovement : MonoBehaviour
{
    public Transform playerBody;
    public CharacterController characterController;
    public PlayerStats playerStats;
    public Transform groundCheck;
    public AudioManager audioManager;

    [HideInInspector]
    public bool isWalking = false;
    [HideInInspector]
    public bool isRunning = false;
    [HideInInspector]
    public float playerSpeed;
    public float walkSpeed = 2f;
    public float sprintSpeed = 4f;
    public float climbSpeed = 1f;

    [HideInInspector]
    public Vector3 moveVelocity;

    [HideInInspector]
    public bool isJumping = false;
    [HideInInspector]
    public bool hasJumped = false;
    public float gravityForce = -25f;
    public float jumpHeight = 0.8f;
    [HideInInspector]
    public Vector3 verticalVelocity;

    [HideInInspector]
    public bool isCrouching = false;
    public float crouchHeight = 1.1f;

    [HideInInspector]
    public bool isGrounded;
    float groundDistanceNormal = 0.35f;
    public LayerMask groundMask;
    float defaultStepOffset;

    [HideInInspector]
    public bool isSliding = false;
    [HideInInspector]
    public float slopeSpeed = 0;

    [HideInInspector]
    public bool isClimbing = false;

    private void Start()
    {
        defaultStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        //Ground check logic
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistanceNormal, groundMask, QueryTriggerInteraction.Ignore);

        //Movement logic 
        bool isMoving = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
        if (isMoving && !isClimbing && !isSliding)
        {
            MovePlayer();
            PlayStepSound();
        }
        else
        {
            playerSpeed = 0f;
            isWalking = false;
            isRunning = false;
        }

        //Climbing logic
        if (Input.GetAxis("Vertical") != 0 && isClimbing)
        {
            Climb();
        }

        //Crouching logic
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isClimbing)
        {
            ToggleCrouch();
        }

        //Jumping logic
        if (Input.GetKeyDown(KeyCode.Space) && !hasJumped && !isCrouching && !isSliding && !isClimbing)
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
            if(verticalVelocity.y < -4f)
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

            Slide();
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    private void Slide()
    {
        float slideSpeed = 10f;
        RaycastHit groundHit;

        bool groundRayCast = Physics.Raycast(playerBody.position, Vector3.down, out groundHit, 1.2f, -1, QueryTriggerInteraction.Ignore);
        Vector3 groundNormal = groundHit.normal;

        Vector3 groundParallel = Vector3.Cross(playerBody.up, groundNormal);
        Vector3 slopeParallel = Vector3.Cross(groundParallel, groundNormal);

        float currentSlope = Mathf.Round(Vector3.Angle(groundNormal, playerBody.up));

        if (isGrounded && groundRayCast && currentSlope > characterController.slopeLimit)
        {
            slopeSpeed += Time.deltaTime * (slopeParallel.magnitude + (isSliding ? 0f : (moveVelocity.magnitude * 80f))) * slideSpeed;
            characterController.Move(slopeParallel * slopeSpeed * Time.deltaTime);
            audioManager.playSound3D("Sound_Player_Slide", false, 0f, gameObject);
            isSliding = true;
        }
        else
        {
            if (isSliding)
            {
                audioManager.fadeOutSound3D("Sound_Player_Slide", 0.35f, gameObject);
                slopeSpeed = 0;
                isSliding = false;
            }
        }
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Sprinting logic
        if (Input.GetKey(KeyCode.LeftShift) && z == 1 && !isCrouching && playerStats.canRun)
        {
            playerSpeed = sprintSpeed;
            isRunning = true;
            isWalking = false;
        }
        else
        {
            if (!isCrouching)
            {
                playerSpeed = walkSpeed;
            }
            else
            {
                playerSpeed = walkSpeed * 0.5f;
            }

            isWalking = true;
            isRunning = false;
        }

        if (isGrounded)
        {
            moveVelocity = playerBody.right * x + playerBody.forward * z;
        }

        //Strafe running speed fix
        if (x != 0 && z != 0)
        {
            playerSpeed *= 0.75f;
        }

        characterController.Move(moveVelocity * playerSpeed * Time.deltaTime);
    }

    void Climb()
    {
        float y = Input.GetAxis("Vertical");
        playerSpeed = climbSpeed;

        if (isCrouching)
        {
            ToggleCrouch();
            ApplyGravity();
        }

        if (!isGrounded)
        {
            if (y > 0f)
            {
                moveVelocity = playerBody.up * y + playerBody.forward * y;
            }
            else
            {
                moveVelocity = playerBody.up * y;
            }
        }
        else
        {
            if(y > 0f)
            {
                moveVelocity = playerBody.up * y;
            }
            else
            {
                moveVelocity = playerBody.up * y + playerBody.forward * y;
            }
        }

        characterController.Move(moveVelocity * playerSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (playerStats.canJump)
        {
            Ray ray = new Ray();
            RaycastHit hit;
            ray.origin = playerBody.position;
            ray.direction = Vector3.up;
            if (!Physics.Raycast(ray, out hit, characterController.height - 1.2f, -1, QueryTriggerInteraction.Ignore))
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
            groundCheck.localPosition = new Vector3(0f, groundCheck.localPosition.y + (characterController.height - crouchHeight) * 0.5f, 0f);
            characterController.height = crouchHeight;
            playerBody.position = new Vector3(playerBody.position.x, playerBody.position.y * 0.5f, playerBody.position.z);
            isCrouching = true;

            audioManager.playCollectionSound2D("Sound_Player_Crouch", true, 0f);
        }
        else
        {
            Ray ray = new Ray();
            RaycastHit hit;
            ray.origin = playerBody.position;
            ray.direction = Vector3.up;
            if (!Physics.Raycast(ray, out hit, characterController.height - 0.1f, -1, QueryTriggerInteraction.Ignore))
            {
                groundCheck.localPosition = new Vector3(0f, -0.55f, 0f); //Default ground check Y
                characterController.height = 2f;
                isCrouching = false;

                audioManager.playCollectionSound2D("Sound_Player_Crouch", true, 0f);
            }
        }
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
            audioManager.playCollectionSound3D("Sound_Step_Walk_Dirt", true, 0f, gameObject);
        }
    }
}

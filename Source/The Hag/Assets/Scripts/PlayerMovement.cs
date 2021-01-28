﻿using System.Collections.Generic;
using UnityEngine;

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
    float groundDistance = 0.375f;
    public LayerMask groundMask;
    float defaultStepOffset;

    [HideInInspector]
    public bool isSliding = false;
    [HideInInspector]
    public float slopeSpeed = 0;
    RaycastHit slopeHit;
    Vector3 slopeParallel;

    List<GameObject> unityGameObjects = new List<GameObject>();

    private void Start()
    {
        defaultStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        //Ground check logic
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //Movement logic 
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            MovePlayer();
            StepSound();
        }
        else
        {
            playerSpeed = 0f;
            isWalking = false;
            isRunning = false;
        }

        //Crouching logic
        if (Input.GetButtonDown("Crouch") /**&& Input.GetButtonUp("Crouch")**/ && isGrounded)
        {
            Crouch();
        }

        //Jumping logic
        if (Input.GetButtonDown("Jump") && !hasJumped && !isCrouching && !isSliding)
        {
            if (playerStats.canJump)
            {
                Jump();
            }
        }

        //Gravity logic
        ApplyGravity();
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
            if (verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -4f;
            }

            SlopeSlide();

            if (isJumping)
            {
                characterController.stepOffset = defaultStepOffset;
                hasJumped = false;
                isJumping = false;
            }
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    private void SlopeSlide()
    {
        Physics.Raycast(playerBody.position, Vector3.down, out slopeHit, 1f);
        Vector3 n = slopeHit.normal;

        Vector3 groundParallel = Vector3.Cross(playerBody.up, n);
        slopeParallel = Vector3.Cross(groundParallel, n);

        float currentSlope = Mathf.Round(Vector3.Angle(n, playerBody.up));

        if (currentSlope > characterController.slopeLimit && isGrounded && (slopeHit.collider.gameObject.layer == LayerMask.NameToLayer("Slope")))
        {
            slopeSpeed += Time.deltaTime * (slopeParallel.magnitude + (isSliding ? 0f : (moveVelocity.magnitude * 80f))) * 12f;
            characterController.Move(slopeParallel * slopeSpeed * Time.deltaTime);
            isSliding = true;
        }
        else
        {
            slopeSpeed = 0;
            isSliding = false;
        }
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Sprinting logic
        if (Input.GetButton("Sprint") && z == 1 && !isCrouching && playerStats.canRun)
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

        if (isSliding)
        {
            x = 0;
            z = 0;
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

    void Jump()
    {
        characterController.stepOffset = 0f;
        verticalVelocity.y = Mathf.Sqrt(jumpHeight * -1f * gravityForce);
        hasJumped = true;

        audioManager.playCollectionSound3D("Sound_Player_Jump", true, 0f, gameObject);
    }

    void Crouch()
    {
        if (!isCrouching)
        {
            groundCheck.localPosition = new Vector3(0f, groundCheck.localPosition.y + (characterController.height - crouchHeight) / 2, 0f);
            characterController.height = crouchHeight;
            isCrouching = true;
        }
        else
        {
            Ray ray = new Ray();
            RaycastHit hit;
            ray.origin = playerBody.position;
            ray.direction = Vector3.up;
            if (!Physics.Raycast(ray, out hit, characterController.height - 0.1f))
            {
                groundCheck.localPosition = new Vector3(0f, -0.6f, 0f);
                characterController.height = 2f;
                isCrouching = false;
            }
        }

        audioManager.playCollectionSound2D("Sound_Player_Crouch", true, 0f);
    }

    void StepSound()
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
}

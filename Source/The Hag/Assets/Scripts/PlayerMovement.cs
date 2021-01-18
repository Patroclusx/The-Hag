using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform playerBody;
    public CharacterController characterController;
    public Transform groundCheck;

    [HideInInspector]
    public float playerSpeed;
    public float walkSpeed = 3f;
    public float sprintSpeed = 5f;
    [HideInInspector]
    public bool isWalking = false;
    [HideInInspector]
    public bool isRunning = false;

    [HideInInspector]
    public Vector3 moveVelocity;

    public float gravityForce = -25f;
    public float jumpHeight = 2.5f;
    [HideInInspector]
    public Vector3 jumpVelocity;

    [HideInInspector]
    public bool isCrouching = false;
    public float crouchScaleY;

    [HideInInspector]
    public bool isGrounded;
    float groundDistance = 0.3f;
    public LayerMask groundMask;

    // Update is called once per frame
    void Update()
    {
        //Ground check logic
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //Movement logic 
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
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
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            Jump();
        }

        //Gravity logic
        ApplyGravity();
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            jumpVelocity.y += gravityForce * Time.deltaTime;
        }
        else
        {
            if (jumpVelocity.y < 0f)
            {
                jumpVelocity.y = -4f;
            }
        }

        characterController.Move(jumpVelocity * Time.deltaTime);
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Sprinting logic
        if (Input.GetButton("Sprint") && z == 1 && !isCrouching)
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

        if (isGrounded || isCrouching)
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
        jumpVelocity.y = Mathf.Sqrt(jumpHeight * -1f * gravityForce);
    }

    void Crouch()
    {
        if (!isCrouching)
        {
            playerBody.localScale = new Vector3(playerBody.localScale.x, crouchScaleY, playerBody.localScale.z);
            isCrouching = true;
        }
        else
        {
            Ray ray = new Ray();
            RaycastHit hit;
            ray.origin = playerBody.position;
            ray.direction = Vector3.up;
            if (!Physics.Raycast(ray, out hit, 1.7f))
            {
                playerBody.localScale = new Vector3(playerBody.localScale.x, 1.1f, playerBody.localScale.z);
                isCrouching = false;
            }
        }
    }

    void StepSound()
    {
        if (isGrounded && moveVelocity.magnitude > 0.6f && !GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}

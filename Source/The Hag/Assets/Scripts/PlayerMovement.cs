using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController characterController;

    [System.NonSerialized]
    public float speed;
    [System.NonSerialized]
    public float gravity = -15f;
    [System.NonSerialized]
    public float jumpHeight = 2f;
    [System.NonSerialized]
    public float walk = 5f;
    [System.NonSerialized]
    public float sprint = 10f;
   

    Vector3 moveVelocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        

        moveVelocity = transform.right * x + transform.forward * z;

        characterController.Move(moveVelocity * speed * Time.deltaTime);
         //Sprinting logic
        if (Input.GetButton("Sprint") && z == 1 && isGrounded)
        {
            speed = sprint;
           
        }
        else
        {
            speed = walk;
            
        }
        //Jumping logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -1f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        if (isGrounded && moveVelocity.magnitude > 0.6f && !GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}

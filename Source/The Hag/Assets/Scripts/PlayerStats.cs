using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public AudioManager audioManager;

    //Stamina
    public float playerStamina = 100f;
    public float staminaChangeSpeed = 15f;
    [HideInInspector]
    public bool canRun = true;
    [HideInInspector]
    public bool canJump = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        handleStamina(staminaChangeSpeed);
    }

    void handleStamina(float staminaSpeed)
    {
        if (playerStamina < 40f)
        {
            audioManager.playCollectionSound2D("Sound_Player_Breath", false, 0f);
        }

        if (playerMovement.isRunning && canRun)
        {
            if (playerStamina > 0f)
            {
                if (!playerMovement.isJumping)
                {
                    playerStamina -= Time.deltaTime * staminaSpeed;
                }
            }
            else
            {
                playerStamina = 0f;
                canRun = false;
            }
        }
        if (playerMovement.isJumping && canJump)
        {
            if (playerStamina >= 10f)
            {
                playerStamina -= 10f;
                canJump = false;
            }
            else
            {
                playerStamina = 0f;
                canJump = false;
            }
        }

        if (playerStamina < 100f)
        {
            //Stamina recover
            if (!playerMovement.isRunning && !playerMovement.isJumping)
            {
                playerStamina += Time.deltaTime * (staminaSpeed * 0.8f);
                if (playerStamina > 100f)
                {
                    playerStamina = 100f;
                }
            }

            //Run recover
            if (playerStamina >= 30f && !playerMovement.isRunning)
            {
                canRun = true;
            }

            //Jump recover
            if (playerStamina >= 10f && !playerMovement.isJumping)
            {
                canJump = true;
            }
        }
    }
}

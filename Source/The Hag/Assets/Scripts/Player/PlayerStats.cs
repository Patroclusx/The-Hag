using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public AudioManager audioManager;

    //Interaction
    [System.NonSerialized]
    public static bool canInteract = true;
    [System.NonSerialized]
    public static float reachDistance = 1.1f;
    [System.NonSerialized]
    public static float throwForce = 180f;

    [Header("Player Speed")]
    //Speed
    public float walkSpeed = 2f;
    public float sprintSpeed = 4f;
    public float climbSpeed = 1f;

    [Header("Player Stamina")]
    //Stamina
    public bool isStaminaDrainEnabled = true;
    public float playerStamina = 100f;
    public float staminaChangeSpeed = 15f;
    public float adrenalineModifier = 2f;
    [HideInInspector]
    public bool isAdrenalineOn = false;
    [HideInInspector]
    public bool canRun = true;
    [HideInInspector]
    public bool canJump = true;

    bool canRecoverRun = true;
    bool canRecoverJump = true;

    // Update is called once per frame
    void Update()
    {
        float staminaSpeed = staminaChangeSpeed;
        if (isAdrenalineOn)
        {
            staminaSpeed = staminaChangeSpeed / adrenalineModifier;
        }
        handleStamina(staminaSpeed);
    }

    void handleStamina(float staminaSpeed)
    {
        if (playerStamina < 40f)
        {
            audioManager.playCollectionSound2D("Sound_Player_Breath", false, 0f);
        }

        if (isStaminaDrainEnabled)
        {
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
                    playerStamina -= staminaSpeed;
                    canJump = false;
                }
                else
                {
                    playerStamina = 0f;
                    canJump = false;
                }
            }
        }

        if (playerStamina < 100f)
        {
            //Stamina recover
            if (!playerMovement.isRunning && !playerMovement.isJumping)
            {
                playerStamina += Time.deltaTime * staminaChangeSpeed * adrenalineModifier;
                if (playerStamina > 100f)
                {
                    playerStamina = 100f;
                }
            }

            //Run recover
            if (playerStamina >= staminaSpeed * 2f && !playerMovement.isRunning)
            {
                if(canRecoverRun)
                    canRun = true;
            }

            //Jump recover
            if (playerStamina >= staminaSpeed && !playerMovement.isJumping)
            {
                if(canRecoverJump)
                    canJump = true;
            }
        }
    }

    public void setCanRun(bool canRun)
    {
        this.canRun = canRun;
        canRecoverRun = canRun;
    }

    public void setCanJump(bool canJump)
    {
        this.canJump = canJump;
        canRecoverJump = canJump;
    }
}

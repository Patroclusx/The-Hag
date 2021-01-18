using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    public float walkingBobbingSpeed = 14f;
    public float runningBobbingSpeed = 18f;
    public float bobbingAmount = 0.05f;
    public PlayerMovement playerMovement;

    float defaultPosY = 0.74f;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(playerMovement.moveVelocity.x) > 0.4f || Mathf.Abs(playerMovement.moveVelocity.z) > 0.4f)
        {
            //Player is moving
            timer += Time.deltaTime * (playerMovement.isRunning ? runningBobbingSpeed : walkingBobbingSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), transform.localPosition.z);
        }
    }
}

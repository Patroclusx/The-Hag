using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public float bobbingAmount = 0.04f;
    float walkingBobbingSpeed = 12.5f;
    float runningBobbingSpeed = 18f;

    float defaultPosX;
    float defaultPosY;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosX = transform.localPosition.x;
        defaultPosY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if ((playerMovement.isWalking || playerMovement.isRunning) && (Mathf.Abs(playerMovement.moveVelocity.x) > 0.35f || Mathf.Abs(playerMovement.moveVelocity.z) > 0.35f))
        {
            //Player is moving
            timer += Time.deltaTime * (playerMovement.isRunning ? runningBobbingSpeed : walkingBobbingSpeed * playerMovement.playerSpeed * 0.5f);
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
            transform.localPosition = new Vector3(defaultPosX + Mathf.Sin(timer * 0.5f) * bobbingAmount, transform.localPosition.y, transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * runningBobbingSpeed), transform.localPosition.z);
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, defaultPosX, Time.deltaTime * runningBobbingSpeed), transform.localPosition.y, transform.localPosition.z);
        }
    }
}

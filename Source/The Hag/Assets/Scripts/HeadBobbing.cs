using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public float walkingBobbingSpeed = 12.5f;
    public float runningBobbingSpeed = 18f;
    public float bobbingAmount = 0.03f;

    float defaultPosY;
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
            timer += Time.deltaTime * (playerMovement.isRunning ? runningBobbingSpeed : walkingBobbingSpeed) * (playerMovement.isCrouching ? 0.5f : 1f);
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * runningBobbingSpeed), transform.localPosition.z);
        }
    }
}

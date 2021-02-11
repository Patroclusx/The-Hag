using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public ObjectInteraction objectInteraction;
    public MouseLook mouseLook;

    Quaternion originTransform;
    void Start()
    {
        originTransform = gameObject.transform.localRotation;
    }

    void Update()
    {
        if (mouseLook.isEnabled)
        {
            handSway();
        }
    }

    void handSway()
    {
        float upLimit = objectInteraction.carryingObject ? 75f : 40f;
        float downLimit = objectInteraction.carryingObject ? -40f : -50f;
        float xMouseRotation = -mouseLook.xRotation;
        float mouseX = Input.GetAxis("Mouse X") * mouseLook.mouseSens;

        if (xMouseRotation > upLimit)
        {
            //Looking upwards
            xMouseRotation -= upLimit;
        }
        else if (xMouseRotation < downLimit)
        {
            //Looking downwards
            xMouseRotation -= downLimit;
        }
        else
        {
            xMouseRotation = 0f;
        }

        Quaternion toTransform = originTransform * Quaternion.Euler(xMouseRotation, 0f, 0f) * Quaternion.AngleAxis(-mouseX * 1.5f, Vector2.up);
        gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, toTransform, Time.deltaTime * 4f);
    }
}

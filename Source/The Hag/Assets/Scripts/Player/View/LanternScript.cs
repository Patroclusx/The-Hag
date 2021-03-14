using UnityEngine;

public class LanternScript : MonoBehaviour
{
    public MouseLook mouseLook;

    // Update is called once per frame
    void Update()
    {
        float xMouseRotationHandle = Mathf.Clamp(mouseLook.xRotation, -20f, 50f);

        gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, Quaternion.Euler(xMouseRotationHandle, 0, 0), Time.deltaTime * 2f);
    }
}

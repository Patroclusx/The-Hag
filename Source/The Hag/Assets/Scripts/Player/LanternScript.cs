using UnityEngine;

public class LanternScript : MonoBehaviour
{
    public MouseLook mouseLook;
    public Transform childLantern;

    // Update is called once per frame
    void Update()
    {
        float xMouseRotationHandle = Mathf.Clamp(mouseLook.xRotation, -20f, 50f);
        //float xMouseRotationLantern = Mathf.Clamp(mouseLook.xRotation, -10f, 15f);

        gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, Quaternion.Euler(xMouseRotationHandle, 0, 0), Time.deltaTime * 2f);
        //childLantern.localRotation = Quaternion.Lerp(childLantern.localRotation, Quaternion.Euler(xMouseRotationLantern, 0, 0), Time.deltaTime * 3f);
    }
}

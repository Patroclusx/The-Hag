using UnityEngine;

public class LanternScript : MonoBehaviour
{
    public Transform player;
    public MouseLook mouseLook;

    Vector3 defaultRotation;

    // Start is called before the first frame update
    void Start()
    {
        defaultRotation = new Vector3(0f, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        float xRotation = Mathf.Clamp(mouseLook.xRotation, -5f, 25f);

        gameObject.transform.localRotation = Quaternion.Euler(xRotation - 90f, defaultRotation.y, defaultRotation.z);
    }
}

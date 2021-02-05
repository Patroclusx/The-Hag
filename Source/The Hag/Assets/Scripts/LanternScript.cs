using UnityEngine;

public class LanternScript : MonoBehaviour
{
    public Transform player;
    public MouseLook mouseLook;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.localRotation = Quaternion.Euler(mouseLook.xRotation - 90f, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z);
    }
}

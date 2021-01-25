using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator transition;
    public GameObject doorObject;
    public LayerMask doorMask;
    RaycastHit hitInfo;


    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 1.1f, doorMask))
        {
            if (Input.GetButtonDown("Fire1") && hitInfo.transform.gameObject.Equals(doorObject))
            {
                if ((transition.GetCurrentAnimatorStateInfo(0).IsName("DefaultState") && transition.GetBool("isClosedByDefault")) || !transition.GetCurrentAnimatorStateInfo(0).IsName("DoorOpen"))
                {
                    transition.SetTrigger("setOpen");
                }
                else
                {
                    transition.SetTrigger("setClose");
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 1.25f, doorMask))
        {
            if (Input.GetButtonDown("Interact") && hitInfo.transform.gameObject.Equals(doorObject))
            {
                if (transition.GetCurrentAnimatorStateInfo(0).IsName("DefaultState") || transition.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f){
                    transition.SetBool("isOpened", !transition.GetBool("isOpened"));
                }
            }
        }
    }
}

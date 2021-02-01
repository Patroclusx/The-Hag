using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorScript : MonoBehaviour
{
    public Animator transition;
    public GameObject doorObject;
    public LayerMask doorMask;
    public PointerEventData eventData;
    RaycastHit hitInfo;
   /* public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Debug.Log("double click");
        }
    }*/
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
            else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 1.25f, doorMask) && eventData.clickCount == 2)
            {
                Debug.Log("double click");
               
                if (Input.GetButtonDown("Interact") && hitInfo.transform.gameObject.Equals(doorObject))
                {
                    if (transition.GetCurrentAnimatorStateInfo(0).IsName("DefaultState") || transition.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        transition.SetBool("isOpened", !transition.GetBool("isOpened"));
                    }
                }

            }
        }
    }
}

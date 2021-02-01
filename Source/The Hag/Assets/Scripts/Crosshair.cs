using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public GameObject crosshair;
    public GameObject crosshairHand;

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, -1, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.transform.tag == "Interactable")
            {
                crosshairHand.SetActive(true);
                crosshair.SetActive(false);
            }
            else
            {
                crosshairHand.SetActive(false);
                crosshair.SetActive(true);
            }
        }
        else
        {
            crosshairHand.SetActive(false);
            crosshair.SetActive(true);
        }
    }
}

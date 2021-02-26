using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public GameObject crosshair;
    public GameObject crosshairHand;
    public LayerMask interactableLayers;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        bool raycast = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, interactableLayers, QueryTriggerInteraction.Collide);

        if (PlayerStats.canInteract && raycast && hitInfo.transform.tag == "Interactable")
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
}

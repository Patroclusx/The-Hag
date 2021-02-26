using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    GameObject itemInHand;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pickUpItem();
        }
    }

    void pickUpItem()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, LayerMask.GetMask("Item"), QueryTriggerInteraction.Collide))
        {
            itemInHand = hitInfo.transform.gameObject;

            if (itemInHand.tag == "Interactable")
            {
                if (Inventory.instance.hasSpace())
                {
                    Item item = itemInHand.GetComponent<ItemObjHolder>().item;
                    if (item != null)
                    {
                        Inventory.instance.addItem(itemInHand.GetComponent<ItemObjHolder>().item);
                        Destroy(itemInHand);
                    }
                    else
                    {
                        Debug.LogWarning("GameObject does not hold an Item object!");
                    }
                }
                else
                {
                    Debug.LogWarning("No space left in inventory!");
                }
            }
        }
    }
}

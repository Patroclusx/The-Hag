using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;

    GameObject itemInHand;
    Inventory inventory;

    void Start()
    {
        inventory = Inventory.instance;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Item selectedItem = inventory != null ? inventory.selectedItem : null;
            if (selectedItem != null)
            {
                RaycastHit hitInfo;
                bool rayHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
                selectedItem.use(rayHit ? hitInfo.transform.gameObject : null);

                inventory.selectedItem = null;
                PlayerStats.canInteract = true;
            }
            else
            {
                pickUpItem();
            }
        }
    }

    void pickUpItem()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, LayerMask.GetMask("Item"), QueryTriggerInteraction.Ignore))
        {
            itemInHand = hitInfo.transform.gameObject;

            if (itemInHand.tag == "Interactable")
            {
                if (Inventory.instance.hasSpace())
                {
                    ItemObjHolder itemObjHolder = itemInHand.GetComponent<ItemObjHolder>();
                    Item item = itemObjHolder != null ? itemObjHolder.item : null;
                    if (item != null)
                    {
                        Inventory.instance.addItem(item);
                        item.usableGameObjects = itemObjHolder.usableGameObjects;
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

using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Item item;
    public Image icon;

    public void addItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void selectItem()
    {
        if(item != null)
        {
            if (item.usableGameObjects.Count == 0)
            {
                item.use(null);
            }
            else
            {
                Inventory.instance.selectedItem = item;
            }
            GetComponentInParent<InventoryUI>().toggleInventory(true);
        }
    }
}

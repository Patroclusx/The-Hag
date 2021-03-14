using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;
    public MouseLook mouseLook;

    Inventory inventory;

    InventorySlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((PlayerStats.canInteract || mouseLook.isInInventory) && Input.GetKeyDown(KeyCode.E))
        {
            toggleInventory(false);
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(i < inventory.itemsList.Count)
            {
                slots[i].addItem(inventory.itemsList[i]);
            }
            else
            {
                slots[i].clearSlot();
            }
        }
    }

    public void toggleInventory(bool isItemSelected)
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        mouseLook.toggleInventoryCursor(isItemSelected);
    }
}

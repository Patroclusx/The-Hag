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
        if (!mouseLook.isInteracting && Input.GetKeyDown(KeyCode.E))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            mouseLook.toggleInventoryCursor();
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
        }
    }
}

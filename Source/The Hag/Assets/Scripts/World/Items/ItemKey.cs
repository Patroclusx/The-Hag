using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key", menuName = "Inventory/Key")]
public class ItemKey : Item
{
    public override bool use(GameObject hoveredObject)
    {
        if (base.use(hoveredObject))
        {
            DoorInteraction doorInteraction = hoveredObject.GetComponent<DoorInteraction>();
            if (doorInteraction != null)
            {
                doorInteraction.isLocked = false;
                Inventory.instance.removeItem(this);
                return true;
            }
        }

        return false;
    }
}

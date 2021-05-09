using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    [HideInInspector]
    public List<GameObject> usableGameObjects = new List<GameObject>();

    public virtual bool use(GameObject hoveredObject)
    {
        if(usableGameObjects.Count == 0)
        {
            //Item does not need a game object to be used on
            return true;
        }
        else
        {
            //Item needs a game object to be used on
            if(hoveredObject != null)
            {
                if (usableGameObjects.Contains(hoveredObject))
                {
                    return true;
                }
            }
        }

        return false;
    }
}

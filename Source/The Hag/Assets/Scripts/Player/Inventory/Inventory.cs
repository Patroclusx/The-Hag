using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<Item> itemsList = new List<Item>();

    public int maxSpace = 10;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    //Singleton instance
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
        }

        instance = this;
    }

    public void addItem(Item itemObject)
    {
        itemsList.Add(itemObject);

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    public Item getItem(int index)
    {
        if(itemsList.Count > index)
        {
            return itemsList[index];
        }
        else
        {
            Debug.Log("Invalid inventory item index!");
            return null;
        }
    }

    public bool hasSpace()
    {
        if (itemsList.Count < maxSpace)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

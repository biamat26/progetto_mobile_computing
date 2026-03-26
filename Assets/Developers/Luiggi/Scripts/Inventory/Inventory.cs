using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int maxSlots = 8;

    public event Action OnInventoryChanged;

    public bool AddItem(Item item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventario pieno!");
            return false;
        }
        items.Add(item);
        Debug.Log("Raccolto: " + item.itemName);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        OnInventoryChanged?.Invoke();
    }
}
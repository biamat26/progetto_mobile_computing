using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;           // trascina il Player qui
    public GameObject slotPrefab;         // trascina il Prefab Slot qui
    public Transform gridParent;          // trascina InventoryPanel qui

    private List<GameObject> slots = new List<GameObject>();

    private void Start()
    {
        inventory.OnInventoryChanged += UpdateUI;
    }

    private void UpdateUI()
    {
        // Distruggi tutti gli slot esistenti
        foreach (GameObject slot in slots)
            Destroy(slot);
        slots.Clear();

        // Ricrea uno slot per ogni item
        foreach (Item item in inventory.items)
        {
            GameObject newSlot = Instantiate(slotPrefab, gridParent);
            Image icon = newSlot.transform.Find("ItemIcon").GetComponent<Image>();
            icon.sprite = item.icon;
            icon.enabled = true;
            slots.Add(newSlot);
        }
    }
}
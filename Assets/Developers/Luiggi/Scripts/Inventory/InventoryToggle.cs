using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;

    private void Start()
    {
        inventoryPanel.SetActive(false); // chiuso di default
    }

    private void Update()
    {
        if (InputManager.ToggleInventory)
        {
            bool isOpen = inventoryPanel.activeSelf;
            inventoryPanel.SetActive(!isOpen);
        }
    }
}
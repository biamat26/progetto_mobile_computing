using UnityEngine;
using UnityEngine.InputSystem; // <-- Abbiamo aggiunto questa libreria!

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject customCursor;

    private void Start()
    {
        inventoryPanel.SetActive(false); 
        customCursor.SetActive(false);   
        Cursor.visible = false;          
    }

    private void Update()
    {
        // Controllo del clic con il NUOVO Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.visible = false;
        }

        if (InputManager.ToggleInventory)
        {
            bool isOpen = inventoryPanel.activeSelf;
            
            inventoryPanel.SetActive(!isOpen);
            customCursor.SetActive(!isOpen);
        }
    }
}
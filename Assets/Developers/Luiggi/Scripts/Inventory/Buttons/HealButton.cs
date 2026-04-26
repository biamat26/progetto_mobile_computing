using UnityEngine;

public class HealButton : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public void OnHealClicked()
    {
        InventorySystem inv = InventorySystem.Instance;
        int slot = inv.GetSelectedSlot();

        if (slot == -1) { Debug.Log("Nessuno slot selezionato!"); return; }

        ItemData item = inv.GetItem(slot);
        if (item == null) { Debug.Log("Slot vuoto!"); return; }

        if (item.itemType != ItemType.Heal) { Debug.Log("Non è una cura!"); return; }

        playerHealth.Heal(item.healAmount);
        inv.RemoveItem(slot);
    }
}
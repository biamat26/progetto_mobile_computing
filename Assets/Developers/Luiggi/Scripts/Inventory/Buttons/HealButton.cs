using UnityEngine;

public class HealButton : MonoBehaviour
{
    public PlayerHealth playerHealth;

    // --- NUOVE VARIABILI AUDIO AGGIUNTE ---
    [Header("Audio Cura")]
    public AudioSource audioSource;
    public AudioClip healSound;
    // --------------------------------------

    public void OnHealClicked()
    {
        InventorySystem inv = InventorySystem.Instance;
        int slot = inv.GetSelectedSlot();

        if (slot == -1) { Debug.Log("Nessuno slot selezionato!"); return; }

        ItemData item = inv.GetItem(slot);
        if (item == null) { Debug.Log("Slot vuoto!"); return; }

        if (item.itemType != ItemType.Heal) { Debug.Log("Non è una cura!"); return; }

        // --- AVVIO AUDIO CURA ---
        if (audioSource != null && healSound != null)
        {
            audioSource.PlayOneShot(healSound);
        }
        // ------------------------

        // Curalo
        playerHealth.Heal(item.healAmount);
        
        // Rimuove l'oggetto e spegne in automatico il quadratino verde!
        inv.RemoveItem(slot);
    }
}
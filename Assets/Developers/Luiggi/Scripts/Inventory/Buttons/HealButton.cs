using UnityEngine;

public class HealButton : MonoBehaviour
{
    // Ho rimosso la variabile "public PlayerHealth playerHealth"
    // Ora lo script lo trova da solo in automatico, a prova di crash e di riavvii!

    [Header("Audio Cura")]
    public AudioSource audioSource;
    public AudioClip healSound;

    public void OnHealClicked()
    {
        InventorySystem inv = InventorySystem.Instance;
        int slot = inv.GetSelectedSlot();

        if (slot == -1) { Debug.Log("Nessuno slot selezionato!"); return; }

        ItemData item = inv.GetItem(slot);
        if (item == null) { Debug.Log("Slot vuoto!"); return; }

        if (item.itemType != ItemType.Heal) { Debug.Log("Non è una cura!"); return; }

        // --- IL TRUCCO CHE RISOLVE IL BUG ---
        // Cerca il PlayerHealth "fresco" appena spawnato in questa scena
        PlayerHealth playerAttuale = Object.FindFirstObjectByType<PlayerHealth>();

        if (playerAttuale == null)
        {
            Debug.LogWarning("Nessun giocatore trovato nella scena per essere curato!");
            return;
        }
        // ------------------------------------

        // --- AVVIO AUDIO CURA ---
        if (audioSource != null && healSound != null)
        {
            audioSource.PlayOneShot(healSound);
        }
        // ------------------------

        // Curalo passando la quantità presa dall'oggetto
        playerAttuale.Heal(item.healAmount);
        
        // Rimuove l'oggetto e spegne in automatico il quadratino verde
        inv.RemoveItem(slot);
    }
}
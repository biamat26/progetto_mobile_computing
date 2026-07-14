using UnityEngine;

public class DropButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    // Rimuoviamo il [SerializeField] perché lo troveremo via codice
    private Transform playerTransform; 

    // --- NUOVE VARIABILI AUDIO AGGIUNTE QUI ---
    [Header("Audio")]
    [Tooltip("Inserisci qui il suono da riprodurre quando si rilascia un oggetto")]
    [SerializeField] private AudioClip dropSound;
    // ------------------------------------------

    public void OnDrop()
    {
        Debug.Log("[DROP] OnDrop chiamato dopo reboot!");

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("[DROP] Player trovato? " + (player != null ? player.name : "NULL"));

            if (player != null)
                playerTransform = player.transform;
            else
            {
                Debug.LogError("Player non trovato! Assicurati che l'oggetto Player abbia il tag 'Player'.");
                return;
            }
        }

        // --- RICHIAMO AUDIO AGGIUNTO QUI ---
        if (AudioManager.instance != null && dropSound != null)
        {
            AudioManager.instance.PlayClickSound(dropSound);
        }
        // -----------------------------------

        InventorySystem.Instance.DropSelected(itemPrefab, playerTransform.position);
    }
}
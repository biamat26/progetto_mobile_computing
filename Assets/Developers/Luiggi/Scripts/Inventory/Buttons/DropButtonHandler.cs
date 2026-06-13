using UnityEngine;

public class DropButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    
    // Rimuoviamo il [SerializeField] perché lo troveremo via codice
    private Transform playerTransform; 

    public void OnDrop()
    {
        // 1. Cerchiamo il Player dinamicamente se non lo abbiamo ancora trovato
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player non trovato! Assicurati che l'oggetto Player abbia il tag 'Player'.");
                return; // Interrompiamo la funzione per evitare errori
            }
        }

        // 2. Ora che siamo sicuri di avere il Player, procediamo con il Drop
        InventorySystem.Instance.DropSelected(itemPrefab, playerTransform.position);
    }
}
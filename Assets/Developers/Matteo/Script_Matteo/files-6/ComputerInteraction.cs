using UnityEngine;
using System; // Necessario per l'evento Action

/// <summary>
/// Mettilo sul GameObject dei computer.
/// Quando il player preme E vicino al computer, apre il WirePuzzleManager
/// oppure attiva l'evento della ALU se impostato come terminale speciale.
/// </summary>
public class ComputerInteraction : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private WirePuzzleManager puzzleManager;
    [SerializeField] private GameObject tooltipInteragisci;

    [Header("Impostazioni Terminale ALU")]
    [Tooltip("Spunta questa casella nell'Inspector se questo è il computer speciale della ALU")]
    [SerializeField] private bool isAluTerminal = false; 
    [SerializeField] private string documentoRichiesto = "DocumentoALU"; // Il nome/tag del file da cercare
    
    // L'evento che Matteo ascolterà per far partire le ondate
    public static event Action OnAluDefenseStarted;

    private bool playerVicino = false;
    private GameObject playerRef; // Ci salviamo il player per leggerne l'inventario
    private bool eventoAvviato = false; // Per evitare che il player faccia partire le ondate 2 volte

    void Awake()
    {
        if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
    }

    void Update()
    {
        if (playerVicino && Input.GetKeyDown(KeyCode.E))
        {
            // Se questo è il computer speciale della ALU e l'evento non è ancora partito
            if (isAluTerminal && !eventoAvviato)
            {
                GestisciInterazioneALU();
            }
            // Altrimenti, se è un computer normale (o l'evento ALU è già finito), apre il puzzle
            else if (puzzleManager != null && !puzzleManager.IsSolved())
            {
                puzzleManager.OpenPuzzle();
            }
        }
    }

    private void GestisciInterazioneALU()
{
    // Usiamo il Singleton Instance invece di GetComponent
    if (InventorySystem.Instance != null)
    {
        int indiceDocumento = -1;

        // Scorriamo i 16 slot per cercare il documento speciale
        for (int i = 0; i < 16; i++)
        {
            var item = InventorySystem.Instance.GetItem(i);
            
            // Attenzione: assumo che ItemData sia uno ScriptableObject o abbia un campo "name".
            // Se nel tuo ItemData la variabile del nome si chiama diversamente (es. "itemName"), cambiala qui sotto.
            if (item != null && item.name == documentoRichiesto) 
            {
                indiceDocumento = i;
                break; // Trovato! Interrompiamo la ricerca
            }
        }

        // Se l'indice non è più -1, significa che abbiamo trovato l'oggetto
        if (indiceDocumento != -1)
        {
            // 1. Rimuoviamo l'oggetto usando il suo indice
            InventorySystem.Instance.RemoveItem(indiceDocumento);
            eventoAvviato = true; 

            // 2. Messaggio nel terminale
            Debug.Log("<color=red>TERMINALE ALU:</color> Attenzione d'ora in poi ci saranno i virus che ti attaccheranno in 3 ondate diverse, preparati a salvare l'alu dall'infezione dei virus!");
            
            // 3. Nascondi il tooltip
            if (tooltipInteragisci) tooltipInteragisci.SetActive(false);

            // 4. Lancia l'evento per lo script dello spawner di Matteo
            OnAluDefenseStarted?.Invoke();
        }
        else
        {
            Debug.Log("Ti manca il documento per avviare i calcoli dell'ALU!");
        }
    }
}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        playerVicino = true;
        playerRef = other.gameObject; // Salviamo il riferimento al Player per accedere al suo inventario

        // Mostra il tooltip solo se il puzzle non è risolto o se l'evento ALU non è ancora stato avviato
        if (tooltipInteragisci)
        {
            if (isAluTerminal && !eventoAvviato) tooltipInteragisci.SetActive(true);
            else if (!isAluTerminal && puzzleManager != null && !puzzleManager.IsSolved()) tooltipInteragisci.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        playerVicino = false;
        playerRef = null;
        
        if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
    }
}
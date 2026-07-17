using UnityEngine;

public class CatButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject inventoryCanvas;

    [Header("Audio")]
    [Tooltip("Inserisci qui il suono da riprodurre quando si usa il comando cat")]
    [SerializeField] private AudioClip catSound;

    public void OnCat()
    {
        int selectedSlot = InventorySystem.Instance.GetSelectedSlot();
        if (selectedSlot == -1) return;

        ItemData item = InventorySystem.Instance.GetItem(selectedSlot);
        if (item == null) return;

        if (AudioManager.instance != null && catSound != null)
        {
            AudioManager.instance.PlayClickSound(catSound);
        }

        InventorySystem.Instance.DeselectCurrentSlot();

        // Chiudiamo l'inventario tramite il singleton InventoryToggle,
        // così rilascia correttamente la pausa richiesta all'apertura (tasto Q).
        if (InventoryToggle.Istanza != null)
            InventoryToggle.Istanza.HideInventory();
        else if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false); // fallback di sicurezza

        if (item.immagineDocumento != null)
        {
            if (DocumentViewer.Istanza != null)
            {
                DocumentViewer.Istanza.MostraImmagine(item);
            }
            else
            {
                Debug.LogError("Errore: DocumentViewer non è presente nella scena o è stato distrutto!");
            }
            return;
        }

        if (string.IsNullOrEmpty(item.contenuto))
        {
            TerminalManager.Istanza.MostraMessaggioLibero("> Errore: questo oggetto non è leggibile.");
            if (!TerminalManager.Istanza.isExpanded)
                TerminalManager.Istanza.ToggleTerminal();
            return;
        }

        string testo = "> cat " + item.itemName + "\n\n> Contenuto del documento:\n" + item.contenuto;

        // PRIMA impostiamo il messaggio...
        TerminalManager.Istanza.MostraMessaggioLibero(testo);

        // ...POI apriamo il terminale, che lo scriverà una sola volta, correttamente
        if (!TerminalManager.Istanza.isExpanded)
            TerminalManager.Istanza.ToggleTerminal();
    }
}
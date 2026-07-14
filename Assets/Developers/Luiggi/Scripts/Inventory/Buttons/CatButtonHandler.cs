using UnityEngine;

public class CatButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject inventoryCanvas;

    // --- NUOVE VARIABILI AUDIO AGGIUNTE QUI ---
    [Header("Audio")]
    [Tooltip("Inserisci qui il suono da riprodurre quando si usa il comando cat")]
    [SerializeField] private AudioClip catSound;
    // ------------------------------------------

    public void OnCat()
    {
        int selectedSlot = InventorySystem.Instance.GetSelectedSlot();
        if (selectedSlot == -1) return;

        ItemData item = InventorySystem.Instance.GetItem(selectedSlot);
        if (item == null) return;

        // --- RICHIAMO AUDIO AGGIUNTO QUI ---
        if (AudioManager.instance != null && catSound != null)
        {
            AudioManager.instance.PlayClickSound(catSound);
        }
        // -----------------------------------

        // --- SPEGNAMO LA SELEZIONE ---
        InventorySystem.Instance.DeselectCurrentSlot();

        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);

        // Se ha un'immagine, mostra quella — altrimenti usa il terminale come prima
        if (item.immagineDocumento != null)
        {
            DocumentViewer.Istanza.MostraImmagine(item);
            return;
        }

        if (string.IsNullOrEmpty(item.contenuto))
        {
            TerminalManager.Istanza.MostraMessaggioLibero("> Errore: questo oggetto non è leggibile.");
            return;
        }

        string testo = "> cat " + item.itemName + "\n\n> Contenuto del documento:\n" + item.contenuto;

        // Se il terminale è chiuso, aprilo
        if (!TerminalManager.Istanza.isExpanded)
            TerminalManager.Istanza.ToggleTerminal();

        TerminalManager.Istanza.MostraMessaggioLibero(testo);
    }
}
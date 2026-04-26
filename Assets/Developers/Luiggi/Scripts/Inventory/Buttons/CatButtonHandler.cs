using UnityEngine;

public class CatButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject inventoryCanvas;

    public void OnCat()
{
    int selectedSlot = InventorySystem.Instance.GetSelectedSlot();
    if (selectedSlot == -1) return;

    ItemData item = InventorySystem.Instance.GetItem(selectedSlot);
    if (item == null) return;

    if (inventoryCanvas != null)
        inventoryCanvas.SetActive(false);

    // Se ha un'immagine, mostra quella — altrimenti usa il terminale come prima
    if (item.immagineDocumento != null)
    {
        DocumentViewer.Istanza.MostraImmagine(item.immagineDocumento);
        return;
    }

    if (string.IsNullOrEmpty(item.contenuto))
    {
        TerminalManager.Istanza.MostraMessaggioLibero("> Errore: questo oggetto non è leggibile.");
        return;
    }

    string testo = "> cat " + item.itemName + "\n\n> Contenuto del documento:\n" + item.contenuto;
    TerminalManager.Istanza.MostraMessaggioLibero(testo);
    TerminalManager.Istanza.isExpanded = true;
    TerminalManager.Istanza.ToggleTerminal();
    TerminalManager.Istanza.ToggleTerminal();
}
}
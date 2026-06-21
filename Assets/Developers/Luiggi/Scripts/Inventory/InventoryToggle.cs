using UnityEngine;
using System.Collections;

public class InventoryToggle : MonoBehaviour
{
    [SerializeField] private GameObject inventoryCanvas;

    void Start()
    {
        inventoryCanvas.SetActive(false);
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.Q))
    {
        // Se il documento è aperto, lascialo gestire a DocumentViewer (sopra) e non toccare l'inventario
        if (DocumentViewer.Istanza != null && DocumentViewer.Istanza.gameObject.activeSelf)
            return;

        bool opening = !inventoryCanvas.activeSelf;
        inventoryCanvas.SetActive(opening);
        Time.timeScale = opening ? 0f : 1f;

        if (opening)
            StartCoroutine(RefreshAfterFrame());
    }
}

    IEnumerator RefreshAfterFrame()
{
    yield return new WaitForEndOfFrame();
    if (InventorySystem.Instance != null)
        InventorySystem.Instance.RefreshUI();
}
    public void HideInventory()
    {
        inventoryCanvas.SetActive(false);
    }
}
using UnityEngine;
using System.Collections;

public class InventoryToggle : MonoBehaviour
{
    private GameObject inventoryCanvas;

    void Start()
    {
        AggiornaRiferimento();
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);
    }

    void Update()
    {
        if (inventoryCanvas == null)
        {
            AggiornaRiferimento();
            if (inventoryCanvas == null) return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (DocumentViewer.Istanza != null && DocumentViewer.Istanza.gameObject.activeSelf)
                return;

            bool opening = !inventoryCanvas.activeSelf;
            inventoryCanvas.SetActive(opening);
            Time.timeScale = opening ? 0f : 1f;

            if (opening)
                StartCoroutine(RefreshAfterFrame());
        }
    }

    private void AggiornaRiferimento()
    {
        if (InventorySystem.Instance != null)
            inventoryCanvas = InventorySystem.Instance.inventoryRoot;
    }

    IEnumerator RefreshAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.RefreshUI();
    }

    public void HideInventory()
    {
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);
    }
}
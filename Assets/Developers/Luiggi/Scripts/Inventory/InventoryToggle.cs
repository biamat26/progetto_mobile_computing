using UnityEngine;
using System.Collections;

public class InventoryToggle : MonoBehaviour
{
    public static InventoryToggle Istanza;

    private GameObject inventoryCanvas;
    private bool pausaRichiestaDaMe = false;

    void Awake()
    {
        Istanza = this;
    }

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

            if (opening)
            {
                PauseManager.RequestPause();
                pausaRichiestaDaMe = true;
                StartCoroutine(RefreshAfterFrame());
            }
            else
            {
                if (pausaRichiestaDaMe)
                {
                    PauseManager.ReleasePause();
                    pausaRichiestaDaMe = false;
                }
            }
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

        if (pausaRichiestaDaMe)
        {
            PauseManager.ReleasePause();
            pausaRichiestaDaMe = false;
        }
    }
}
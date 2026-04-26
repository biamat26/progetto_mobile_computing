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
            bool opening = !inventoryCanvas.activeSelf;
            inventoryCanvas.SetActive(opening);

            Time.timeScale = opening ? 0f : 1f;  // ← aggiunta

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
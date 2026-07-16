using UnityEngine;
using UnityEngine.UI;

public class DocumentViewer : MonoBehaviour
{
    public static DocumentViewer Istanza;
    public Image immagine;
    private Vector2 dimensioneOriginale;
    private bool dimensioneSalvata = false;

    void Awake()
    {
        Istanza = this;
        gameObject.SetActive(false);
    }

    private bool AssicuratiImmagineValida()
    {
        if (immagine == null) immagine = GetComponentInChildren<Image>();
        if (immagine == null) return false;
        if (!dimensioneSalvata)
        {
            dimensioneOriginale = immagine.rectTransform.sizeDelta;
            dimensioneSalvata = true;
        }
        return true;
    }

    public void MostraImmagine(ItemData item)
    {
        if (!AssicuratiImmagineValida()) return;

        // --- NASCONDI HUD (sempre, mentre il documento e' aperto) ---
        ImpostaHUD(false);

        immagine.sprite = item.immagineDocumento;
        if (item.usaDimensioniSpeciali)
            immagine.rectTransform.sizeDelta = item.dimensioniSpeciali;
        else
            immagine.rectTransform.sizeDelta = dimensioneOriginale;

        gameObject.SetActive(true);
        PauseManager.RequestPause();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Chiudi()
    {
        Debug.Log("Chiudi() chiamato!");
        gameObject.SetActive(false);
        PauseManager.ReleasePause();

        // --- RIATTIVA HUD SOLO SE C'E' UN'ONDATA IN CORSO ---
        WaveManager wm = FindFirstObjectByType<WaveManager>();
        bool inBattaglia = wm != null && wm.AreWavesInProgress();
        ImpostaHUD(inBattaglia);
    }

    private void ImpostaHUD(bool visibile)
    {
        WaveManager wm = FindFirstObjectByType<WaveManager>();
        if (wm != null && wm.hudContainer != null)
        {
            CanvasGroup cg = wm.hudContainer.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = visibile ? 1f : 0f;
                cg.blocksRaycasts = visibile;
            }
        }
    }
}
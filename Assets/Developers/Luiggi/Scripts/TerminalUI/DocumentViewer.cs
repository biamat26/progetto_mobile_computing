using UnityEngine;
using UnityEngine.UI;

public class DocumentViewer : MonoBehaviour
{
    public static DocumentViewer Istanza;
    public Image immagine;
    private Vector2 dimensioneOriginale;
    private bool dimensioneSalvata = false;

    private int frameApertura = -1; // NUOVO: per ignorare il click dello stesso frame di apertura

    void Awake()
    {
        Istanza = this;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        // Ignora il click sul frame stesso in cui si e' aperto il popup
        if (Time.frameCount == frameApertura) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
        {
            Chiudi();
        }
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
        frameApertura = Time.frameCount; // NUOVO: registra il frame di apertura
        PauseManager.RequestPause();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Chiudi()
    {
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
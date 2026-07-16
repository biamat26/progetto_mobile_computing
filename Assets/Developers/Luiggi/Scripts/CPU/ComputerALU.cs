using UnityEngine;
using System.Collections;

public class ComputerALU : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private GameObject popupConDocumento;
    [SerializeField] private WaveManager waveManager;

    [Header("Audio Popup")]
    [Tooltip("Trascina qui l'AudioSource che contiene il suono da riprodurre durante il popup.")]
    [SerializeField] private AudioSource audioSourcePopup;
    [Tooltip("Tempo in secondi della sfumatura (fade in/out) del suono.")]
    [SerializeField] private float tempoSfumatura = 0.5f;

    [Header("Documento con Password (fine ondate)")]
    [SerializeField] private GameObject documentoConPassword;
    [SerializeField] private Transform puntoUscitaDocumento;

    [Header("Documento richiesto")]
    [SerializeField] private string documentoRichiesto = "DocumentoALU";

    private bool playerVicino = false;
    private bool eventoAvviato = false;
    private Coroutine fadeCoroutine;

    void Update()
    {
        if (playerVicino && !eventoAvviato && Input.GetKeyDown(KeyCode.E))
        {
            Interagisci();
        }
    }

    private void Interagisci()
    {
        if (InventorySystem.Instance == null) return;

        int indiceDocumento = -1;
        for (int i = 0; i < 16; i++)
        {
            var item = InventorySystem.Instance.GetItem(i);
            if (item != null && item.name == documentoRichiesto)
            {
                indiceDocumento = i;
                break;
            }
        }

        if (indiceDocumento != -1)
        {
            // Il player ha il documento
            InventorySystem.Instance.RemoveItem(indiceDocumento);
            eventoAvviato = true;

            PauseManager.RequestPause();
            if (popupConDocumento) popupConDocumento.SetActive(true);

            // AVVIO DEL SUONO POPUP CON FADE-IN
            if (audioSourcePopup != null)
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeInAudioPopup(audioSourcePopup, tempoSfumatura));
            }

            // Spegniamo il WaveTrigger adesso che l'evento è iniziato con successo
            WaveTrigger wt = GetComponent<WaveTrigger>();
            if (wt != null)
            {
                wt.DisattivaTriggerPermanente();
            }
        }
        else
        {
            // CORRETTO: Usiamo 'Istanza' in italiano come definito nel vostro script
            if (TerminalManager.Istanza != null)
            {
                TerminalManager.Istanza.MostraMessaggioLibero(
                    "> ACCESSO NEGATO.\n" +
                    "> Documento di autorizzazione mancante.\n\n" +
                    "> Il file richiesto per avviare i calcoli si trova\n" +
                    "> archiviato nel settore RAM.\n\n" +
                    "> Recuperalo e torna qui per procedere."
                );
                TerminalManager.Istanza.ApriTerminale();
            }
        }
    }

    // Da collegare al bottone "Chiudi" del popup CON documento
    public void ChiudiPopupConDocumento()
    {
        if (!eventoAvviato)
        {
            Debug.LogWarning("Tentativo di avvio ondate senza autorizzazione!");
            return;
        }

        // SFUMATURA IN USCITA DEL SUONO POPUP
        if (audioSourcePopup != null && audioSourcePopup.isPlaying)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOutAudioPopup(audioSourcePopup, tempoSfumatura));
        }

        PauseManager.ReleasePause();
        if (popupConDocumento) popupConDocumento.SetActive(false);

        if (waveManager != null)
            waveManager.StartWaves();
    }

    // Coroutine per far crescere il suono all'apertura del popup (funziona anche a Time.timeScale = 0)
    private IEnumerator FadeInAudioPopup(AudioSource source, float durata)
    {
        source.volume = 0f;
        source.Play();

        float t = 0f;
        while (t < durata)
        {
            t += Time.unscaledDeltaTime; // USIAMO UNSCALED!
            source.volume = Mathf.Lerp(0f, 1f, t / durata);
            yield return null;
        }

        source.volume = 1f;
    }

    // Coroutine per sfumare l'audio in uscita, anche a tempo fermo (Time.timeScale = 0)
    private IEnumerator FadeOutAudioPopup(AudioSource source, float durata)
    {
        float volumeIniziale = source.volume;
        float t = 0f;

        while (t < durata)
        {
            t += Time.unscaledDeltaTime; // USIAMO UNSCALED!
            source.volume = Mathf.Lerp(volumeIniziale, 0f, t / durata);
            yield return null;
        }

        source.Stop();
        source.volume = 1f; // Ripristiniamo il volume pieno per la prossima apertura
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || eventoAvviato) return;
        playerVicino = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerVicino = false;
    }

    public void OnOndateCompletate()
    {
        Debug.Log("Tutte le ondate completate! Faccio apparire il documento con la password.");

        if (documentoConPassword != null && puntoUscitaDocumento != null)
        {
            GameObject doc = Instantiate(documentoConPassword, puntoUscitaDocumento.position, Quaternion.identity);
            StartCoroutine(AnimaVoloDocumento(doc));
        }
    }

    private IEnumerator AnimaVoloDocumento(GameObject doc)
    {
        Vector3 partenza = doc.transform.position;
        Vector3 arrivo = partenza + new Vector3(0, 1.5f, 0);

        float durata = 0.8f;
        float t = 0f;

        while (t < durata)
        {
            t += Time.deltaTime;
            doc.transform.position = Vector3.Lerp(partenza, arrivo, t / durata);
            yield return null;
        }
    }
}
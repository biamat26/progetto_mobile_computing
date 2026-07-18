using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SimulazioneDownload : MonoBehaviour
{
    [Header("Impostazioni Download")]
    [Tooltip("Quanti secondi deve durare il finto download?")]
    public float tempoDiDownload = 4f;

    private int indiceScenaDaCaricare;

    [Header("Riferimenti UI")]
    public Slider barraDiCaricamento;
    public TMP_Text testoPercentuale;
    public TMP_Text testoStato; 

    [Header("Audio (Opzionale)")]
    public AudioSource audioSource;
    public AudioClip suonoDownloadFinito;

    void Start()
    {
        // 1. Legge il post-it
        indiceScenaDaCaricare = PlayerPrefs.GetInt("IndiceScenaDestinazione", 3);
        
        // --- SISTEMA ANTI-LOOP INFALLIBILE ---
        // SceneManager.GetActiveScene().buildIndex ottiene l'indice di QUESTA scena (es. 2).
        // Se il gioco prova a ricaricare la stessa scena di download, forziamo l'uscita verso il livello 3!
        if (indiceScenaDaCaricare == SceneManager.GetActiveScene().buildIndex)
        {
            Debug.LogWarning("ATTENZIONE: Bloccato un loop infinito! Forzato il caricamento della scena 3.");
            indiceScenaDaCaricare = 3; 
        }
        // -------------------------------------

        if (barraDiCaricamento != null)
        {
            barraDiCaricamento.minValue = 0f;
            barraDiCaricamento.maxValue = 100f;
            barraDiCaricamento.value = 0f;
        }

        StartCoroutine(EseguiFintoDownload());
    }

    private IEnumerator EseguiFintoDownload()
    {
        float tempoTrascorso = 0f;

        while (tempoTrascorso < tempoDiDownload)
        {
            tempoTrascorso += Time.deltaTime;
            float percentuale = (tempoTrascorso / tempoDiDownload) * 100f;
            
            if (barraDiCaricamento != null)
                barraDiCaricamento.value = percentuale;

            if (testoPercentuale != null)
                testoPercentuale.text = Mathf.RoundToInt(percentuale) + "%";

            if (testoStato != null)
            {
                if (percentuale < 30f)
                    testoStato.text = "Inizializzazione protocolli di rete...";
                else if (percentuale < 60f)
                    testoStato.text = "Decrittazione pacchetti dati...";
                else if (percentuale < 90f)
                    testoStato.text = "Installazione payload nel sistema...";
                else
                    testoStato.text = "Accesso garantito.";
            }

            yield return null; 
        }

        if (barraDiCaricamento != null) barraDiCaricamento.value = 100f;
        if (testoPercentuale != null) testoPercentuale.text = "100%";

        if (audioSource != null && suonoDownloadFinito != null)
        {
            audioSource.PlayOneShot(suonoDownloadFinito);
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        // CARICAMENTO DINAMICO CORRETTO
        SceneManager.LoadScene(indiceScenaDaCaricare);
    }
}
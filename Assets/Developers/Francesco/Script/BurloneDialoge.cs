using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // Aggiunto per poter cambiare scena!

public class BurloneDialogue : MonoBehaviour
{
    [Header("UI Dialogo")]
    public GameObject pannelloDialogo;     
    public TMP_Text testoDialogo;          

    [Header("Pannelli Bottoni")]
    public GameObject gruppoBottoniDomanda1; 
    public GameObject gruppoBottoniSiNo;     

    [Header("Audio Dialogo")]
    [Tooltip("La musica che parte durante la chiacchierata (sostituisce quella di scena)")]
    public AudioClip musicaDialogo;
    [Tooltip("Il suono di quando clicchi una delle risposte")]
    public AudioClip suonoBottone;
    [Tooltip("Il ticchettio della scrittura. IMPORTANTE: usa un suono cortissimo!")]
    public AudioClip suonoScrittura;

    private SceneAudioController sceneAudioController;
    private AudioSource audioScrittura; 

    // --- LA "MEMORIA" DEL BOSS ---
    private bool haGiaRifiutato = false; 

    void Start()
    {
        pannelloDialogo.SetActive(false);
        gruppoBottoniDomanda1.SetActive(false);
        gruppoBottoniSiNo.SetActive(false);

        audioScrittura = gameObject.AddComponent<AudioSource>();
        audioScrittura.playOnAwake = false;
        audioScrittura.volume = 0.4f; 
        audioScrittura.pitch = 1.1f;  
    }

    public void AvviaDialogo()
    {
        // --- 1. BLOCCA LA MUSICA DI SCENA ---
        sceneAudioController = FindFirstObjectByType<SceneAudioController>();
        if (sceneAudioController != null)
        {
            sceneAudioController.StopAllCoroutines();
            if (sceneAudioController.musicaScena != null)
            {
                AudioSource[] tuttiGliAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
                foreach (AudioSource src in tuttiGliAudio)
                {
                    if (src.clip == sceneAudioController.musicaScena) src.Stop();
                }
            }
        }

        // --- 2. AVVIA MUSICA DEL DIALOGO ---
        if (AudioManager.instance != null && musicaDialogo != null)
        {
            AudioManager.instance.PlayMusic(musicaDialogo, 0f, 0f);
        }

        pannelloDialogo.SetActive(true);
        gruppoBottoniDomanda1.SetActive(false);
        gruppoBottoniSiNo.SetActive(false);
        
        // --- 3. CONTROLLO MEMORIA (SCELTA DEL DIALOGO) ---
        if (haGiaRifiutato == true)
        {
            StartCoroutine(ScriviTesto("Di nuovo tu? Hai aggiornato il tuo coraggio o sei qui solo per farmi perdere tempo? Vuoi veramente combattere con me?", MostraScelteSiNo));
        }
        else
        {
            StartCoroutine(ScriviTesto("Ah-ah-ah! Fermo dove sei, intruso! Sento il tuo processore che suda freddo da qui... Guardami bene, utente. Sai chi sono io?", MostraScelteIniziali));
        }
    }

    private void MostraScelteIniziali()
    {
        gruppoBottoniDomanda1.SetActive(true); 
    }

    private void RiproduciSuonoClick()
    {
        if (suonoBottone != null)
        {
            AudioSource.PlayClipAtPoint(suonoBottone, Camera.main.transform.position);
        }
    }

    // --- FUNZIONI PER I PRIMI 3 BOTTONI ---
    public void ScegliRisposta1() 
    {
        RiproduciSuonoClick();
        gruppoBottoniDomanda1.SetActive(false);
        StartCoroutine(ScriviTesto("Uffa... che noia. Nessuno apprezza più la suspense al giorno d'oggi! Hai ficcanasato nei file di sistema, eh? Ma passiamo alle cose formali...", ChiediSeVuoleCombattere));
    }

    public void ScegliRisposta2() 
    {
        RiproduciSuonoClick();
        gruppoBottoniDomanda1.SetActive(false);
        StartCoroutine(ScriviTesto("Magnifico! Adoro il pubblico nuovo! Preparati a formattare le tue certezze, hai davanti il re del caos: BURLONE! Ma dimmi un po'...", ChiediSeVuoleCombattere));
    }

    public void ScegliRisposta3() 
    {
        RiproduciSuonoClick();
        gruppoBottoniDomanda1.SetActive(false);
        StartCoroutine(ScriviTesto("Come osi...? 'Malware'? IO SONO ARTE DIGITALE PURA! Siete voi utenti il vero virus! Volevo andarci piano, ma ora facciamo sul serio...", ChiediSeVuoleCombattere));
    }

    // --- LA DOMANDA FINALE ---
    private void ChiediSeVuoleCombattere()
    {
        StartCoroutine(ScriviTesto("Vuoi veramente combattere con me?", MostraScelteSiNo));
    }

    private void MostraScelteSiNo()
    {
        gruppoBottoniSiNo.SetActive(true); 
    }

    // --- FUNZIONI PER I BOTTONI SI E NO ---
    public void ScegliDiCombattere() 
    {
        RiproduciSuonoClick();
        gruppoBottoniSiNo.SetActive(false);
        StartCoroutine(ScriviTesto("Eccellente! Maestro, accenda la musica! Che lo spettacolo abbia inizio!", AvviaBattaglia));
    }

    public void ScegliDiScappare() 
    {
        RiproduciSuonoClick();
        gruppoBottoniSiNo.SetActive(false);
        haGiaRifiutato = true; 
        StartCoroutine(ScriviTesto("Codardo! Torna quando avrai aggiornato il tuo coraggio alla versione 2.0!", ChiudiDialogo));
    }

    // --- AZIONI FINALI ---
    private void AvviaBattaglia()
    {
        // Zittiamo la musica del dialogo per evitare che continui mentre carica l'altra scena
        if (musicaDialogo != null)
        {
            AudioSource[] tuttiGliAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource src in tuttiGliAudio)
            {
                if (src.clip == musicaDialogo) src.Stop();
            }
        }

        // CARICA LA SCENA DEL BOSS (Assicurati che il nome sia esatto!)
        SceneManager.LoadScene("Combattimento Finale"); 
    }

    private void ChiudiDialogo()
    {
        pannelloDialogo.SetActive(false); 
        
        if (musicaDialogo != null)
        {
            AudioSource[] tuttiGliAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource src in tuttiGliAudio)
            {
                if (src.clip == musicaDialogo) src.Stop();
            }
        }

        if (sceneAudioController != null)
        {
            sceneAudioController.SendMessage("Start");
        }
    }

    // --- EFFETTO MACCHINA DA SCRIVERE CON AUDIO ---
    private IEnumerator ScriviTesto(string testoDaScrivere, System.Action fineScritturaCallback)
    {
        testoDialogo.text = "";
        
        foreach (char c in testoDaScrivere)
        {
            testoDialogo.text += c;
            
            if (suonoScrittura != null && !char.IsWhiteSpace(c))
            {
                audioScrittura.PlayOneShot(suonoScrittura);
            }

            yield return new WaitForSeconds(0.02f); 
        }

        yield return new WaitForSeconds(0.5f); 
        
        fineScritturaCallback?.Invoke();
    }
}
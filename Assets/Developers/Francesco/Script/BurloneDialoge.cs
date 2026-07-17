using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; 

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
            StartCoroutine(ScriviTesto("Di nuovo tu? Hai capito la differenza tra CISC e RISC?", MostraScelteSiNo));
        }
        else
        {
            // NOTA: Ho unito la stringa su una sola riga di codice usando il \n per evitare errori di compilazione in C#
            StartCoroutine(ScriviTesto("Ah-ah-ah! Fermo dove sei, intruso! Io sono Riccardo Burlone, il re dei virus. Vediamo se sei degno di stare qui...\nLa mia domanda è: questa operazione logica --> (x AND y) OR [ (y XOR x) AND (x OR z) ] <-- è l'equivalente di...? ", MostraScelteIniziali));
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
        StartCoroutine(ScriviTesto("Ahahah!! Lo immaginavo che non avresti saputo collegare due fili su Logisim...", ChiediSeVuoleCombattere));
    }

    public void ScegliRisposta2() 
    {
        RiproduciSuonoClick();
        gruppoBottoniDomanda1.SetActive(false);
        StartCoroutine(ScriviTesto("...\nOk va bene, se mi sconfiggerai ti metterò 18", ChiediSeVuoleCombattere));
    }

    public void ScegliRisposta3() 
    {
        RiproduciSuonoClick();
        gruppoBottoniDomanda1.SetActive(false);
        StartCoroutine(ScriviTesto("Ahahah!! Lo immaginavo che non avresti saputo collegare due fili su Logisim...", ChiediSeVuoleCombattere));
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
        StartCoroutine(ScriviTesto("Finalmente! Aspettavo questo momento da un po' di tempo. Fatti sotto...", AvviaBattaglia));
    }

    public void ScegliDiScappare() 
    {
        RiproduciSuonoClick();
        gruppoBottoniSiNo.SetActive(false);
        haGiaRifiutato = true; 
        StartCoroutine(ScriviTesto("Villano! Torna quando avrai capito la differenza tra CISC e RISC", ChiudiDialogo));
    }

    // --- AZIONI FINALI ---
    private void AvviaBattaglia()
    {
        if (musicaDialogo != null)
        {
            AudioSource[] tuttiGliAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource src in tuttiGliAudio)
            {
                if (src.clip == musicaDialogo) src.Stop();
            }
        }

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
            // Se incontra il comando "a capo", fa una pausa e pulisce lo schermo
            if (c == '\n')
            {
                // MODIFICA: Ora aspetta 2.5 secondi prima di cancellare
                yield return new WaitForSeconds(2.5f); 
                
                // Cancella il testo dal pannello
                testoDialogo.text = ""; 
                
                // Salta questo ciclo e passa direttamente alla lettera successiva
                continue; 
            }

            testoDialogo.text += c;
            
            if (suonoScrittura != null && !char.IsWhiteSpace(c))
            {
                audioScrittura.PlayOneShot(suonoScrittura);
            }

            // MODIFICA: Generazione testo rallentata da 0.02f a 0.04f
            yield return new WaitForSeconds(0.04f); 
        }

        yield return new WaitForSeconds(0.5f); 
        
        fineScritturaCallback?.Invoke();
    }
}
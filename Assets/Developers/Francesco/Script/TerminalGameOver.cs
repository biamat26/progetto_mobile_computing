using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TerminalGameOver : MonoBehaviour
{
    [Header("UI Terminale")]
    public TextMeshProUGUI terminalText;
    public string mainMenuSceneName = "Main Menu";
    public float velocitaDigitazione = 0.05f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingSound; // Suono del singolo tasto
    public AudioClip crashSound;  // Suono di errore grave alla fine

    // Questo è il testo che il Professore digiterà. La '\n' serve per andare a capo.
    private string sequenzaDistruzione = 
        "> OVERRIDE DI SISTEMA INIZIATO...\n" +
        "> Acquisizione privilegi di ROOT completata.\n" +
        "> professor_burlone@kernel:~# sudo rm -rf --no-preserve-root /\n" +
        "> Cancellazione file di base in corso...\n" +
        "> Eliminazione OS completata.\n";

    void Start()
    {
        // Pulisce lo schermo all'avvio
        if (terminalText != null) terminalText.text = "";
        
        StartCoroutine(EseguiGameOver());
    }

    IEnumerator EseguiGameOver()
    {
        // Pausa drammatica iniziale
        yield return new WaitForSeconds(1.5f);

        // Separa il testo riga per riga per fare delle pause tra un comando e l'altro
        string[] righe = sequenzaDistruzione.Split('\n');
        
        foreach (string riga in righe)
        {
            if (string.IsNullOrEmpty(riga)) continue;

            // Digita lettera per lettera
            foreach (char lettera in riga.ToCharArray())
            {
                terminalText.text += lettera;
                
                if (audioSource != null && typingSound != null)
                {
                    // Usa un volume e un pitch leggermente casuale per sembrare una vera tastiera
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(typingSound, 0.4f);
                }
                
                yield return new WaitForSeconds(velocitaDigitazione);
            }
            
            // Va a capo e aspetta un secondo prima di scrivere la riga successiva
            terminalText.text += "\n";
            yield return new WaitForSeconds(1f); 
        }

        yield return new WaitForSeconds(1f);
        
        // Colpo di grazia: il crash
        terminalText.text += "\n<color=red>FATAL ERROR: SYSTEM NOT FOUND.</color>\n<color=red>SHUTTING DOWN...</color>";
        
        if (audioSource != null && crashSound != null)
        {
            audioSource.pitch = 1f; // Rimette il pitch normale per l'esplosione/errore
            audioSource.PlayOneShot(crashSound);
        }

        // Aspetta 5 secondi, al buio, con la scritta rossa, e poi lo butta fuori al Menu
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
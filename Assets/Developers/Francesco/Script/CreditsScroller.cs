using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections; // --- AGGIUNTO: Necessario per il timer ---

public class CreditsScroller : MonoBehaviour
{
    [Header("Impostazioni Testo")]
    [Tooltip("Trascina qui l'oggetto Text (TMP) dei titoli di coda")]
    public TextMeshProUGUI testoCrediti;
    
    [Tooltip("Velocità con cui il testo sale verso l'alto")]
    public float speed = 50f; 

    [Header("Impostazioni Uscita")]
    [Tooltip("Il nome della scena a cui tornare alla fine")]
    public string nomeScenaMenu = "MainMenuScene"; 
    
    // --- NUOVA VARIABILE PER IL TIMER ---
    [Tooltip("Quanti secondi ci mette il testo a finire?")]
    public float tempoRitornoMenu = 30f; 
    // ------------------------------------

    void Start()
    {
        string nomeGiocatore = "Ospite";
        if (UserSession.Instance != null && !string.IsNullOrEmpty(UserSession.Instance.Username))
        {
            nomeGiocatore = UserSession.Instance.Username;
        }

        string testoEpico = 
            "ERRORE DI SISTEMA RISOLTO: MINACCIA ELIMINATA.\n\n" +
            "Il codice malevolo noto come 'Riccardo Burlone' è stato isolato, frammentato e definitivamente svuotato dalla memoria cache. " +
            "Il suo disperato tentativo di mandare la CPU in overflow si è rivelato un totale fallimento di fronte alla tua impeccabile logica di sistema.\n\n" +
            "I cicli di clock stanno tornando alla normalità. La RAM respira di nuovo, finalmente ripulita dai blocchi di dati corrotti. " +
            "L'intera architettura hardware risuona di una rinnovata armonia elettrica.\n\n" +
            "Hai dimostrato che nessuna anomalia, per quanto instabile o aggressiva, è al di là di un buon debugging. Il sistema è salvo. Il Kernel ti è debitore.\n\n\n" +
            "--- SISTEMA RIPRISTINATO DA: ---\n\n" +
            "> " + nomeGiocatore.ToUpper() + " <\n" +
            "Lead Programmer & Debugger Supremo\n\n\n" +
            "--- SVILUPPATO DA: ---\n\n" +
            "Francesco, Luigi e Matteo\n" + 
            "System Architects & Cyber-Designers\n\n\n" +
            "Grazie per aver giocato.";

        if (testoCrediti != null)
        {
            testoCrediti.text = testoEpico;
        }

        // --- FACCIAMO PARTIRE IL CONTO ALLA ROVESCIA ---
        StartCoroutine(RitornoAutomatico());
    }

    void Update()
    {
        if (testoCrediti != null)
        {
            testoCrediti.rectTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;
        }

        // Continua a permettere al giocatore di saltare i crediti se si annoia
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(nomeScenaMenu);
        }
    }

    // --- FUNZIONE DEL TIMER ---
    IEnumerator RitornoAutomatico()
    {
        // Aspetta per i secondi indicati nell'Inspector
        yield return new WaitForSeconds(tempoRitornoMenu);
        
        // Torna al menu
        SceneManager.LoadScene(nomeScenaMenu);
    }
}
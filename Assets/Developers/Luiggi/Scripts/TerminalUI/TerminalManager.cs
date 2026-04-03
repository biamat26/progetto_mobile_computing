using System.Collections.Generic;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    public static TerminalManager Istanza;

    [Header("Componenti")]
    public TerminalUI terminalUI;
    public RectTransform terminalRect; 

    [Header("Stato e Cronologia")]
    public bool isExpanded = false;
    public int maxMessaggi = 10; // Quanti messaggi ricorda al massimo

    [Header("Impostazioni Dimensioni")]
    public Vector2 sizeMini = new Vector2(150, 150); // Messo quadrato come volevi
    public Vector2 sizeFull = new Vector2(800, 500);
    public Vector2 offsetMini = new Vector2(-20, 20);

    // Database dei messaggi e Memoria storica
    private Dictionary<string, string> databaseMessaggi = new Dictionary<string, string>();
    private List<string> cronologiaMessaggi = new List<string>();

    void Awake()
    {
        if (Istanza != null && Istanza != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Istanza = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CaricaMessaggi(); 
        
        Time.timeScale = 1f;
        isExpanded = false;
        AggiornaVisuale();

        // Facciamo apparire il primo messaggio all'avvio
        MostraAiuto("intro");
    }

    private void CaricaMessaggi()
    {
        databaseMessaggi.Clear(); 
        
        // AGGIUNGI QUI I TUOI MESSAGGI
        databaseMessaggi.Add("intro", "> SISTEMA ONLINE.\n> Monitoraggio avviato.");
        databaseMessaggi.Add("virus_alert", "> ATTENZIONE!\n> Rilevata minaccia virale.");
        databaseMessaggi.Add("hint_generico", "> ANALISI AMBIENTALE:\n> Esplora l'area circostante.");
    }

    public void ToggleTerminal()
    {
        isExpanded = !isExpanded;
        AggiornaVisuale();
        
        // Quando lo apri/chiudi, ricarica il testo corretto
        if (cronologiaMessaggi.Count > 0)
        {
            AggiornaTestoTerminale(cronologiaMessaggi[cronologiaMessaggi.Count - 1]);
        }
    }

    private void AggiornaVisuale()
    {
        if (isExpanded)
        {
            // STATO GRANDE
            Time.timeScale = 0f; 
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; 
            
            terminalRect.anchorMin = new Vector2(0.5f, 0.5f);
            terminalRect.anchorMax = new Vector2(0.5f, 0.5f);
            terminalRect.pivot = new Vector2(0.5f, 0.5f);
            terminalRect.anchoredPosition = Vector2.zero; 
            terminalRect.sizeDelta = sizeFull; 
        }
        else
        {
            // STATO PICCOLO
            Time.timeScale = 1f; 
            Cursor.visible = true; 
            Cursor.lockState = CursorLockMode.None; 

            terminalRect.anchorMin = new Vector2(1, 0); 
            terminalRect.anchorMax = new Vector2(1, 0);
            terminalRect.pivot = new Vector2(1, 0);
            terminalRect.anchoredPosition = offsetMini; 
            terminalRect.sizeDelta = sizeMini;
        }
    }

    public void MostraAiuto(string idMessaggio)
    {
        if (databaseMessaggi.ContainsKey(idMessaggio))
        {
            string nuovoMessaggio = databaseMessaggi[idMessaggio];
            
            // Aggiunge alla cronologia e cancella i vecchi se sono troppi
            cronologiaMessaggi.Add(nuovoMessaggio);
            if (cronologiaMessaggi.Count > maxMessaggi)
            {
                cronologiaMessaggi.RemoveAt(0);
            }

            AggiornaTestoTerminale(nuovoMessaggio);
        }
        else
        {
            Debug.LogWarning("Messaggio non trovato: " + idMessaggio);
        }
    }

    private void AggiornaTestoTerminale(string ultimoMessaggio)
    {
        if (isExpanded)
        {
            // Se è grande, unisce tutti i messaggi della storia e li spara subito a schermo
            string testoCompleto = "> ARCHIVIO SISTEMA:\n\n" + string.Join("\n\n", cronologiaMessaggi);
            terminalUI.ScriviMessaggio(testoCompleto, true); 
        }
        else
        {
            // Se è piccolo, scrive solo l'ultimo messaggio con l'effetto hacker
            terminalUI.ScriviMessaggio(ultimoMessaggio, true); 
        }
    }

    public void ResetProgressoGiocatore()
    {
        if (!isExpanded) { }
    }
}
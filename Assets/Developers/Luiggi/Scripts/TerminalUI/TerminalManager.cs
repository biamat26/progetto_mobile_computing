using System.Collections.Generic;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    public static TerminalManager Istanza;

    [Header("Componenti")]
    public TerminalUI terminalUI;
    public RectTransform terminalRect; 
    public GameObject bottoneTerminale; // <-- NUOVO: Il riferimento al bottoncino

    [Header("Stato e Cronologia")]
    public bool isExpanded = false;
    public int maxMessaggi = 10; // Quanti messaggi ricorda al massimo

    [Header("Impostazioni Dimensioni")]
    public Vector2 sizeFull = new Vector2(800, 500);

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
        
        // Nascondi il terminale grande all'avvio
        terminalRect.gameObject.SetActive(false);

        // Carica subito la storia iniziale nella cronologia "dietro le quinte"
        MostraAiuto("intro"); 
    }

    public void MostraMessaggioLibero(string testo)
    {
        cronologiaMessaggi.Add(testo);
        if (cronologiaMessaggi.Count > maxMessaggi)
            cronologiaMessaggi.RemoveAt(0);
            
        AggiornaTestoTerminale(testo);
    }

    private void CaricaMessaggi()
    {
        databaseMessaggi.Clear(); 
        
        // --- LA NUOVA INTRO DEL GIOCO CON SPIEGAZIONE E MISSIONI ---
        string testoIntro = "> CONNESSIONE AL SISTEMA STABILITA...\n" +
                            "> STATO UTENTE: Digitalizzato.\n\n" +
                            "> BENVENUTO, ALUNNO.\n" +
                            "> Sei stato intrappolato all'interno della struttura hardware di un computer.\n" +
                            "> Attualmente ti trovi fisicamente sui circuiti della scheda madre.\n" +
                            "> Il sistema è gravemente infetto e ha bisogno del tuo intervento manuale.\n\n" +
                            "> DIRETTIVE DI MISSIONE:\n" +
                            "> 1. Esplora i componenti fisici del PC (CPU, RAM, Hard Disk).\n" +
                            "> 2. Utilizza i BUS di sistema per viaggiare tra i vari settori.\n" +
                            "> 3. Combatti ed elimina le minacce Virus che incontri.\n" +
                            "> 4. Sconfiggi il Boss finale (Rootkit) per riparare il PC e fuggire.\n\n" +
                            "> Buona fortuna. Avvio del sistema in corso...";

        databaseMessaggi.Add("intro", testoIntro);

        // Altri messaggi esistenti
        databaseMessaggi.Add("virus_alert", "> ATTENZIONE!\n> Rilevata minaccia virale.");
        databaseMessaggi.Add("hint_generico", "> ANALISI AMBIENTALE:\n> Esplora l'area circostante.");
    }

    public void ToggleTerminal()
    {
        isExpanded = !isExpanded;
        
        // Accende o spegne il pannello gigante
        terminalRect.gameObject.SetActive(isExpanded);

        // Spegne il bottoncino se il terminale è aperto, lo riaccende se è chiuso
        if(bottoneTerminale != null)
        {
            bottoneTerminale.SetActive(!isExpanded); 
        }

        AggiornaVisuale();

        if (isExpanded)
        {
            // Ricarica il testo corretto
            if (cronologiaMessaggi.Count > 0)
            {
                AggiornaTestoTerminale(cronologiaMessaggi[cronologiaMessaggi.Count - 1]);
            }
        }
    }

    private void AggiornaVisuale()
    {
        // Visto che l'oggetto si spegne quando non è expanded, 
        // ci interessa solo settare le impostazioni di quando è aperto!
        if (isExpanded)
        {
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
            // Quando si chiude, facciamo solo ripartire il tempo
            Time.timeScale = 1f; 
            // Cursor.visible = false; // Togli il commento se nascondi il mouse durante il gioco
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
        // Compiliamo il testo a schermo solo se il terminale è aperto
        if (isExpanded)
        {
            string testoCompleto = "> ARCHIVIO SISTEMA:\n\n" + string.Join("\n\n", cronologiaMessaggi);
            terminalUI.ScriviMessaggio(testoCompleto, true); 
        }
    }

    public void ResetProgressoGiocatore()
    {
        if (!isExpanded) { }
    }
}
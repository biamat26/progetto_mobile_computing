using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    public static TerminalManager Istanza;

    [Header("Componenti")]
    public TerminalUI terminalUI;
    public RectTransform terminalRect; 
    public GameObject bottoneTerminale;

    [Header("Visibilità per Scena")]
    public string[] sceneDiGioco; // nomi esatti delle scene dove il terminale deve essere visibile
    public GameObject terminalRoot; // il genitore che contiene bottone + icona + pannello (es. "Terminal" o "ButtonTerminale")

    [Header("Stato e Cronologia")]
    public bool isExpanded = false;
    public int maxMessaggi = 10; // Quanti messaggi ricorda al massimo

    [Header("Impostazioni Dimensioni")]
    public Vector2 sizeFull = new Vector2(800, 500);

    [Header("Notifica")]
    public GameObject iconaNotifica; // l'oggetto lampeggiante, lo assegni tu nell'Inspector
    public AudioSource audioSource; // <-- NUOVO
    public AudioClip suonoNotifica; // <-- NUOVO
    // Database dei messaggi e Memoria storica
    private Dictionary<string, string> databaseMessaggi = new Dictionary<string, string>();
    private List<string> cronologiaMessaggi = new List<string>();
    private string ultimoMessaggio = "";
    private bool primaVolta = false;

  void Awake()
{
    if (Istanza != null && Istanza != this) 
    { 
        Destroy(gameObject); 
        return; 
    }
    Istanza = this;
    DontDestroyOnLoad(gameObject);

    SceneManager.sceneLoaded += OnSceneLoaded; // <-- NUOVO
}

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // <-- NUOVO
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool èSceneDiGioco = System.Array.Exists(sceneDiGioco, nome => nome == scene.name);
        
        if (terminalRoot != null)
            terminalRoot.SetActive(èSceneDiGioco);
    }

    void Start()
    {
        CaricaMessaggi(); 
        
        Time.timeScale = 1f;
        isExpanded = false;
        
        terminalRect.gameObject.SetActive(false);

        // Precarica l'intro SENZA accendere la notifica: si apre da sola allo spawn
        MostraAiuto("Intro", false); // <-- false invece di lasciare il default
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
        string testoIntro = 
    "> CONNESSIONE AL SISTEMA STABILITA...\n" +
    "> STATO UTENTE: Digitalizzato.\n\n" +
    "> BENVENUTO, ALUNNO.\n" +
    "> Sei stato intrappolato all'interno di un PC gravemente infetto da Virus.\n" +
    "> La tua posizione attuale: HARD DISK — settore di archiviazione dati.\n\n" +
    "> SITUAZIONE CRITICA:\n" +
    "> I Virus stanno corrompendo i dati del sistema.\n" +
    "> Devi muoverti rapidamente prima che il danno sia irreversibile.\n\n" +
    "> OBIETTIVO IMMEDIATO:\n" +
    "> Raggiungi la RAM attraverso il BUS di sistema.\n" +
    "> La RAM è il prossimo settore — è lì che inizia la vera battaglia.\n\n" +
    "> Buona fortuna. Il sistema dipende da te.";

        databaseMessaggi.Add("Intro", testoIntro);

        string passwordPortaPrincipale = "> PASSWORD RICHIESTA: Porta Principale\n>" + 
        "Inserisci la password per accedere al settore successivo.";
        string portaNumeroBinario = "> ATTENZIONE: Porta rilevata nel settore.\n" +
    "> Per sbloccarla dovrai parlare come un computer.\n\n" +
    "> I computer non capiscono i numeri come li conosci tu.\n" +
    "> Usano solo due stati: SPENTO (0) e ACCESO (1).\n" +
    "> Questa è la base del sistema BINARIO.\n\n" +
    "> COME FUNZIONA LA CONVERSIONE:\n" +
    "> Ogni blocco rappresenta una potenza di 2, da destra verso sinistra.\n" +
    "> Blocco 1 (dx)  = 2^0 =  1\n" +
    "> Blocco 2       = 2^1 =  2\n" +
    "> Blocco 3       = 2^2 =  4\n" +
    "> Blocco 4       = 2^3 =  8\n" +
    "> Blocco 5 (sx)  = 2^4 = 16\n" +
    "> Somma i valori dei blocchi ACCESI per ottenere il numero decimale.\n" +
    "> Esempio: 0 1 0 1 0 = 2 + 8 = 10\n\n" +
    "> ISTRUZIONI:\n" +
    "> Salta sui blocchi per attivarli.\n" +
    "> Blocco AZZURRO = 1 (acceso).\n" +
    "> Blocco BLU SCURO  = 0 (spento).\n" +
    "> Forma il numero corretto per sbloccare la porta.\n\n" +
    "> Il sistema attende il tuo input...";
    string portaChiave = "> ATTENZIONE: Questa porta è serrata! Trova la chiave per aprirla!\n>";
        databaseMessaggi.Add("PortaBit", portaNumeroBinario);
        databaseMessaggi.Add("PasswordPorta", passwordPortaPrincipale);
        databaseMessaggi.Add("PortaChiave", portaChiave);
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
    if (cronologiaMessaggi.Count > 0)
    {
        if (primaVolta)
        {
            primaVolta = false;
            terminalUI.ScriviMessaggio(ultimoMessaggio, true);
            AggiornaNotifica();
        }
        else
        {
            terminalUI.ScriviMessaggio(ultimoMessaggio, false); // senza effetto
        }
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

public void MostraAiuto(string idMessaggio, bool mostraNotifica = true)
{
    if (databaseMessaggi.ContainsKey(idMessaggio))
    {
        string nuovoMessaggio = databaseMessaggi[idMessaggio];
        
        cronologiaMessaggi.Add(nuovoMessaggio);
        if (cronologiaMessaggi.Count > maxMessaggi)
            cronologiaMessaggi.RemoveAt(0);

        ultimoMessaggio = nuovoMessaggio;
        primaVolta = true;

        if (mostraNotifica)
            AggiornaNotifica();
    }
    else
    {
        Debug.LogWarning("Messaggio non trovato: " + idMessaggio);
    }
}

private void AggiornaNotifica()
{
    bool nuovoStato = primaVolta && !isExpanded;

    // Suona solo se la notifica sta passando da spenta ad accesa
    if (iconaNotifica != null)
    {
        bool eraGiaAttiva = iconaNotifica.activeSelf;
        
        if (nuovoStato && !eraGiaAttiva)
            RiproduciSuonoNotifica();

        iconaNotifica.SetActive(nuovoStato);
    }
}

private void RiproduciSuonoNotifica()
{
    if (audioSource != null && suonoNotifica != null)
        audioSource.PlayOneShot(suonoNotifica);
}

    private void AggiornaTestoTerminale(string ultimoMessaggio)
    {
        // Compiliamo il testo a schermo solo se il terminale è aperto
       if (isExpanded)
    {
        terminalUI.ScriviMessaggio(ultimoMessaggio, true);
    }
    }

    public void ResetProgressoGiocatore()
    {
        if (!isExpanded) { }
    }

    public void ApriTerminale()
{
    if (isExpanded) return; // già aperto, non fare nulla

    isExpanded = true;
    terminalRect.gameObject.SetActive(true);

    if (bottoneTerminale != null)
        bottoneTerminale.SetActive(false);

    AggiornaVisuale();

    if (cronologiaMessaggi.Count > 0)
    {
        if (primaVolta)
        {
            primaVolta = false;
            terminalUI.ScriviMessaggio(ultimoMessaggio, true);
            AggiornaNotifica(); // <-- NUOVO
        }
        else
        {
            terminalUI.ScriviMessaggio(ultimoMessaggio, false);
        }
    }
}
}
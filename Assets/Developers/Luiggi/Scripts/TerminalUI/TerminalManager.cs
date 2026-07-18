using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    public static TerminalManager Istanza;

    [Header("Gestione UI Sovrapposta")]
    [SerializeField] private GameObject waveHUD;

    [Header("Componenti")]
    public TerminalUI terminalUI;
    public RectTransform terminalRect;
    public GameObject bottoneTerminale;

    [Header("Visibilità per Scena")]
    public string[] sceneDiGioco;
    public GameObject terminalRoot;

    [Header("Stato e Cronologia")]
    public bool isExpanded = false;
    public int maxMessaggi = 10;

    [Header("Impostazioni Dimensioni")]
    public Vector2 sizeFull = new Vector2(800, 500);

    [Header("Notifica")]
    public GameObject iconaNotifica;
    public AudioSource audioSource;
    public AudioClip suonoNotifica;

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
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        isExpanded = false;
        terminalRect.gameObject.SetActive(false);
        MostraAiuto("Intro", false);
    }

    public void ToggleTerminal()
    {
        bool eraEspanso = isExpanded;
        isExpanded = !isExpanded;

        // Gestione HUD "Fantasma" controllata in modo intelligente
        GestisciHUD(isExpanded);

        terminalRect.gameObject.SetActive(isExpanded);

        if (bottoneTerminale != null)
            bottoneTerminale.SetActive(!isExpanded);

        AggiornaVisuale(eraEspanso);

        if (isExpanded && cronologiaMessaggi.Count > 0)
        {
            if (primaVolta)
            {
                primaVolta = false;
                terminalUI.ScriviMessaggio(ultimoMessaggio, true);
                AggiornaNotifica();
            }
            else
            {
                terminalUI.ScriviMessaggio(ultimoMessaggio, false);
            }
        }
    }

    public void ApriTerminale()
    {
        if (isExpanded) return;

        bool eraEspanso = isExpanded; // false
        isExpanded = true;
        GestisciHUD(true); // Nasconde HUD
        terminalRect.gameObject.SetActive(true);

        if (bottoneTerminale != null)
            bottoneTerminale.SetActive(false);

        AggiornaVisuale(eraEspanso);

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
                terminalUI.ScriviMessaggio(ultimoMessaggio, false);
            }
        }
    }

    // Metodo unico per gestire la visibilità HUD
    private void GestisciHUD(bool terminaleAperto)
    {
        WaveManager wm = FindFirstObjectByType<WaveManager>();
        if (wm != null && wm.hudContainer != null)
        {
            CanvasGroup cg = wm.hudContainer.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                if (terminaleAperto)
                {
                    // Se apro il terminale, nascondo SEMPRE l'HUD
                    cg.alpha = 0f;
                    cg.blocksRaycasts = false;
                }
                else
                {
                    // Se chiudo il terminale, riattivo l'HUD SOLO SE c'è un'ondata in corso
                    bool inBattaglia = wm.AreWavesInProgress();

                    cg.alpha = inBattaglia ? 1f : 0f;
                    cg.blocksRaycasts = inBattaglia;
                }
            }
        }
    }

    // eraEspansoPrima: stato di isExpanded PRIMA di questa chiamata,
    // serve per evitare doppie richieste/rilasci di pausa sul contatore condiviso.
    private void AggiornaVisuale(bool eraEspansoPrima)
    {
        if (isExpanded)
        {
            if (!eraEspansoPrima) PauseManager.RequestPause();

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
            if (eraEspansoPrima) PauseManager.ReleasePause();
        }
    }

    // --- METODI UTILITY ---
public void MostraMessaggioLibero(string testo)
{
    cronologiaMessaggi.Add(testo);
    if (cronologiaMessaggi.Count > maxMessaggi) cronologiaMessaggi.RemoveAt(0);
    ultimoMessaggio = testo;
    primaVolta = true;

    if (isExpanded)
    {
        // Terminale già aperto: scriviamo subito, una sola volta
        primaVolta = false;
        terminalUI.ScriviMessaggio(testo, true);
    }
    else
    {
        // Terminale chiuso: solo notifica, la scrittura avverrà all'apertura
        AggiornaNotifica();
    }
}

    private void CaricaMessaggi()
    {
        databaseMessaggi.Clear();
        string testoIntro = @"> CONNESSIONE AL SISTEMA STABILITA...
SISTEMA DIGITALIZZATO — Inizializzazione protocollo di emergenza.

Rilevamento anomalia: coscienza biologica tradotta in codice binario.
Ti trovi attualmente all'interno dell'architettura fisica del computer.
L'intero settore è compromesso da un'infezione malware su larga scala che sta corrompendo i flussi di dati.

Direttiva primaria: Bonificare l'area eliminando ogni minaccia virale attiva e aprirsi un varco di sicurezza per procedere verso la memoria RAM.
 La stabilità dell'intero sistema dipende da te.";
        string portaSerrata = "Attenzione! Questa porta è serrata! Esplora la mappa e trova la chiave!";
        string testoBinario = @"> ACCESSO AL SETTORE LOGICO: PROTOCOLLO BINARIO...
REGISTRO DI DIAGNOSTICA: Griglia di commutazione locale attiva.

Sei entrato nel nucleo di elaborazione a basso livello. Per sbloccare questo settore e proseguire, devi interagire con il sistema usando il Codice Binario.

Il codice binario è il linguaggio fondamentale dei computer. Ogni informazione viene tradotta in una sequenza di soli due simboli: 0 e 1 (chiamati 'bit'). 
Per capire come un computer legge i numeri, devi usare il sistema posizionale basato sulle potenze di 2. Ogni bit della sequenza, partendo da DESTRA e andando verso SINISTRA, ha un peso specifico che raddoppia a ogni passo:

- 1° bit (a destra): vale 2^0 = 1
- 2° bit:            vale 2^1 = 2
- 3° bit:            vale 2^2 = 4
...e così via (32, 64, 128...).

Per comporre un numero, ti basta sommare i valori dei bit attivi (pari a 1). 
Ad esempio, per rappresentare il numero 5, devi attivare il 1° bit (valore 1) e il 3° bit (valore 4), lasciando spenti gli altri. Otterrai la sequenza binaria '0101' (ovvero: 4 + 0 + 1).

La griglia di pedane sul pavimento rappresenta questo registro di memoria. Configura i bit modificando lo stato delle pedane:
- Sali su una casella per colorarla di BLU CELESTE: in questo modo la attivi, impostando il suo valore logico su 1.
- Sali nuovamente sulla stessa casella per farla tornare NERA: in questo modo la disattivi, ripristinando il suo valore logico su 0.

Calcola il valore richiesto, configura la griglia e sblocca il varco verso la RAM.";
    string testoPortaBloccata = @"> ALLARME: VARCO DI ACCESSO COMPROMESSO...
    STATO PORTA: BLOCCATA [ERRORE DI ALIMENTAZIONE].

    I sensori di diagnostica rilevano un cortocircuito critico sulla linea principale del sistema. I protocolli di emergenza hanno isolato questo settore elettrico, sigillando la porta di sicurezza per prevenire un sovraccarico distruttivo.

    I flussi di corrente sono interrotti. Devi trovare un modo per aggirare il guasto, ripristinare il circuito o reindirizzare l'energia per forzare l'apertura e proseguire la missione.";
    string testoPortaRamBloccata = @"> TENTATIVO DI ACCESSO ALLA MEMORIA CENTRALE ...
INTERFACCIA DI SBLOCCO: SETTORE RAM SIGILLATO.

Il passaggio diretto che conduce alla memoria RAM è stato completamente bloccato. I sistemi di sicurezza automatizzati hanno eretto una barriera crittografica per tentare di contenere l'infezione dei malware.

L'accesso a questo canale dati è ora protetto da una chiave di cifratura. Trova la password di sicurezza corretta per bypassare il firewall, forzare l'apertura della porta e proseguire!";
string testoCollegamentoFili = @">Davanti a te c'è un terminale di controllo offline. I bus dati fisici che collegano questa unità logica al resto del circuito sono scollegati, interrompendo il passaggio delle informazioni.

Per riattivare l'interfaccia e poter interagire con il computer, devi ripristinare manualmente i collegamenti elettrici. Avvicinati al pannello e collega i fili interrotti associando i terminali corrispondenti. 

Solo quando tutti i flussi di corrente saranno correttamente stabilizzati il terminale si accenderà, permettendoti di accedere ai suoi sistemi di controllo.";
string testoBenvenutoRam = @"> Sei finalmente dentro la memoria RAM!
Qui tutto si muove a una velocità folle e cambia a ogni millisecondo.

Fai la massima attenzione: qui dentro le regole sono spietate. 
Il Garbage Collector è costantemente in funzione per fare pulizia, eliminando senza pietà qualsiasi dato considerato superfluo. Inoltre, l'area è pattugliata da sentinelle di sicurezza; se ti intercettano, verrai catturato e rimosso dalla memoria.

Muoviti con prudenza, eludi la sorveglianza e pianifica i tuoi spostamenti. Il tuo obiettivo finale è aprirti un varco per raggiungere il cervello del computer: la CPU.";
string testoObiettivoRam = @"> Il tuo obiettivo qui nella RAM è andare dritto fino in fondo!
Non deviare dal percorso, schiva le sentinelle che pattugliano la zona e corri dritto verso l'ingresso della CPU!";
string testoChiaveMatteo = "Accidenti serve una chiave! Esplora la RAM e le sue zone nascoste per aprire la porta!";
string testoDeviati = @"> Maledizione, ci hanno deviato la rotta!
Siamo arrivati davanti alla porta della CPU, ma l'accesso è blindato da un codice di sicurezza. Non possiamo passare da qui: dobbiamo tornare sui nostri passi e trovare la password per sbloccarla!";
string CPU = "> BENVENUTO NELLA CPU!\n" +
"> Stato del sistema: CRITICO.\n" +
"> Minaccia rilevata: Infezione da malware rilevata nei registri ALU.\n" +
"> Richiesto intervento immediato di debug.";
string portaCPU = "> ATTENZIONE! L'accesso alla CPU è bloccato da un firewall di sicurezza.\n" +
"> Per proseguire, devi trovare la password di accesso corretta e inserirla nel terminale di sblocco.\n" +
"> Solo così potrai aprire la porta e accedere al cuore del computer.";

        databaseMessaggi.Add("Intro", testoIntro);
        databaseMessaggi.Add("PortaChiave",portaSerrata);
        databaseMessaggi.Add("PortaBit",testoBinario);
        databaseMessaggi.Add("FiliPorta",testoPortaBloccata);
        databaseMessaggi.Add("PasswordPorta",testoPortaRamBloccata);
        databaseMessaggi.Add("InizioRAM",testoBenvenutoRam);
        databaseMessaggi.Add("PortaRAM",testoChiaveMatteo);
        databaseMessaggi.Add("PortaFinale",testoObiettivoRam);
        databaseMessaggi.Add("Scherzetto",testoDeviati);
        databaseMessaggi.Add("InizioCPU",CPU);
        databaseMessaggi.Add("PortaCPU",portaCPU);
        // Aggiungi qui gli altri messaggi del database
    }

public void MostraAiuto(string idMessaggio, bool mostraNotifica = true)
{
    if (databaseMessaggi.ContainsKey(idMessaggio))
    {
        string testo = databaseMessaggi[idMessaggio];

        cronologiaMessaggi.Add(testo);
        if (cronologiaMessaggi.Count > maxMessaggi) cronologiaMessaggi.RemoveAt(0);

        ultimoMessaggio = testo;
        primaVolta = true;

        if (isExpanded)
        {
            primaVolta = false;
            terminalUI.ScriviMessaggio(testo, true);
        }
        else if (mostraNotifica)
        {
            AggiornaNotifica();
        }
    }
}

    private void AggiornaNotifica()
    {
        bool nuovoStato = primaVolta && !isExpanded;
        if (iconaNotifica != null)
        {
            if (nuovoStato && !iconaNotifica.activeSelf)
                if (audioSource != null && suonoNotifica != null) audioSource.PlayOneShot(suonoNotifica);
            iconaNotifica.SetActive(nuovoStato);
        }
    }

    public void ResetProgressoGiocatore()
    {
        if (!isExpanded)
        {
            // Istruzioni di reset
        }
    }
}
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
        terminalUI.ScriviMessaggio(testo, true);
    }

    private void CaricaMessaggi()
    {
        databaseMessaggi.Clear();
        databaseMessaggi.Add("Intro", "> CONNESSIONE AL SISTEMA STABILITA...");
        // Aggiungi qui gli altri messaggi del database
    }

    public void MostraAiuto(string idMessaggio, bool mostraNotifica = true)
    {
        if (databaseMessaggi.ContainsKey(idMessaggio))
        {
            ultimoMessaggio = databaseMessaggi[idMessaggio];
            primaVolta = true;
            if (mostraNotifica) AggiornaNotifica();
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
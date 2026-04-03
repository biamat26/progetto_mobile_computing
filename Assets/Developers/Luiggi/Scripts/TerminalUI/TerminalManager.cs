using System.Collections.Generic;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    public static TerminalManager Istanza;

    [Header("Componenti Collegati")]
    public TerminalUI terminalUI;
    public GameObject terminalPanel;

    [Header("Impostazioni Timer")]
    public float tempoLimiteInattivita = 10f;
    private float timer = 0f;
    private bool timerAttivo = false;

    private Dictionary<string, string> databaseMessaggi = new Dictionary<string, string>();

    void Awake()
    {
        if (Istanza != null && Istanza != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Istanza = this;
        DontDestroyOnLoad(this.gameObject);

        CaricaMessaggi();
    }

    void Start()
    {
        NascondiTerminale();
        Invoke("MostraIntro", 1f); // Fa apparire il benvenuto dopo 1 secondo
    }

    void Update()
    {
        if (timerAttivo)
        {
            timer += Time.deltaTime;
            if (timer >= tempoLimiteInattivita)
            {
                MostraAiuto("hint_generico");
                timerAttivo = false;
            }
        }
    }

    private void CaricaMessaggi()
    {
        databaseMessaggi.Add("intro", "> SISTEMA AVVIATO.\n> Benvenuto Luiggi. Analisi area in corso...");
        databaseMessaggi.Add("hint_generico", "> RILEVATA INATTIVITÀ.\n> Suggerimento: Esplora i dintorni o controlla l'inventario.");
        databaseMessaggi.Add("virus_alert", "> ATTENZIONE!\n> Rilevata minaccia virale nelle vicinanze.");
        // Aggiungi qui i messaggi per le altre stanze quando sarai pronto!
    }

    public void MostraIntro() { MostraAiuto("intro"); AvviaTimer(); }

    public void AvviaTimer() { timer = 0f; timerAttivo = true; }

    public void ResetProgressoGiocatore()
    {
        timer = 0f;
        timerAttivo = true; 
        NascondiTerminale();
    }

    public void MostraAiuto(string idMessaggio)
    {
        if (databaseMessaggi.ContainsKey(idMessaggio))
        {
            terminalPanel.SetActive(true);
            terminalUI.ScriviMessaggio(databaseMessaggi[idMessaggio]);
        }
    }

    public void NascondiTerminale() { terminalPanel.SetActive(false); }
}
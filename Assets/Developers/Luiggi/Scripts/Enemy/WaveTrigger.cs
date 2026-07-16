using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject tooltipInteragisci;

    [Header("Configurazione Co-op")]
    [Tooltip("Se attivo, premendo E le ondate partono subito. DISATTIVALO se l'avvio è gestito da un altro script come ComputerALU.")]
    [SerializeField] private bool avviaOndateAllInterazione = true;

    private bool playerVicino = false;
    private bool giaAttivato = false;

    void Awake()
    {
        if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
    }

    // Metodo pubblico che ComputerALU chiamerà quando l'evento parte DAVVERO con il documento
    public void DisattivaTriggerPermanente()
    {
        giaAttivato = true;
        if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
    }

    void Update()
    {
        if (giaAttivato) return;

        if (playerVicino && Input.GetKeyDown(KeyCode.E))
        {
            // Se non deve avviare le ondate in automatico, non fa nulla (ci pensa ComputerALU)
            if (!avviaOndateAllInterazione) return;

            giaAttivato = true;
            if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
            if (waveManager != null) waveManager.StartWaves();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || giaAttivato) return;
        playerVicino = true;
        if (tooltipInteragisci) tooltipInteragisci.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerVicino = false;
        if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
    }
}
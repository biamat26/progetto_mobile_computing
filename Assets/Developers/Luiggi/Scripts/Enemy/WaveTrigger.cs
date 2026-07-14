using UnityEngine;

/// <summary>
/// Mettilo su un oggetto interagibile (es. terminale/console).
/// Il player preme E per avviare le ondate.
/// </summary>
public class WaveTrigger : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject tooltipInteragisci;

    private bool playerVicino = false;
    private bool giaAttivato = false;

    void Awake()
    {
        if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
    }

    void Update()
    {
        if (giaAttivato) return;

        if (playerVicino && Input.GetKeyDown(KeyCode.E))
        {
            giaAttivato = true;
            if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
            if (playerVicino && Input.GetKeyDown(KeyCode.E))
{
    Debug.Log("E premuto, avvio ondate");
    giaAttivato = true;
    if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
    waveManager.StartWaves();
}
            waveManager.StartWaves();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger enter: " + other.gameObject.name + " tag=" + other.tag);
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

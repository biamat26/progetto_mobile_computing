using UnityEngine;
using UnityEngine.UI;

public class PowerDoorTerminal : MonoBehaviour
{
    [Header("Canvas terminale")]
    [SerializeField] private GameObject terminalCanvas;
    [SerializeField] private Button exitButton;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltip;

    [Header("Riferimento porta")]
    [SerializeField] private DoorPowerManager door;

    private bool playerVicino = false;
    private bool terminalAperto = false;

    void Awake()
    {
        if (terminalCanvas) terminalCanvas.SetActive(false);
        if (tooltip) tooltip.SetActive(false);
        if (exitButton) exitButton.onClick.AddListener(ChiudiTerminale);
    }

    void Update()
    {
        // non mostrare tooltip o terminale se la porta è già aperta
        if (door != null && door.IsOpen()) return;

        if (playerVicino && Input.GetKeyDown(KeyCode.E))
        {
            if (!terminalAperto) ApriTerminale();
        }

        if (terminalAperto && Input.GetKeyDown(KeyCode.Escape))
            ChiudiTerminale();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (door != null && door.IsOpen()) return;
        playerVicino = true;
        if (tooltip) tooltip.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerVicino = false;
        if (tooltip) tooltip.SetActive(false);
        ChiudiTerminale();
    }

    void ApriTerminale()
    {
        terminalAperto = true;
        if (tooltip) tooltip.SetActive(false);
        if (terminalCanvas) terminalCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    void ChiudiTerminale()
    {
        terminalAperto = false;
        if (terminalCanvas) terminalCanvas.SetActive(false);
        if (playerVicino && tooltip && (door == null || !door.IsOpen()))
            tooltip.SetActive(true);
        Time.timeScale = 1f;
    }
}

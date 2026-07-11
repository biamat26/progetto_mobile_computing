using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerDoorTerminal : MonoBehaviour
{
    [Header("Canvas terminale")]
    [SerializeField] private GameObject terminalCanvas;
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
    }

    void Update()
    {
        if (door != null && door.IsOpen()) return;
        if (playerVicino && Input.GetKeyDown(KeyCode.E))
        {
            if (!terminalAperto) ApriTerminale();
        }
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
        Time.timeScale = 1f;
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Gestisce il pannello terminale che appare quando il giocatore preme E vicino alla porta.
/// Attacca questo script a un GameObject vuoto nella scena (es. "DoorManager").
/// </summary>
public class TerminalPanel : MonoBehaviour
{
    [Header("Riferimenti UI (assegna dall'Inspector)")]
    public GameObject terminalPanel;       // Il Canvas Panel del terminale
    public GameObject tooltipPanel;        // Il tooltip "Interagisci" con tasto E

    [Header("Impostazioni")]
    public float interactionDistance = 3f; // Distanza massima per interagire
    public Transform door;                 // Transform della porta
    public Transform player;              // Transform del giocatore

    private bool isNearDoor = false;
    private bool isTerminalOpen = false;

    void Start()
    {
        // Assicurati che entrambi i pannelli siano nascosti all'avvio
        if (terminalPanel != null) terminalPanel.SetActive(false);
        if (tooltipPanel != null)  tooltipPanel.SetActive(false);
    }

    void Update()
    {
        CheckProximity();
        HandleInput();
    }

    void CheckProximity()
    {
        if (player == null || door == null) return;

        float dist = Vector2.Distance(player.position, door.position);
        isNearDoor = dist <= interactionDistance;

        // Mostra tooltip solo se vicino e terminale chiuso
        if (tooltipPanel != null)
            tooltipPanel.SetActive(isNearDoor && !isTerminalOpen);
    }

    void HandleInput()
    {
        if (!isNearDoor) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isTerminalOpen)
                OpenTerminal();
            else
                CloseTerminal();
        }

        // Chiudi con Escape
        if (isTerminalOpen && Input.GetKeyDown(KeyCode.Escape))
            CloseTerminal();
    }

    public void OpenTerminal()
    {
        isTerminalOpen = true;
        if (terminalPanel != null) terminalPanel.SetActive(true);
        if (tooltipPanel != null)  tooltipPanel.SetActive(false);

        // Opzionale: ferma il gioco / blocca il movimento del player qui
        // Time.timeScale = 0f;
    }

    public void CloseTerminal()
    {
        isTerminalOpen = false;
        if (terminalPanel != null) terminalPanel.SetActive(false);
    }
}
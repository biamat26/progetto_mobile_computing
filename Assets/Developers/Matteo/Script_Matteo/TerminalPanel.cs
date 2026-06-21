using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TerminalPanel : MonoBehaviour
{
    [Header("Riferimenti UI (assegna dall'Inspector)")]
    public GameObject terminalPanel;
    public GameObject tooltipPanel;

    [Header("Impostazioni")]
    public float interactionDistance = 3f;
    public Transform door;
    public Transform player;

    private bool isNearDoor = false;
    private bool isTerminalOpen = false;
    private bool isDoorOpen = false;

    void Start()
    {
        if (terminalPanel != null) terminalPanel.SetActive(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    void Update()
    {
        if (isDoorOpen) return;
        CheckProximity();
        HandleInput();
    }

    void CheckProximity()
    {
        if (player == null || door == null) return;
        float dist = Vector2.Distance(player.position, door.position);
        isNearDoor = dist <= interactionDistance;
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
        if (isTerminalOpen && Input.GetKeyDown(KeyCode.Escape))
            CloseTerminal();
    }

    public void OpenTerminal()
    {
        isTerminalOpen = true;
        if (terminalPanel != null) terminalPanel.SetActive(true);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    public void CloseTerminal()
    {
        isTerminalOpen = false;
        if (terminalPanel != null) terminalPanel.SetActive(false);
    }

    public void NotifyDoorOpen()
    {
        isDoorOpen = true;
        isNearDoor = false;
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (terminalPanel != null) terminalPanel.SetActive(false);
    }
}
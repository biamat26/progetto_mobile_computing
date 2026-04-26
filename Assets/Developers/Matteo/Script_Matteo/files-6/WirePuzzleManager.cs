using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WirePuzzleManager : MonoBehaviour
{
    [Header("Connettori")]
    public WireConnector[] leftConnectors;
    public WireConnector[] rightConnectors;

    [Header("Fili visivi")]
    public RectTransform[] wireLines;
    public RectTransform dragLine;
    public Image dragLineImage;

    [Header("UI")]
    public TMP_Text statusText;
    public GameObject successPanel;
    public Button exitButton;

    [Header("Canvas")]
    public GameObject puzzleCanvas;
    public Canvas canvas; // trascina qui il Canvas del puzzle

    private WireConnector draggingFrom = null;
    private int connectedCount = 0;
    private bool puzzleSolved = false;

    private Dictionary<string, int> colorToLineIndex = new Dictionary<string, int>
    {
        { "red",   0 },
        { "blue",  1 },
        { "green", 2 }
    };

    void Start()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(ClosePuzzle);

        foreach (var l in wireLines)
            l.gameObject.SetActive(false);

        if (dragLine != null) dragLine.gameObject.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);
    }

    void InitConnectors()
    {
        foreach (var c in leftConnectors)  c.manager = this;
        foreach (var c in rightConnectors) c.manager = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ClosePuzzle();
    }

    // ── Drag ─────────────────────────────────────────────
    public void BeginDrag(WireConnector from, PointerEventData e)
    {
        draggingFrom = from;
        if (dragLine == null) return;
        dragLine.gameObject.SetActive(true);
        if (dragLineImage != null)
            dragLineImage.color = from.GetColorForWire(from.wireColor);
        UpdateDragLine(from.lineStart.position, e.position);
    }

    public void OnDrag(PointerEventData e)
    {
        if (draggingFrom == null || dragLine == null) return;
        UpdateDragLine(draggingFrom.lineStart.position, e.position);
    }

    public void EndDrag(PointerEventData e)
    {
        if (dragLine != null) dragLine.gameObject.SetActive(false);

        if (draggingFrom != null)
        {
            // trova manualmente quale connettore destro è sotto il mouse
            WireConnector target = GetRightConnectorUnderMouse(e.position);
            if (target != null)
                TryConnect(target);
        }

        draggingFrom = null;
    }

    WireConnector GetRightConnectorUnderMouse(Vector2 screenPos)
    {
        foreach (var c in rightConnectors)
        {
            if (c.isConnected) continue;
            RectTransform rt = c.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos, canvas != null ? canvas.worldCamera : null))
            {
                Debug.Log("Trovato target: " + c.gameObject.name);
                return c;
            }
        }
        return null;
    }

    // ── Connessione ───────────────────────────────────────
    public void TryConnect(WireConnector target)
    {
        if (draggingFrom == null || puzzleSolved) return;
Debug.Log("TryConnect: from=" + draggingFrom.gameObject.name + " to=" + target.gameObject.name);
        Debug.Log("TryConnect: from=" + draggingFrom.wireColor + " to=" + target.wireColor);

        if (draggingFrom.wireColor == target.wireColor)
        {
            draggingFrom.SetConnected(true);
            target.SetConnected(true);

            int idx = colorToLineIndex[draggingFrom.wireColor];
            PositionWireLine(
                wireLines[idx],
                draggingFrom.lineStart.position,
                target.lineStart.position,
                draggingFrom.GetColorForWire(draggingFrom.wireColor));

            connectedCount++;
            UpdateStatus();

            if (connectedCount >= 3)
                StartCoroutine(PuzzleSolvedRoutine());
        }
        else
        {
            StartCoroutine(FlashError(target));
        }

        draggingFrom = null;
    }

    // ── Linee visive ─────────────────────────────────────
    void UpdateDragLine(Vector2 worldStart, Vector2 worldEnd)
    {
        if (dragLine == null) return;
        SetLineBetween(dragLine, worldStart, worldEnd);
    }

    void PositionWireLine(RectTransform line, Vector2 worldStart, Vector2 worldEnd, Color col)
    {
        line.gameObject.SetActive(true);
        var img = line.GetComponent<Image>();
        if (img != null) img.color = col;
        SetLineBetween(line, worldStart, worldEnd);
    }

    void SetLineBetween(RectTransform line, Vector2 worldStart, Vector2 worldEnd)
    {
        Vector2 dir    = worldEnd - worldStart;
        float distance = dir.magnitude;
        float angle    = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        line.position  = (worldStart + worldEnd) / 2f;
        line.sizeDelta = new Vector2(distance, line.sizeDelta.y);
        line.rotation  = Quaternion.Euler(0, 0, angle);
    }

    void UpdateStatus()
    {
        if (statusText != null)
            statusText.text = "Collegati: " + connectedCount + " / 3";
    }

    IEnumerator FlashError(WireConnector target)
    {
        var img = target.GetComponent<Image>();
        if (img == null) yield break;
        Color orig = img.color;
        img.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        img.color = orig;
    }

    IEnumerator PuzzleSolvedRoutine()
    {
        puzzleSolved = true;
        if (statusText != null) statusText.text = "ALIMENTAZIONE RIPRISTINATA";
        if (successPanel != null) successPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(1.5f);

        DoorPowerManager door = FindFirstObjectByType<DoorPowerManager>();
        if (door != null) door.PowerRestored();

        yield return new WaitForSecondsRealtime(1f);
        ClosePuzzle();
    }

    public void OpenPuzzle()
    {
        if (puzzleSolved) return;
        if (puzzleCanvas != null) puzzleCanvas.SetActive(true);
        InitConnectors();
        if (successPanel != null) successPanel.SetActive(false);
        UpdateStatus();
    }

    public void ClosePuzzle()
    {
        if (puzzleCanvas != null) puzzleCanvas.SetActive(false);
    }

    public bool IsSolved() => puzzleSolved;
}

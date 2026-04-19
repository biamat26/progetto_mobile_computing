using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Attacca questo script a un GameObject nella scena "ScenaTransizione".
/// La scena deve avere:
///   - Sfondo nero (Camera background = black)
///   - Un TextMeshProUGUI per il terminale (font monospace, verde #00FF41)
///   - Un TextMeshProUGUI per il contatore secondi (opzionale)
///
/// Dopo 5 secondi carica automaticamente la scena destinazione finale.
/// </summary>
public class HackerTransitionScene : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [SerializeField] private TextMeshProUGUI terminalText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Scena destinazione")]
    [Tooltip("Nome della scena finale da caricare dopo la transizione")]
    [SerializeField] private string finalSceneName = "ScenaBus";

    [Header("Timing")]
    [SerializeField] private float totalDuration = 5f;
    [SerializeField] private float typeSpeed = 0.04f;   // secondi per carattere

    // Righe del terminale in sequenza
    private readonly List<string> _lines = new()
    {
        "> Inizializzazione protocollo di trasporto...",
        "> Connessione al BUS-NET v4.2 [OK]",
        "> Autenticazione nodo: 0xA3F7-CC12 [OK]",
        "> Caricamento rotta: SECTOR_7 → MAINFRAME_HUB",
        "> Allocazione banda: 1024 Gbps [OK]",
        "> Compressione dati passeggero...",
        "  [##########----------]  48%",
        "  [####################]  100% [OK]",
        "> Teletrasporto in corso. Non disconnettersi.",
        "> ETA: 2 secondi...",
        "> Connessione stabilita. Benvenuto a bordo.",
    };

    private string _displayed = "";
    private float  _elapsed   = 0f;

    // ─── Start ─────────────────────────────────────────────────────

    private void Start()
    {
        if (terminalText != null) terminalText.text = "";
        StartCoroutine(RunTerminal());
    }

    // ─── Update timer ──────────────────────────────────────────────

    private void Update()
    {
        _elapsed += Time.deltaTime;

        if (timerText != null)
        {
            float remaining = Mathf.Max(0f, totalDuration - _elapsed);
            timerText.text = $"[ {remaining:F1}s ]";
        }

        if (_elapsed >= totalDuration)
        {
            // Ferma tutto e carica la scena finale
            StopAllCoroutines();
            StartCoroutine(LoadFinalScene());
            enabled = false; // evita chiamate Update ripetute
        }
    }

    // ─── Coroutine terminale ───────────────────────────────────────

    private IEnumerator RunTerminal()
    {
        foreach (string line in _lines)
        {
            // Typewriter carattere per carattere
            foreach (char ch in line)
            {
                _displayed += ch;
                if (terminalText != null)
                    terminalText.text = _displayed;
                yield return new WaitForSeconds(typeSpeed);
            }

            _displayed += "\n";
            if (terminalText != null)
                terminalText.text = _displayed;

            yield return new WaitForSeconds(0.15f); // pausa tra righe
        }

        // Cursore lampeggiante alla fine
        StartCoroutine(BlinkCursor());
    }

    private IEnumerator BlinkCursor()
    {
        bool visible = true;
        while (true)
        {
            if (terminalText != null)
                terminalText.text = _displayed + (visible ? "_" : " ");
            visible = !visible;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // ─── Carica scena finale ───────────────────────────────────────

    private IEnumerator LoadFinalScene()
    {
        // Fade out se SceneTransition è disponibile
        if (SceneTransition.Instance != null)
            yield return StartCoroutine(SceneTransition.Instance.FadeOut(0.6f));

        SceneManager.LoadScene(finalSceneName);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attacca al GameObject "Bus".
/// Richiede: BoxCollider2D con isTrigger = true.
/// Il player deve avere tag "Player".
///
/// Il tasto [E] appare SOLO quando il player entra nel collider
/// e scompare quando esce o inizia la transizione.
/// </summary>
public class BusInteractable : MonoBehaviour
{
    [Header("Scena destinazione")]
    [Tooltip("Nome della scena di transizione (quella con il terminale hacker)")]
    [SerializeField] private string transitionSceneName = "ScenaTransizione";

    [Header("Prompt")]
    [SerializeField] private string promptMessage = "[ E ]  Sali sul bus";

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.5f;

    private bool _playerInRange   = false;
    private bool _isTransitioning = false;

    // ─── Update ────────────────────────────────────────────────────

    private void Update()
    {
        if (_playerInRange && !_isTransitioning && Input.GetKeyDown(KeyCode.E))
            StartCoroutine(FadeAndLoad());
    }

    // ─── Trigger ───────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = true;
        UIPrompt.Instance?.Show(promptMessage);   // mostra [E]
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = false;
        UIPrompt.Instance?.Hide();                // nasconde [E]
    }

    // ─── Transizione ───────────────────────────────────────────────

    private System.Collections.IEnumerator FadeAndLoad()
    {
        _isTransitioning = true;
        UIPrompt.Instance?.Hide();

        if (SceneTransition.Instance != null)
            yield return StartCoroutine(SceneTransition.Instance.FadeOut(fadeDuration));

        // Carica la scena di transizione terminale (che poi caricherà la destinazione finale)
        SceneManager.LoadScene(transitionSceneName);
    }

    // ─── Gizmo ─────────────────────────────────────────────────────

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col == null) return;
        Gizmos.color = new Color(0f, 0.9f, 1f, 0.25f);
        Gizmos.DrawCube(transform.position + (Vector3)col.offset, col.size);
        Gizmos.color = new Color(0f, 0.9f, 1f, 0.8f);
        Gizmos.DrawWireCube(transform.position + (Vector3)col.offset, col.size);
    }
#endif
}
using UnityEngine;

/// <summary>
/// Attiva/disattiva l'intero PromptPanel quando il player è vicino al bus.
/// Nel campo "Prompt Panel" trascina il GameObject "PromptPanel" dal Canvas.
/// NON usa DontDestroyOnLoad — muore con la scena.
/// </summary>
public class UIPrompt : MonoBehaviour
{
    public static UIPrompt Instance { get; private set; }

    [Tooltip("Trascina qui il GameObject PromptPanel (l'intera UI col tasto E)")]
    [SerializeField] private GameObject promptPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Hide();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show(string message = "")
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);
    }

    public void Hide()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}
using UnityEngine;
using TMPro;

/// <summary>
/// Mostra il tasto [E] SOLO quando il player è vicino al bus.
/// NON usa DontDestroyOnLoad: viene distrutto al cambio scena.
/// Metti questo prefab solo nelle scene che ne hanno bisogno.
/// </summary>
public class UIPrompt : MonoBehaviour
{
    public static UIPrompt Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI promptText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // ❌ Niente DontDestroyOnLoad — muore con la scena corrente
        Hide();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show(string message)
    {
        if (promptText == null) return;
        promptText.text = message;
        promptText.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(false);
    }
}
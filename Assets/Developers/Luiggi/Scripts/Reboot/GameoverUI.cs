using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameoverUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public TextMeshProUGUI terminalText;
    public Button rebootButton;
    public GameObject overlay;

    [Header("Glitch titolo")]
    [SerializeField] private float glitchInterval = 0.8f;
    [SerializeField] private float glitchDuration = 0.07f;
    [SerializeField] private int glitchBurst = 3;

    private const string TitleLine = "<color=#22c55e>// SYSTEM FAILURE — kernel panic imminent</color>";
    private const string GlitchChars = "!@#$%<>?/|[]{}01█▓░";

    private readonly string[] lines = new string[]
    {
        "<color=#ef4444>[CRITICAL]</color> Integrity check failed — player.exe",
        "<color=#facc15>[WARNING] </color> Virus count exceeded threshold",
        "<color=#ef4444>[ERROR]  </color>  HP buffer underflow: 0x00000000",
        "<color=#60a5fa>[INFO]   </color>  Dumping memory state...",
        "<color=#4b5563>           0xDEAD 0xBEEF 0xCAFE 0xBABE</color>",
        "<color=#22c55e>[SYSTEM]  </color>  Reboot available. Press to restart."
    };

    // Testo sotto il titolo (aggiornato man mano che arrivano le righe)
    private string bodyText = "";
    private Coroutine glitchCoroutine;

    void Awake()
    {
        overlay.SetActive(false);
        panel.SetActive(false);
        rebootButton.gameObject.SetActive(false);
        rebootButton.onClick.AddListener(RestartLevel);

        ColorBlock colors = rebootButton.colors;
        colors.normalColor = new Color(0, 0, 0, 0);
        colors.highlightedColor = new Color(0.13f, 0.77f, 0.37f, 0.2f);
        colors.pressedColor = new Color(0.13f, 0.77f, 0.37f, 0.4f);
        colors.selectedColor = new Color(0, 0, 0, 0);
        rebootButton.colors = colors;
    }

public void Show()
{
    overlay.SetActive(true);
    panel.SetActive(true);
    
    // Nascondi il bottone terminale durante il game over
    if (TerminalManager.Istanza != null && TerminalManager.Istanza.bottoneTerminale != null)
        TerminalManager.Istanza.bottoneTerminale.SetActive(false);

    StartCoroutine(TypeLines());
}

    private IEnumerator TypeLines()
    {
        // Scrivi il titolo e avvia subito il glitch su di esso
        bodyText = "\n\n";
        terminalText.text = TitleLine + bodyText;
        glitchCoroutine = StartCoroutine(GlitchTitle());

        float[] delays = { 0.1f, 0.4f, 0.4f, 0.4f, 0.3f, 0.4f };

        for (int i = 0; i < lines.Length; i++)
        {
            yield return new WaitForSeconds(delays[i]);
            bodyText += lines[i] + "\n";
            terminalText.text = TitleLine + bodyText;
        }

        yield return new WaitForSeconds(0.4f);
        StartCoroutine(BlinkCursor());
        rebootButton.gameObject.SetActive(true);
    }

    // Glitch loop — gira in background e modifica solo la riga del titolo
    private IEnumerator GlitchTitle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(glitchInterval, glitchInterval * 2f));

            for (int b = 0; b < glitchBurst; b++)
            {
                // Glitch: sostituisce 1-3 caratteri nel testo PLAIN (senza tag colore)
                string plain = "// SYSTEM FAILURE — kernel panic imminent";
                char[] chars = plain.ToCharArray();
                int count = Random.Range(1, 4);
                for (int i = 0; i < count; i++)
                {
                    int idx = Random.Range(0, chars.Length);
                    if (chars[idx] != ' ' && chars[idx] != '/' && chars[idx] != '—')
                        chars[idx] = GlitchChars[Random.Range(0, GlitchChars.Length)];
                }

                string glitchedTitle = "<color=#ef4444>" + new string(chars) + "</color>";
                terminalText.text = glitchedTitle + bodyText;

                yield return new WaitForSeconds(glitchDuration);

                // Ripristina
                terminalText.text = TitleLine + bodyText;
                yield return new WaitForSeconds(0.04f);
            }
        }
    }

    private IEnumerator BlinkCursor()
    {
        bool show = true;
        while (true)
        {
            string cursor = show ? "<color=#22c55e>_</color>" : " ";
            terminalText.text = TitleLine + bodyText + cursor;
            show = !show;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
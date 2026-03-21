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
    private readonly string[] lines = new string[]
{
    "<color=#ef4444>[CRITICAL]</color> Integrity check failed — player.exe",
    "<color=#facc15>[WARNING] </color> Virus count exceeded threshold",
    "<color=#ef4444>[ERROR]  </color>  HP buffer underflow: 0x00000000",
    "<color=#60a5fa>[INFO]   </color>  Dumping memory state...",
    "<color=#4b5563>           0xDEAD 0xBEEF 0xCAFE 0xBABE</color>",
    "<color=#22c55e>[SYSTEM]  </color>  Reboot available. Press to restart."
};

   void Awake()
{   
    overlay.SetActive(false);
    panel.SetActive(false);
    rebootButton.gameObject.SetActive(false);
    rebootButton.onClick.AddListener(RestartLevel);

    // Stile button
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
        Debug.Log("GameOverUI.Show() chiamato!");
        panel.SetActive(true);
        StartCoroutine(TypeLines());
    }

    private IEnumerator TypeLines()
{
    terminalText.text = "<color=#22c55e>// SYSTEM FAILURE — kernel panic imminent</color>\n\n";

    float[] delays = { 0.1f, 0.4f, 0.4f, 0.4f, 0.3f, 0.4f };

    for (int i = 0; i < lines.Length; i++)
    {
        yield return new WaitForSeconds(delays[i]);
        terminalText.text += lines[i] + "\n";
    }

    yield return new WaitForSeconds(0.4f);
    StartCoroutine(BlinkCursor());
    rebootButton.gameObject.SetActive(true);
}

private IEnumerator BlinkCursor()
{
    bool show = true;
    string baseText = terminalText.text;
    while (true)
    {
        terminalText.text = baseText + (show ? "<color=#22c55e>_</color>" : " ");
        show = !show;
        yield return new WaitForSeconds(0.5f);
    }
}

    

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
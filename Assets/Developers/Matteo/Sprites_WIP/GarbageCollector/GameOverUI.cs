using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;  // trascina MessageText qui
    [SerializeField] private float fadeDuration = 1.2f;

    private string _baseMessage = "Il Garbage Collector ti ha preso!\nOggetto non referenziato.\nProcesso terminato";

    void OnEnable()
    {
        canvasGroup.alpha = 0f;
        StartCoroutine(FadeIn());
        StartCoroutine(BlinkCursor());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator BlinkCursor()
    {
        bool show = true;
        while (true)
        {
            messageText.text = _baseMessage + (show ? "_" : "  ");
            show = !show;
            yield return new WaitForSecondsRealtime(0.5f); // unscaled!
        }
    }
}
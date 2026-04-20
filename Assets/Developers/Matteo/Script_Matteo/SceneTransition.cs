using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestisce il fade nero tra scene.
/// Usa DontDestroyOnLoad SOLO per il pannello di fade,
/// niente controller, niente cerchi, niente UI extra.
///
/// Setup:
///   1. Crea un Canvas (Screen Space - Overlay, Sort Order 99)
///   2. Image child che copre tutto lo schermo, colore #000000, Alpha = 0
///   3. Attacca SceneTransition a un GameObject vuoto dentro il Canvas
///   4. Trascina l'Image nel campo fadeImage
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private bool fadeInOnStart = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Assicurati che non ci siano figli indesiderati (controller, joystick, ecc.)
        // Questo oggetto deve contenere SOLO il fade panel
        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = fadeInOnStart ? 1f : 0f;
            fadeImage.color = c;
        }
    }

    private void Start()
    {
        if (fadeInOnStart)
            StartCoroutine(FadeIn(0.8f));
    }

    public IEnumerator FadeOut(float duration) => Fade(0f, 1f, duration);
    public IEnumerator FadeIn(float duration)  => Fade(1f, 0f, duration);

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeImage == null) yield break;
        float elapsed = 0f;
        var c = fadeImage.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
    }
}
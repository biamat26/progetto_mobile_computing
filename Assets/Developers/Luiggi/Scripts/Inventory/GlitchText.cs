using UnityEngine;
using TMPro;
using System.Collections;

public class GlitchText : MonoBehaviour
{
    [SerializeField] private float glitchInterval = 0.2f;
    [SerializeField] private float glitchDuration = 0.1f;
    [SerializeField] private string glitchChars = "!@#$%<>?/|[]{}01";

    [Header("Opzioni extra (Game Over)")]
    [SerializeField] private bool useColorGlitch = false;
    [SerializeField] private bool useOffsetGlitch = false;
    [SerializeField] private float maxOffset = 6f;

    private TextMeshProUGUI tmp;
    private string originalText;
    private Color originalColor;
    private Vector3 originalPosition;
    private bool isGlitching = false;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        originalText = tmp.text;
        originalColor = tmp.color;
        originalPosition = tmp.rectTransform.anchoredPosition3D;
    }

    void OnEnable()
    {
        if (tmp != null && !string.IsNullOrEmpty(originalText))
        {
            tmp.text = originalText;
            tmp.color = originalColor;
            tmp.rectTransform.anchoredPosition3D = originalPosition;
        }
        isGlitching = false;
        StartCoroutine(GlitchLoop());
    }

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            // USIAMO REALTIME COSÌ FUNZIONA ANCHE IN PAUSA!
            yield return new WaitForSecondsRealtime(Random.Range(glitchInterval, glitchInterval * 2f));
            
            if (!isGlitching)
                StartCoroutine(GlitchOnce());
        }
    }

    IEnumerator GlitchOnce()
    {
        // Se il testo è stato cambiato da altri script (es. Terminale), aggiorniamo l'originalText
        if (tmp.text != originalText && !isGlitching) 
        {
            originalText = tmp.text;
        }

        // Se il testo è vuoto, non possiamo "glitcharlo", quindi ci fermiamo
        if (string.IsNullOrEmpty(originalText))
        {
            yield break; 
        }

        isGlitching = true;
        int numGlitches = Random.Range(1, 4);

        for (int g = 0; g < numGlitches; g++)
        {
            char[] chars = originalText.ToCharArray();

            // Sicura extra: se per qualche motivo non ci sono caratteri, usciamo
            if (chars.Length == 0) break;

            int count = Random.Range(1, 3);
            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, chars.Length);
                if (chars[index] != ' ' && chars[index] != '[' && chars[index] != ']')
                    chars[index] = glitchChars[Random.Range(0, glitchChars.Length)];
            }

            tmp.text = new string(chars);

            // Color glitch
            if (useColorGlitch)
            {
                Color[] glitchColors = { Color.red, Color.cyan, Color.white, new Color(1f, 0.2f, 0.2f) };
                tmp.color = glitchColors[Random.Range(0, glitchColors.Length)];
            }

            // Offset glitch
            if (useOffsetGlitch)
            {
                float ox = Random.Range(-maxOffset, maxOffset);
                float oy = Random.Range(-maxOffset * 0.4f, maxOffset * 0.4f);
                tmp.rectTransform.anchoredPosition3D = originalPosition + new Vector3(ox, oy, 0f);
            }

            // USIAMO REALTIME
            yield return new WaitForSecondsRealtime(glitchDuration);

            tmp.text = originalText;
            if (useColorGlitch) tmp.color = originalColor;
            if (useOffsetGlitch) tmp.rectTransform.anchoredPosition3D = originalPosition;

            // USIAMO REALTIME
            yield return new WaitForSecondsRealtime(0.04f);
        }

        isGlitching = false;
    }

    void OnDisable()
    {
        StopAllCoroutines();

        if (tmp != null && !string.IsNullOrEmpty(originalText))
        {
            tmp.text = originalText;
            tmp.color = originalColor;
            tmp.rectTransform.anchoredPosition3D = originalPosition;
            isGlitching = false;
        }
    }
}
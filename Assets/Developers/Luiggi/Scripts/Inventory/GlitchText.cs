using UnityEngine;
using TMPro;
using System.Collections;

public class GlitchText : MonoBehaviour
{
    [SerializeField] private float glitchInterval = 0.2f;
    [SerializeField] private float glitchDuration = 0.1f;
    [SerializeField] private string glitchChars = "!@#$%<>?/|[]{}01";

    private TextMeshProUGUI tmp;
    private string originalText;
    private bool isGlitching = false;

    void OnEnable()
{
    tmp = GetComponent<TextMeshProUGUI>();
    originalText = tmp.text;
    StartCoroutine(GlitchLoop());
}

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(glitchInterval, glitchInterval * 2f));
            if (!isGlitching)
                StartCoroutine(GlitchOnce());
        }
    }

    IEnumerator GlitchOnce()
    {
        isGlitching = true;

        int numGlitches = Random.Range(1, 4);
        
        for (int g = 0; g < numGlitches; g++)
        {
            char[] chars = originalText.ToCharArray();
            
            // glitcha 1-3 lettere contemporaneamente
            int count = Random.Range(1, 3);
            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, chars.Length);
                if (chars[index] != ' ' && chars[index] != '[' && chars[index] != ']')
                    chars[index] = glitchChars[Random.Range(0, glitchChars.Length)];
            }
            
            tmp.text = new string(chars);
            yield return new WaitForSeconds(glitchDuration);
            tmp.text = originalText;
            yield return new WaitForSeconds(0.04f);
        }

        isGlitching = false;
    }

    void OnDisable()
{
    StopAllCoroutines();
}
}
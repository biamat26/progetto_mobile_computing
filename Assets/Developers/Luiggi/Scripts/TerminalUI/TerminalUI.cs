using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TerminalUI : MonoBehaviour
{
    [Header("Componenti")]
    public TextMeshProUGUI terminalText;
    public ScrollRect scrollRect;

    [Header("Impostazioni")]
    public float delayLettera = 0.01f;
    [Range(0f, 0.2f)] public float sogliaAutoScroll = 0.1f;

    private Coroutine typingCoroutine;
    private float currentDelay;
    private bool isTyping = false;
    private bool utenteHaScrollato = false;

    public void ScriviMessaggio(string messaggio, bool usaEffettoHacker)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        utenteHaScrollato = false;

        if (usaEffettoHacker)
            typingCoroutine = StartCoroutine(EffettoMacchinaDaScrivere(messaggio));
        else
        {
            terminalText.text = messaggio + "_";
            StartCoroutine(ForzaScrollGiù());
            typingCoroutine = StartCoroutine(SoloCursore(messaggio));
        }
    }

    private IEnumerator EffettoMacchinaDaScrivere(string messaggio)
    {
        terminalText.text = "";
        currentDelay = delayLettera;
        isTyping = true;

        foreach (char lettera in messaggio.ToCharArray())
        {
            terminalText.text += lettera;

            // autoscroll solo se utente non ha scrollato manualmente
            if (!utenteHaScrollato && scrollRect != null)
            {
                bool èMini = TerminalManager.Istanza != null && !TerminalManager.Istanza.isExpanded;
                if (èMini)
                {
                    Canvas.ForceUpdateCanvases();
                    scrollRect.verticalNormalizedPosition = 0f;
                }
                else
                {
                    Canvas.ForceUpdateCanvases();
                    scrollRect.verticalNormalizedPosition = 0f;
                }
            }

            yield return new WaitForSecondsRealtime(currentDelay);
        }

        isTyping = false;
        yield return SoloCursore(terminalText.text);
    }

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0)
            utenteHaScrollato = true;

        if (!isTyping) return;

        if (scroll < 0) // verso il basso = accelera
        {
            currentDelay = Mathf.Max(0.001f, currentDelay - Mathf.Abs(scroll) * 0.05f);
        }
        else if (scroll > 0) // verso l'alto = rallenta
            currentDelay = Mathf.Min(delayLettera, currentDelay + scroll * 0.002f);
    }

    private IEnumerator SoloCursore(string testoFisso)
    {
        while (true)
        {
            terminalText.text = testoFisso + "_";
            yield return new WaitForSecondsRealtime(0.5f);
            terminalText.text = testoFisso + " ";
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    private IEnumerator ForzaScrollGiù()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
    }
}
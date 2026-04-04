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
    public float delayLettera = 0.05f;
    [Range(0f, 0.2f)] public float sogliaAutoScroll = 0.1f; // Se l'utente è vicino al fondo, continua l'auto-scroll

    private Coroutine typingCoroutine;

    public void ScriviMessaggio(string messaggio, bool usaEffettoHacker)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        if (usaEffettoHacker)
        {
            typingCoroutine = StartCoroutine(EffettoMacchinaDaScrivere(messaggio));
        }
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
        
        foreach (char lettera in messaggio.ToCharArray())
        {
            terminalText.text += lettera;
            
            // --- AUTO SCROLL INTELLIGENTE ---
            if (scrollRect != null) 
            {
                // Scrolliamo automaticamente SOLO se l'utente è già vicino al fondo (entro la soglia)
                // O se il terminale è piccolo (Mini), dove lo scroll manuale non serve
                bool èInBasso = scrollRect.verticalNormalizedPosition <= sogliaAutoScroll;
                bool èMini = TerminalManager.Istanza != null && !TerminalManager.Istanza.isExpanded;

                if (èInBasso || èMini)
                {
                    // Forza il testo ad aggiornare il suo layout prima di scrollare
                    Canvas.ForceUpdateCanvases();
                    scrollRect.verticalNormalizedPosition = 0f;
                }
            }

            yield return new WaitForSecondsRealtime(delayLettera);
        }

        yield return SoloCursore(terminalText.text);
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
        // Aspetta due frame per essere sicuri che TMPro abbia calcolato la lunghezza del testo
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
    }
}
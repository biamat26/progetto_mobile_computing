using System.Collections;
using UnityEngine;
using TMPro;

public class TerminalUI : MonoBehaviour
{
    [Header("Componenti")]
    public TextMeshProUGUI terminalText; 

    [Header("Impostazioni")]
    public float delayLettera = 0.05f;

    private Coroutine typingCoroutine;

    // Ora riceve anche un "bool" per sapere se fare l'animazione o no
    public void ScriviMessaggio(string messaggio, bool usaEffettoHacker)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        if (usaEffettoHacker)
        {
            // Scrive lettera per lettera
            typingCoroutine = StartCoroutine(EffettoMacchinaDaScrivere(messaggio));
        }
        else
        {
            // Stampa tutto istantaneamente e fa partire solo il cursore
            terminalText.text = messaggio + "_";
            typingCoroutine = StartCoroutine(SoloCursore(messaggio));
        }
    }

    private IEnumerator EffettoMacchinaDaScrivere(string messaggio)
    {
        terminalText.text = "";
        
        foreach (char lettera in messaggio.ToCharArray())
        {
            terminalText.text += lettera;
            yield return new WaitForSecondsRealtime(delayLettera);
        }

        // Finito di scrivere, passa al cursore lampeggiante
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
}
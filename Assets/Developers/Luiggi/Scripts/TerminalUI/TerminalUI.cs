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

    public void ScriviMessaggio(string messaggio)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        typingCoroutine = StartCoroutine(EffettoMacchinaDaScrivere(messaggio));
    }

    private IEnumerator EffettoMacchinaDaScrivere(string messaggio)
    {
        terminalText.text = "";
        
        // 1. Scrittura lettera per lettera
        foreach (char lettera in messaggio.ToCharArray())
        {
            terminalText.text += lettera;
            yield return new WaitForSeconds(delayLettera);
        }

        // 2. Cursore lampeggiante infinito
        string testoFisso = terminalText.text;
        
        while (true)
        {
            terminalText.text = testoFisso + "_";
            yield return new WaitForSeconds(0.5f);
            terminalText.text = testoFisso + " ";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TerminalIntro : MonoBehaviour
{
    [Header("UI Terminale")]
    public TextMeshProUGUI terminalText;
    public string gameSceneName = "Test_HD";
    public float velocitaDigitazione = 0.03f;

    [Header("Pulsante Continua")]
    public Button continuaButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingSound;

    // Testo dell'introduzione — modifica pure il contenuto qui
    private string testoIntro =
        "> CONNESSIONE STABILITA...\n" +
        "> AVVIO PROTOCOLLO DI EMERGENZA...\n" +
        "\n" +
        "ANOMALIA RILEVATA: coscienza biologica convertita in codice binario.\n" +
        "\n" +
        "Stato attuale: sei all'interno dell'architettura fisica del sistema.\n" +
        "Il settore risulta compromesso — un'infezione malware su larga scala sta corrompendo i flussi di dati in tempo reale.\n" +
        "\n" +
        "DIRETTIVA PRIMARIA:\n" +
        "Neutralizzare ogni minaccia virale attiva.\n" +
        "Aprire un varco sicuro verso la memoria RAM.\n" +
        "\n" +
        "La stabilita' dell'intero sistema dipende dalla tua azione.\n";

    void Start()
    {
        if (terminalText != null) terminalText.text = "";
        if (continuaButton != null)
        {
            continuaButton.gameObject.SetActive(false);
            continuaButton.onClick.AddListener(VaiAllaScenaDiGioco);
        }

        StartCoroutine(EseguiIntro());
    }

    IEnumerator EseguiIntro()
    {
        yield return new WaitForSeconds(1f);

        string[] righe = testoIntro.Split('\n');
        foreach (string riga in righe)
        {
            if (string.IsNullOrEmpty(riga))
            {
                terminalText.text += "\n";
                yield return new WaitForSeconds(0.4f);
                continue;
            }

            foreach (char lettera in riga.ToCharArray())
            {
                terminalText.text += lettera;
                if (audioSource != null && typingSound != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(typingSound, 0.4f);
                }
                yield return new WaitForSeconds(velocitaDigitazione);
            }

            terminalText.text += "\n";
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        // mostra il pulsante Continua
        if (continuaButton != null)
            continuaButton.gameObject.SetActive(true);
    }

    void VaiAllaScenaDiGioco()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
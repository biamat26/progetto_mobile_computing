using UnityEngine;
using UnityEngine.SceneManagement;

public class GestoreUscita : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pannelloConferma; // Trascina qui il tuo Panel_Conferma

    [Header("Settings")]
    public string nomeScenaMenuPrincipale = "MainMenu"; // Inserisci il nome esatto della tua scena del Menu

    void Start()
    {
        // Assicuriamoci che all'avvio il pannello sia nascosto
        if (pannelloConferma != null)
        {
            pannelloConferma.SetActive(false);
        }
    }

    // Metodo assegnato al bottone in alto a destra
    public void ApriSchermataConferma()
    {
        pannelloConferma.SetActive(true);
        Time.timeScale = 0f; // (Opzionale) Mette in pausa il gioco
    }

    // Metodo assegnato al bottone "No"
    public void ChiudiSchermataConferma()
    {
        pannelloConferma.SetActive(false);
        Time.timeScale = 1f; // (Opzionale) Fa ripartire il gioco
    }

    // Metodo assegnato al bottone "Sì"
    public void TornaAlMenuPrincipale()
    {
        Time.timeScale = 1f; // Riporta il tempo alla normalità prima di cambiare scena
        SceneManager.LoadScene(1);
    }
}
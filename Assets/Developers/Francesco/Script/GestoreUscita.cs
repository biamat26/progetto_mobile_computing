using UnityEngine;
using UnityEngine.SceneManagement;

public class GestoreUscita : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pannelloConferma; 

    [Header("Settings")]
    public string nomeScenaMenuPrincipale = "MainMenu"; 

    void Start()
    {
        if (pannelloConferma != null)
        {
            pannelloConferma.SetActive(false);
        }
    }

    public void ApriSchermataConferma()
    {
        pannelloConferma.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ChiudiSchermataConferma()
    {
        pannelloConferma.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void TornaAlMenuPrincipale()
    {
        // --- LOGICA DI SALVATAGGIO ---
        string emailUtente = "Ospite";
        
        if (UserSession.Instance != null && !string.IsNullOrEmpty(UserSession.Instance.Email))
        {
            emailUtente = UserSession.Instance.Email;
        }
        
        string chiave = "ScenaSalvata_" + emailUtente;
        int scenaAttuale = SceneManager.GetActiveScene().buildIndex;

        // Salva la scena attuale associandola alla mail
        PlayerPrefs.SetInt(chiave, scenaAttuale);
        PlayerPrefs.Save(); 

        Debug.Log("Partita salvata per: " + emailUtente + " alla scena " + scenaAttuale);
        // -----------------------------

        Time.timeScale = 1f; 
        
        // Assicurati che l'indice '1' corrisponda effettivamente alla tua scena del Menù Principale nei Build Settings!
        SceneManager.LoadScene(1); 
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour 
{
    [Header("UI Menù")]
    public GameObject pannelloScelta;
    public TextMeshProUGUI testoMessaggio; 

    [Header("Impostazioni")]
    [Tooltip("La scena da cui parte una Nuova Partita")]
    public int indicePrimaScena = 2; 

    [Tooltip("Il nome esatto della tua scena di caricamento")]
    public string nomeScenaDownload = "SchermataDownload";

    private string GetChiaveSalvataggio()
    {
        string emailUtente = "Ospite"; 
        
        if (UserSession.Instance != null && !string.IsNullOrEmpty(UserSession.Instance.Email))
        {
            emailUtente = UserSession.Instance.Email;
        }
        
        return "ScenaSalvata_" + emailUtente;
    }

    private string GetNomeGiocatore()
    {
        if (UserSession.Instance != null && !string.IsNullOrEmpty(UserSession.Instance.Username))
        {
            return UserSession.Instance.Username;
        }
        return "Ospite"; 
    }

    public void ApriPannelloGioca() 
    {
        pannelloScelta.SetActive(true);
        if (testoMessaggio != null) testoMessaggio.text = ""; 
    }

    public void NuovaPartita() 
    {
        if (testoMessaggio != null) 
        {
            string nome = GetNomeGiocatore();
            testoMessaggio.text = "Accesso a nuova partita... Benvenuto, " + nome + "!";
        }
        
        // Se non è un ospite, cancella il salvataggio precedente per ricominciare da zero
        if (GetNomeGiocatore() != "Ospite")
        {
            string chiave = GetChiaveSalvataggio();
            PlayerPrefs.DeleteKey(chiave); 
        }
        
        StartCoroutine(CaricaScenaConRitardo(indicePrimaScena));
    }

    public void CaricaPartita() 
    {
        if (GetNomeGiocatore() == "Ospite")
        {
            if (testoMessaggio != null) testoMessaggio.text = "ERRORE: Gli account Ospite non possono caricare salvataggi!";
            return; // Ferma la funzione qui, non va avanti a caricare!
        }

        string chiave = GetChiaveSalvataggio();

        if (PlayerPrefs.HasKey(chiave))
        {
            if (testoMessaggio != null) 
            {
                string nome = GetNomeGiocatore();
                testoMessaggio.text = "Accesso a partita caricata... Bentornato, " + nome + "!";
            }
            
            int scenaDaCaricare = PlayerPrefs.GetInt(chiave);
            StartCoroutine(CaricaScenaConRitardo(scenaDaCaricare));
        }
        else
        {
            if (testoMessaggio != null) testoMessaggio.text = "Nessuna partita salvata per questo account!";
        }
    }

    private IEnumerator CaricaScenaConRitardo(int indiceScena)
    {
        // 1. Salviamo l'INDICE numerico della scena in un PlayerPrefs
        PlayerPrefs.SetInt("IndiceScenaDestinazione", indiceScena);
        PlayerPrefs.Save();

        // Aspetta che finisca il testo a schermo
        yield return new WaitForSeconds(1.5f); 
        
        // 2. Carica la barra di simulazione!
        SceneManager.LoadSceneAsync(nomeScenaDownload);
    }

    public void QuitGame() 
    {
        Application.Quit();
    }
}
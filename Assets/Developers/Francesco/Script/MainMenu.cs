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

    // --- FUNZIONE PER RECUPERARE LA MAIL ---
    private string GetChiaveSalvataggio()
    {
        string emailUtente = "Ospite"; 
        
        if (UserSession.Instance != null && !string.IsNullOrEmpty(UserSession.Instance.Email))
        {
            emailUtente = UserSession.Instance.Email;
        }
        
        return "ScenaSalvata_" + emailUtente;
    }

    public void ApriPannelloGioca() 
    {
        pannelloScelta.SetActive(true);
        if (testoMessaggio != null) testoMessaggio.text = ""; 
    }

    public void NuovaPartita() 
    {
        if (testoMessaggio != null) testoMessaggio.text = "Accesso a nuova partita...";
        
        string chiave = GetChiaveSalvataggio();
        
        // Cancella SOLO il salvataggio di questa specifica email
        PlayerPrefs.DeleteKey(chiave); 
        
        StartCoroutine(CaricaScenaConRitardo(indicePrimaScena));
    }

    public void CaricaPartita() 
    {
        string chiave = GetChiaveSalvataggio();

        // Controlla se esiste il salvataggio per QUESTO utente
        if (PlayerPrefs.HasKey(chiave))
        {
            if (testoMessaggio != null) testoMessaggio.text = "Accesso a partita caricata...";
            
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
        yield return new WaitForSeconds(1.5f); 
        SceneManager.LoadSceneAsync(indiceScena);
    }

    public void QuitGame() 
    {
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ================================================================
// MainMenuUI.cs
// Posizione: Assets/Scripts/UI/
//
// SETUP IN UNITY:
// 1. Attacca questo script al Canvas della MainMenuScene
// 2. Trascina i riferimenti negli slot dell'Inspector
// ================================================================

public class MainMenuUI : MonoBehaviour
{
    [Header("Pulsanti principali")]
    [SerializeField] private Button btnGioca;
    [SerializeField] private Button btnImpostazioni;
    [SerializeField] private Button btnEsci;

    [Header("Pannello Impostazioni")]
    [SerializeField] private GameObject panelSettings; // Overlay impostazioni

    [Header("Info utente")]
    [SerializeField] private TMP_Text txtBenvenuto; // Es: "Ciao, mario99!"

    private void Start()
    {
        // Mostra nome utente
        if (UserSession.Instance != null && txtBenvenuto != null)
            txtBenvenuto.text = $"Ciao, {UserSession.Instance.Username}!";

        // Collega pulsanti
        btnGioca.onClick.AddListener(OnGiocaClicked);
        btnImpostazioni.onClick.AddListener(OnImpostazioniClicked);
        btnEsci.onClick.AddListener(OnEsciClicked);

        // Nasconde pannello impostazioni
        if (panelSettings != null)
            panelSettings.SetActive(false);
    }

    private void OnGiocaClicked()
    {
        AudioManager.Instance?.PlayClick();
        GameManager.Instance.GoToCharacterSelect();
    }

    private void OnImpostazioniClicked()
    {
        AudioManager.Instance?.PlayClick();
        if (panelSettings != null)
            panelSettings.SetActive(!panelSettings.activeSelf);
    }

    private void OnEsciClicked()
    {
        AudioManager.Instance?.PlayClick();
        AuthManager.Instance?.Logout();
        GameManager.Instance.GoToLogin();
    }
}
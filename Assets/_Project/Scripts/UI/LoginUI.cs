using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// ================================================================
// LoginUI.cs
// Da attaccare al pannello Login nella LoginScene
//
// SETUP IN UNITY:
// 1. Crea un Canvas con due pannelli: PanelLogin e PanelRegister
// 2. In ogni pannello metti: InputField Email, InputField Password,
//    Button conferma, Button switch pannello, Text errore
// 3. Trascina i riferimenti negli slot qui sotto nell'Inspector
// 4. Crea un GameObject vuoto "AuthManager" e aggiungi AuthManager.cs
// ================================================================

public class LoginUI : MonoBehaviour
{
    [Header("Pannello Login")]
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private Button        btnLogin;
    [SerializeField] private Button        btnGoToRegister;
    [SerializeField] private TMP_Text      loginErrorText;
    [SerializeField] private Toggle        rememberMeToggle;

    [Header("Pannello Registrazione")]
    [SerializeField] private GameObject    panelRegister;
    [SerializeField] private TMP_InputField regEmailInput;
    [SerializeField] private TMP_InputField regPasswordInput;
    [SerializeField] private TMP_InputField regPasswordConfirmInput;
    [SerializeField] private Button        btnRegister;
    [SerializeField] private Button        btnGoToLogin;
    [SerializeField] private TMP_Text      registerErrorText;

    [Header("Loading")]
    [SerializeField] private GameObject loadingPanel; // Pannello "Caricamento..."

    [Header("Scena successiva")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    // -------------------------------------------------------
    private void Start()
    {
        // Nasconde errori all'avvio
        HideErrors();

        // Collega i pulsanti
        btnLogin.onClick.AddListener(OnLoginClicked);
        btnRegister.onClick.AddListener(OnRegisterClicked);
        btnGoToRegister.onClick.AddListener(() => SwitchPanel(false));
        btnGoToLogin.onClick.AddListener(() => SwitchPanel(true));

        // Mostra pannello login di default
        SwitchPanel(true);

        // Auto-login se sessione salvata
        if (AuthManager.Instance.HasSavedSession())
        {
            ShowLoading(true);
            AuthManager.Instance.AutoLogin(OnLoginSuccess, (err) => {
                ShowLoading(false);
                // Sessione scaduta, mostra login normalmente
            });
        }
    }

    // ================================================================
    // LOGIN
    // ================================================================
    private void OnLoginClicked()
    {
        HideErrors();
        string email    = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text;

        ShowLoading(true);
        btnLogin.interactable = false;

        AuthManager.Instance.Login(email, password,
            onSuccess: () =>
            {
                // Salva sessione se "ricordami" è spuntato
                if (rememberMeToggle != null && rememberMeToggle.isOn)
                    AuthManager.Instance.SaveSession(email, password);

                OnLoginSuccess();
            },
            onError: (errorMsg) =>
            {
                ShowLoading(false);
                btnLogin.interactable = true;
                ShowError(loginErrorText, errorMsg);
            }
        );
    }

    // ================================================================
    // REGISTRAZIONE
    // ================================================================
    private void OnRegisterClicked()
    {
        HideErrors();

        string email    = regEmailInput.text.Trim();
        string password = regPasswordInput.text;
        string confirm  = regPasswordConfirmInput.text;

        // Controllo password corrispondenti
        if (password != confirm)
        {
            ShowError(registerErrorText, "Le password non corrispondono.");
            return;
        }

        ShowLoading(true);
        btnRegister.interactable = false;

        AuthManager.Instance.Register(email, password,
            onSuccess: () =>
            {
                OnLoginSuccess(); // Dopo registrazione → vai al menu
            },
            onError: (errorMsg) =>
            {
                ShowLoading(false);
                btnRegister.interactable = true;
                ShowError(registerErrorText, errorMsg);
            }
        );
    }

    // ================================================================
    // DOPO LOGIN/REGISTRAZIONE OK
    // ================================================================
    private void OnLoginSuccess()
    {
        ShowLoading(false);
        Debug.Log($"[LoginUI] Accesso OK come: {AuthManager.Instance.UserEmail}");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ================================================================
    // UTILITY UI
    // ================================================================
    private void SwitchPanel(bool showLogin)
    {
        panelLogin.SetActive(showLogin);
        panelRegister.SetActive(!showLogin);
        HideErrors();
    }

    private void ShowError(TMP_Text textField, string message)
    {
        if (textField != null)
        {
            textField.text = message;
            textField.gameObject.SetActive(true);
        }
    }

    private void HideErrors()
    {
        if (loginErrorText    != null) loginErrorText.gameObject.SetActive(false);
        if (registerErrorText != null) registerErrorText.gameObject.SetActive(false);
    }

    private void ShowLoading(bool show)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(show);
    }
}
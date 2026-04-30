using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TerminalLoginUI : MonoBehaviour
{
    [Header("Terminal Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip keyPressSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip successSound;

    [Header("Pannelli Terminale")]
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private TMP_Text loginConsoleOutput; // Sostituisce loginErrorText

    [Space(10)]
    [SerializeField] private GameObject panelRegister;
    [SerializeField] private TMP_InputField regEmailInput;
    [SerializeField] private TMP_InputField regPasswordInput;
    [SerializeField] private TMP_InputField regPasswordConfirmInput;
    [SerializeField] private TMP_Text registerConsoleOutput; // Sostituisce registerErrorText

    [Header("Sistema")]
    [SerializeField] private TMP_Text systemStatusText; // Pannello loading stile terminale
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    
    private bool isProcessing = false;

    private void Start()
    {
        ClearConsole();
        SwitchToLogin();

        // Auto-login se sessione salvata
        if (AuthManager.Instance.HasSavedSession())
        {
            StartCoroutine(TypewriterEffect(systemStatusText, "> INIZIALIZZAZIONE AUTO-LOGIN..."));
            AuthManager.Instance.AutoLogin(OnLoginSuccess, (err) => {
                ClearConsole();
            });
        }
    }

    private void Update()
    {
        if (isProcessing) return;

        // Permette di premere INVIO (Enter) per eseguire l'azione
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (panelLogin.activeSelf)
                ExecuteLogin();
            else if (panelRegister.activeSelf)
                ExecuteRegister();
        }

        // Permette di cambiare pannello con il tasto TAB (opzionale)
        if (Input.GetKeyDown(KeyCode.Tab) && !loginEmailInput.isFocused && !loginPasswordInput.isFocused)
        {
            if (panelLogin.activeSelf) SwitchToRegister();
            else SwitchToLogin();
        }
    }

    // ================================================================
    // LOGICA LOGIN & REGISTRAZIONE
    // ================================================================
    public void ExecuteLogin() // Puoi collegarlo anche a un bottone finto se vuoi
    {
        string email = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text;

        isProcessing = true;
        StartCoroutine(TypewriterEffect(loginConsoleOutput, "> ESECUZIONE LOGIN.EXE... ATTENDERE"));

        AuthManager.Instance.Login(email, password,
            onSuccess: () =>
            {
                PlaySound(successSound);
                StartCoroutine(TypewriterEffect(loginConsoleOutput, "> ACCESSO CONSENTITO. BENVENUTO."));
                Invoke(nameof(OnLoginSuccess), 1.5f); // Aspetta un secondo per far leggere il messaggio
            },
            onError: (errorMsg) =>
            {
                PlaySound(errorSound);
                StartCoroutine(TypewriterEffect(loginConsoleOutput, $"> ERRORE DI SISTEMA: {errorMsg}"));
                isProcessing = false;
            }
        );
    }

    public void ExecuteRegister()
    {
        string email = regEmailInput.text.Trim();
        string password = regPasswordInput.text;
        string confirm = regPasswordConfirmInput.text;

        if (password != confirm)
        {
            PlaySound(errorSound);
            StartCoroutine(TypewriterEffect(registerConsoleOutput, "> ERRORE: LE PASSWORD NON CORRISPONDONO."));
            return;
        }

        isProcessing = true;
        StartCoroutine(TypewriterEffect(registerConsoleOutput, "> CREAZIONE NUOVO NODO UTENTE..."));

        AuthManager.Instance.Register(email, password,
            onSuccess: () =>
            {
                PlaySound(successSound);
                StartCoroutine(TypewriterEffect(registerConsoleOutput, "> REGISTRAZIONE COMPLETATA. TRASFERIMENTO DATI..."));
                Invoke(nameof(OnLoginSuccess), 1.5f);
            },
            onError: (errorMsg) =>
            {
                PlaySound(errorSound);
                StartCoroutine(TypewriterEffect(registerConsoleOutput, $"> ERRORE DI SISTEMA: {errorMsg}"));
                isProcessing = false;
            }
        );
    }

    private void OnLoginSuccess()
    {
        UserSession.Instance.SetSession(AuthManager.Instance.UserEmail, AuthManager.Instance.UserId, AuthManager.Instance.UserToken);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ================================================================
    // UTILITY TERMINALE E ANIMAZIONI
    // ================================================================
    public void SwitchToLogin()
    {
        panelLogin.SetActive(true);
        panelRegister.SetActive(false);
        ClearConsole();
        loginEmailInput.Select(); // Seleziona in automatico il campo email
    }

    public void SwitchToRegister()
    {
        panelLogin.SetActive(false);
        panelRegister.SetActive(true);
        ClearConsole();
        regEmailInput.Select();
    }

    private void ClearConsole()
    {
        if (loginConsoleOutput != null) loginConsoleOutput.text = "> _";
        if (registerConsoleOutput != null) registerConsoleOutput.text = "> _";
        if (systemStatusText != null) systemStatusText.text = "";
    }

    // Effetto "Macchina da scrivere" tipico dei terminali
    private IEnumerator TypewriterEffect(TMP_Text textBox, string fullText)
    {
        textBox.text = "";
        foreach (char c in fullText)
        {
            textBox.text += c;
            PlaySound(keyPressSound, 0.5f); // Suono più basso per la digitazione rapida
            yield return new WaitForSeconds(0.02f); // Velocità di digitazione
        }
        textBox.text += " \u2588"; // Aggiunge il blocco solido finale (cursore)
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
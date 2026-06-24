using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TerminalLoginUI : MonoBehaviour
{
    [Header("Terminal Audio")]
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private AudioClip keyPressSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip successSound;

    [Header("Pannelli Terminale")]
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private TMP_Text loginConsoleOutput; 

    [Space(10)]
    [SerializeField] private GameObject panelRegister;
    [SerializeField] private TMP_InputField regEmailInput;
    [SerializeField] private TMP_InputField regPasswordInput;
    [SerializeField] private TMP_InputField regPasswordConfirmInput;
    [SerializeField] private TMP_Text registerConsoleOutput; 

    [Header("Sistema")]
    [SerializeField] private TMP_Text systemStatusText; 
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    
    private bool isProcessing = false;

    // Variabili per tenere traccia delle animazioni in corso e non farle accavallare
    private Coroutine loginTypingCoroutine;
    private Coroutine registerTypingCoroutine;
    private Coroutine systemTypingCoroutine;

    private void Start()
    {
        ClearConsole();
        SwitchToLogin();

        // Auto-login se sessione salvata
        if (AuthManager.Instance.HasSavedSession())
        {
            PrintToSystemConsole("> INIZIALIZZAZIONE AUTO-LOGIN...");
            AuthManager.Instance.AutoLogin(
                onSuccess: () => 
                {
                    AuthManager.Instance.CheckEmailVerificationStatus(
                        onComplete: (isVerified) => 
                        {
                            if (isVerified) 
                            {
                                OnLoginSuccess();
                            } 
                            else 
                            {
                                AuthManager.Instance.Logout();
                                ClearConsole();
                                PrintToSystemConsole("> AUTO-LOGIN FALLITO: EMAIL NON CONFERMATA.");
                            }
                        },
                        onError: (err) => 
                        {
                            AuthManager.Instance.Logout();
                            ClearConsole();
                        }
                    );
                }, 
                onError: (err) => {
                    ClearConsole();
                }
            );
        }
    }

    private void Update()
    {
        if (isProcessing) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (panelLogin.activeSelf)
                ExecuteLogin();
            else if (panelRegister.activeSelf)
                ExecuteRegister();
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !loginEmailInput.isFocused && !loginPasswordInput.isFocused)
        {
            if (panelLogin.activeSelf) SwitchToRegister();
            else SwitchToLogin();
        }
    }

    // ================================================================
    // LOGICA LOGIN
    // ================================================================
    public void ExecuteLogin() 
    {
        string email = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text;

        isProcessing = true;
        PrintToLoginConsole("> ESECUZIONE LOGIN.EXE... ATTENDERE");

        AuthManager.Instance.Login(email, password,
            onSuccess: () =>
            {
                AuthManager.Instance.CheckEmailVerificationStatus(
                    onComplete: (isVerified) => 
                    {
                        if (isVerified) 
                        {
                            PlaySound(successSound);
                            PrintToLoginConsole("> ACCESSO CONSENTITO. BENVENUTO.");
                            Invoke(nameof(OnLoginSuccess), 1.5f); 
                        }
                        else
                        {
                            PlaySound(errorSound);
                            PrintToLoginConsole("> ACCESSO NEGATO: CONFERMARE EMAIL. CONTROLLARE CASELLA POSTALE.");
                            AuthManager.Instance.Logout();
                            isProcessing = false;
                        }
                    },
                    onError: (errorMsg) => 
                    {
                        PlaySound(errorSound);
                        PrintToLoginConsole($"> ERRORE DI SISTEMA: {errorMsg}");
                        AuthManager.Instance.Logout();
                        isProcessing = false;
                    }
                );
            },
            onError: (errorMsg) =>
            {
                PlaySound(errorSound);
                PrintToLoginConsole($"> ERRORE DI SISTEMA: {errorMsg}");
                isProcessing = false;
            }
        );
    }

    // ================================================================
    // LOGICA REGISTRAZIONE
    // ================================================================
    public void ExecuteRegister()
    {
        string email = regEmailInput.text.Trim();
        string password = regPasswordInput.text;
        string confirm = regPasswordConfirmInput.text;

        if (password != confirm)
        {
            PlaySound(errorSound);
            PrintToRegisterConsole("> ERRORE: LE PASSWORD NON CORRISPONDONO.");
            return;
        }

        isProcessing = true;
        PrintToRegisterConsole("> CREAZIONE NUOVO NODO UTENTE...");

        AuthManager.Instance.Register(email, password,
            onSuccess: () =>
            {
                AuthManager.Instance.SendEmailVerification(
                    onSuccess: () => 
                    {
                        PlaySound(successSound);
                        PrintToRegisterConsole("> UTENTE CREATO. EMAIL DI CONFERMA INVIATA. VERIFICARE L'ACCOUNT PER ACCEDERE.");
                        AuthManager.Instance.Logout(); 
                        isProcessing = false; 
                    },
                    onError: (errorMsg) => 
                    {
                        PlaySound(errorSound);
                        PrintToRegisterConsole($"> UTENTE CREATO, MA ERRORE INVIO MAIL: {errorMsg}");
                        AuthManager.Instance.Logout();
                        isProcessing = false;
                    }
                );
            },
            onError: (errorMsg) =>
            {
                PlaySound(errorSound);
                PrintToRegisterConsole($"> ERRORE DI SISTEMA: {errorMsg}");
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
        loginEmailInput.Select(); 
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

    // --- FUNZIONI SICURE PER L'ANIMAZIONE DEL TESTO ---

    private void PrintToLoginConsole(string text)
    {
        if (loginTypingCoroutine != null) StopCoroutine(loginTypingCoroutine);
        loginTypingCoroutine = StartCoroutine(TypewriterEffect(loginConsoleOutput, text));
    }

    private void PrintToRegisterConsole(string text)
    {
        if (registerTypingCoroutine != null) StopCoroutine(registerTypingCoroutine);
        registerTypingCoroutine = StartCoroutine(TypewriterEffect(registerConsoleOutput, text));
    }

    private void PrintToSystemConsole(string text)
    {
        if (systemTypingCoroutine != null) StopCoroutine(systemTypingCoroutine);
        systemTypingCoroutine = StartCoroutine(TypewriterEffect(systemStatusText, text));
    }

    private IEnumerator TypewriterEffect(TMP_Text textBox, string fullText)
    {
        textBox.text = "";
        foreach (char c in fullText)
        {
            textBox.text += c;
            PlaySound(keyPressSound, 0.5f); 
            yield return new WaitForSeconds(0.02f); 
        }
        textBox.text += " \u2588"; 
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    // ================================================================
    // LOGICA RECUPERO PASSWORD
    // ================================================================
    public void ExecutePasswordReset()
    {
        string email = loginEmailInput.text.Trim();

        // Controlla se l'utente ha scritto qualcosa nel campo email
        if (string.IsNullOrEmpty(email))
        {
            PlaySound(errorSound);
            PrintToLoginConsole("> ERRORE: DIGITARE L'EMAIL NEL CAMPO APPOSITO PER IL RECUPERO.");
            return;
        }

        isProcessing = true;
        PrintToLoginConsole("> RICERCA NODO UTENTE IN CORSO...");

        AuthManager.Instance.ResetPassword(email,
            onSuccess: () => 
            {
                PlaySound(successSound);
                PrintToLoginConsole("> EMAIL DI RIPRISTINO INVIATA. CONTROLLARE LA CASELLA POSTALE.");
                isProcessing = false;
            },
            onError: (errorMsg) => 
            {
                PlaySound(errorSound);
                PrintToLoginConsole($"> ERRORE DI SISTEMA: {errorMsg}");
                isProcessing = false;
            }
        );
    }
}
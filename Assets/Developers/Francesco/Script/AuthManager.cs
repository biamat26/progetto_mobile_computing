using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

// ================================================================
// AuthManager.cs
// Gestisce Login e Registrazione tramite Firebase REST API
// ================================================================

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    [Header("Firebase Config")]
    [SerializeField] private string firebaseApiKey = "AIzaSyCA_N8tI7GyE1E9REC0Rv1nMsfJOr7Az0k";

    // URL Firebase REST API
    private const string REGISTER_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
    private const string LOGIN_URL    = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";
    private const string SEND_VERIFICATION_URL = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=";
    private const string GET_USER_DATA_URL     = "https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=";

    // Dati utente loggato (accessibili da altri script)
    public string UserEmail    { get; private set; }
    public string UserId       { get; private set; }
    public string UserToken    { get; private set; }
    public bool   IsLoggedIn   { get; private set; }
    public bool IsEmailVerified { get; private set; }

    // -------------------------------------------------------
    // Singleton: AuthManager persiste tra le scene
    // -------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ================================================================
    // REGISTRAZIONE
    // ================================================================
    public void Register(string email, string password,
                         Action onSuccess, Action<string> onError)
    {
        StartCoroutine(RegisterCoroutine(email, password, onSuccess, onError));
    }

    private IEnumerator RegisterCoroutine(string email, string password,
                                          Action onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            onError?.Invoke("Email e password non possono essere vuote.");
            yield break;
        }

        if (password.Length < 6)
        {
            onError?.Invoke("La password deve essere di almeno 6 caratteri.");
            yield break;
        }

        string jsonBody = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";
        byte[] bodyRaw  = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(REGISTER_URL + firebaseApiKey, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ParseAndSaveUser(request.downloadHandler.text);
                Debug.Log($"[AuthManager] Registrazione OK: {UserEmail}");
                onSuccess?.Invoke();
            }
            else
            {
                string errorMsg = ParseFirebaseError(request.downloadHandler.text);
                Debug.LogWarning($"[AuthManager] Errore registrazione: {errorMsg}");
                onError?.Invoke(errorMsg);
            }
        }
    }

    // ================================================================
    // LOGIN
    // ================================================================
    public void Login(string email, string password,
                      Action onSuccess, Action<string> onError)
    {
        StartCoroutine(LoginCoroutine(email, password, onSuccess, onError));
    }

    private IEnumerator LoginCoroutine(string email, string password,
                                       Action onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            onError?.Invoke("Inserisci email e password.");
            yield break;
        }

        string jsonBody = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";
        byte[] bodyRaw  = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(LOGIN_URL + firebaseApiKey, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ParseAndSaveUser(request.downloadHandler.text);
                Debug.Log($"[AuthManager] Login OK: {UserEmail}");
                onSuccess?.Invoke();
            }
            else
            {
                string errorMsg = ParseFirebaseError(request.downloadHandler.text);
                Debug.LogWarning($"[AuthManager] Errore login: {errorMsg}");
                onError?.Invoke(errorMsg);
            }
        }
    }

    // ================================================================
    // LOGOUT
    // ================================================================
    public void Logout()
    {
        UserEmail  = null;
        UserId     = null;
        UserToken  = null;
        IsLoggedIn = false;
        PlayerPrefs.DeleteKey("savedEmail");
        PlayerPrefs.DeleteKey("savedPassword");
        PlayerPrefs.DeleteKey("EmailGiocatore"); 
        Debug.Log("[AuthManager] Logout effettuato.");
    }

    // ================================================================
    // SALVA SESSIONE
    // ================================================================
    public void SaveSession(string email, string password)
    {
        PlayerPrefs.SetString("savedEmail",    email);
        PlayerPrefs.SetString("savedPassword", password);
        PlayerPrefs.Save();
    }

    public bool HasSavedSession()
    {
        return PlayerPrefs.HasKey("savedEmail") && PlayerPrefs.HasKey("savedPassword");
    }

    public void AutoLogin(Action onSuccess, Action<string> onError)
    {
        if (HasSavedSession())
        {
            string email    = PlayerPrefs.GetString("savedEmail");
            string password = PlayerPrefs.GetString("savedPassword");
            Login(email, password, onSuccess, onError);
        }
        else
        {
            onError?.Invoke("Nessuna sessione salvata.");
        }
    }

    // ================================================================
    // UTILITY PRIVATE
    // ================================================================
    private void ParseAndSaveUser(string json)
    {
        UserEmail  = ExtractJsonValue(json, "email");
        UserId     = ExtractJsonValue(json, "localId");
        UserToken  = ExtractJsonValue(json, "idToken");
        IsLoggedIn = true;

        // Salva l'email nella memoria del gioco per il BattleSystem
        PlayerPrefs.SetString("EmailGiocatore", UserEmail);
        PlayerPrefs.Save();
    }

    private string ExtractJsonValue(string json, string key)
    {
        string searchKey = $"\"{key}\"";
        int keyIndex = json.IndexOf(searchKey);
        if (keyIndex < 0) return "";

        int colonIndex = json.IndexOf(":", keyIndex);
        if (colonIndex < 0) return "";

        int startQuote = json.IndexOf("\"", colonIndex);
        if (startQuote < 0) return "";

        int endQuote = json.IndexOf("\"", startQuote + 1);
        if (endQuote < 0) return "";

        return json.Substring(startQuote + 1, endQuote - startQuote - 1);
    }

    private string ParseFirebaseError(string json)
    {
        if (json.Contains("EMAIL_EXISTS"))         return "Email già registrata.";
        if (json.Contains("EMAIL_NOT_FOUND"))      return "Email non trovata.";
        if (json.Contains("INVALID_PASSWORD"))     return "Password errata.";
        if (json.Contains("INVALID_EMAIL"))        return "Email non valida.";
        if (json.Contains("WEAK_PASSWORD"))        return "Password troppo debole (min. 6 caratteri).";
        if (json.Contains("TOO_MANY_ATTEMPTS"))    return "Troppi tentativi. Riprova più tardi.";
        if (json.Contains("USER_DISABLED"))        return "Account disabilitato.";
        if (json.Contains("INVALID_LOGIN_CREDENTIALS")) return "Credenziali non valide.";
        return "Errore di connessione. Controlla la rete.";
    }

    // ================================================================
    // VERIFICA EMAIL
    // ================================================================
    public void SendEmailVerification(Action onSuccess, Action<string> onError)
    {
        StartCoroutine(SendEmailVerificationCoroutine(onSuccess, onError));
    }

    private IEnumerator SendEmailVerificationCoroutine(Action onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(UserToken))
        {
            onError?.Invoke("Utente non loggato. Impossibile inviare l'email di verifica.");
            yield break;
        }

        string jsonBody = $"{{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"{UserToken}\"}}";
        byte[] bodyRaw  = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(SEND_VERIFICATION_URL + firebaseApiKey, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[AuthManager] Email di verifica inviata con successo.");
                onSuccess?.Invoke();
            }
            else
            {
                string errorMsg = ParseFirebaseError(request.downloadHandler.text);
                Debug.LogWarning($"[AuthManager] Errore invio verifica: {errorMsg}");
                onError?.Invoke(errorMsg);
            }
        }
    }

    public void CheckEmailVerificationStatus(Action<bool> onComplete, Action<string> onError)
    {
        StartCoroutine(CheckEmailVerificationStatusCoroutine(onComplete, onError));
    }

    private IEnumerator CheckEmailVerificationStatusCoroutine(Action<bool> onComplete, Action<string> onError)
    {
        if (string.IsNullOrEmpty(UserToken))
        {
            onError?.Invoke("Nessun token utente disponibile.");
            yield break;
        }

        string jsonBody = $"{{\"idToken\":\"{UserToken}\"}}";
        byte[] bodyRaw  = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(GET_USER_DATA_URL + firebaseApiKey, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                IsEmailVerified = responseText.Contains("\"emailVerified\": true") || responseText.Contains("\"emailVerified\":true");
                
                Debug.Log($"[AuthManager] Stato verifica email: {IsEmailVerified}");
                onComplete?.Invoke(IsEmailVerified);
            }
            else
            {
                string errorMsg = ParseFirebaseError(request.downloadHandler.text);
                Debug.LogWarning($"[AuthManager] Errore controllo email: {errorMsg}");
                onError?.Invoke(errorMsg);
            }
        }
    }

    // ================================================================
    // RECUPERO PASSWORD
    // ================================================================
    public void ResetPassword(string email, Action onSuccess, Action<string> onError)
    {
        StartCoroutine(ResetPasswordCoroutine(email, onSuccess, onError));
    }

    private IEnumerator ResetPasswordCoroutine(string email, Action onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(email))
        {
            onError?.Invoke("Inserisci un'email valida.");
            yield break;
        }

        string jsonBody = $"{{\"requestType\":\"PASSWORD_RESET\",\"email\":\"{email}\"}}";
        byte[] bodyRaw  = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(SEND_VERIFICATION_URL + firebaseApiKey, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[AuthManager] Email di reset password inviata.");
                onSuccess?.Invoke();
            }
            else
            {
                string errorMsg = ParseFirebaseError(request.downloadHandler.text);
                Debug.LogWarning($"[AuthManager] Errore reset password: {errorMsg}");
                onError?.Invoke(errorMsg);
            }
        }
    }

    // ================================================================
    // LOGIN COME OSPITE
    // ================================================================
    public void LoginAsGuest()
    {
        PlayerPrefs.SetString("EmailGiocatore", "Ospite");
        PlayerPrefs.Save();
        Debug.Log("[AuthManager] Login effettuato come Ospite.");
    }
}
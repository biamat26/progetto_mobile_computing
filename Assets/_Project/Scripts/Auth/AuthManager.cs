using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

// ================================================================
// AuthManager.cs
// Gestisce Login e Registrazione tramite Firebase REST API
//
// SETUP:
// 1. Vai su https://console.firebase.google.com
// 2. Crea un progetto → Authentication → Email/Password → Abilita
// 3. Impostazioni progetto → Copia la tua WEB_API_KEY
// 4. Incollala nella variabile firebaseApiKey qui sotto
// ================================================================

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    [Header("Firebase Config")]
    [SerializeField] private string firebaseApiKey = "AIzaSyCA_N8tI7GyE1E9REC0Rv1nMsfJOr7Az0k";

    // URL Firebase REST API
    private const string REGISTER_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
    private const string LOGIN_URL    = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";

    // Dati utente loggato (accessibili da altri script)
    public string UserEmail    { get; private set; }
    public string UserId       { get; private set; }
    public string UserToken    { get; private set; }
    public bool   IsLoggedIn   { get; private set; }

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
    // Chiamata da LoginUI: AuthManager.Instance.Register(email, pass, OnSuccess, OnError)
    // ================================================================
    public void Register(string email, string password,
                         Action onSuccess, Action<string> onError)
    {
        StartCoroutine(RegisterCoroutine(email, password, onSuccess, onError));
    }

    private IEnumerator RegisterCoroutine(string email, string password,
                                          Action onSuccess, Action<string> onError)
    {
        // Validazione base lato client
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

        // Costruisce il JSON per Firebase
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
                // Registrazione OK → salva i dati
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
    // Chiamata da LoginUI: AuthManager.Instance.Login(email, pass, OnSuccess, OnError)
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
        Debug.Log("[AuthManager] Logout effettuato.");
    }

    // ================================================================
    // SALVA SESSIONE — Ricordami (opzionale)
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

    // Parsing manuale del JSON di risposta Firebase (senza librerie esterne)
    private void ParseAndSaveUser(string json)
    {
        UserEmail  = ExtractJsonValue(json, "email");
        UserId     = ExtractJsonValue(json, "localId");
        UserToken  = ExtractJsonValue(json, "idToken");
        IsLoggedIn = true;
    }

    // Estrae un valore da una stringa JSON senza JsonUtility
    private string ExtractJsonValue(string json, string key)
    {
        string search = $"\"{key}\":\"";
        int start = json.IndexOf(search);
        if (start < 0) return "";
        start += search.Length;
        int end = json.IndexOf("\"", start);
        return end < 0 ? "" : json.Substring(start, end - start);
    }

    // Traduce gli errori Firebase in messaggi leggibili
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
}
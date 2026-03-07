using UnityEngine;

// ================================================================
// UserSession.cs
// Posizione: Assets/Scripts/Auth/
// Contiene i dati dell'utente loggato, accessibili da tutta l'app
// ================================================================

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }

    // Dati utente
    public string Email       { get; set; }
    public string UserId      { get; set; }
    public string Token       { get; set; }
    public string Username    { get; set; }
    public int    SelectedCharacterIndex { get; set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Popola la sessione dopo il login
    public void SetSession(string email, string userId, string token)
    {
        Email    = email;
        UserId   = userId;
        Token    = token;
        Username = email.Split('@')[0]; // Nome utente = parte prima della @
        Debug.Log($"[UserSession] Sessione avviata per: {Username}");
    }

    public void ClearSession()
    {
        Email    = null;
        UserId   = null;
        Token    = null;
        Username = null;
        SelectedCharacterIndex = 0;
    }

    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);
}
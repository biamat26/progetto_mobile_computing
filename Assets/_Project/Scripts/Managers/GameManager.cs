using UnityEngine;
using UnityEngine.SceneManagement;

// ================================================================
// GameManager.cs
// Posizione: Assets/Scripts/Managers/
// Gestisce lo stato globale del gioco e il cambio scena
// ================================================================

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Nomi delle scene (devono corrispondere ai file .unity)
    public const string SCENE_LOGIN      = "LoginScene";
    public const string SCENE_MAINMENU   = "MainMenuScene";
    public const string SCENE_CHARSELECT = "CharacterSelectScene";
    public const string SCENE_GAME       = "GameScene";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Viene chiamato automaticamente ad ogni cambio scena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scena caricata: {scene.name}");
        // Avvia la musica giusta per ogni scena
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(scene.name);
    }

    // ================================================================
    // NAVIGAZIONE TRA SCENE
    // ================================================================
    public void GoToLogin()         => SceneManager.LoadScene(SCENE_LOGIN);
    public void GoToMainMenu()      => SceneManager.LoadScene(SCENE_MAINMENU);
    public void GoToCharacterSelect()=> SceneManager.LoadScene(SCENE_CHARSELECT);
    public void GoToGame()          => SceneManager.LoadScene(SCENE_GAME);

    public void QuitGame()
    {
        Debug.Log("[GameManager] Uscita dal gioco.");
        Application.Quit();
    }
}
using UnityEngine;

// ================================================================
// AudioManager.cs
// Posizione: Assets/Scripts/Managers/
//
// SETUP IN UNITY:
// 1. Crea un GameObject vuoto → chiamalo "AudioManager"
// 2. Attacca questo script
// 3. Aggiungi 2 componenti AudioSource allo stesso GameObject
// 4. Trascina i clip audio negli slot dell'Inspector
// ================================================================

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;   // Per la musica di sottofondo
    [SerializeField] private AudioSource sfxSource;     // Per gli effetti sonori

    [Header("Musiche")]
    [SerializeField] private AudioClip musicLogin;
    [SerializeField] private AudioClip musicMainMenu;
    [SerializeField] private AudioClip musicCharSelect;

    [Header("Effetti Sonori")]
    [SerializeField] private AudioClip sfxClick;
    [SerializeField] private AudioClip sfxHover;
    [SerializeField] private AudioClip sfxError;
    [SerializeField] private AudioClip sfxSuccess;

    // Valori di volume salvati
    private float musicVolume = 1f;
    private float sfxVolume   = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Carica volumi salvati da PlayerPrefs
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume   = PlayerPrefs.GetFloat("SFXVolume",   1f);
        ApplyVolumes();
    }

    // ================================================================
    // MUSICA
    // ================================================================
    public void PlayMusic(string sceneName)
    {
        AudioClip clip = sceneName switch
        {
            "LoginScene"           => musicLogin,
            "MainMenuScene"        => musicMainMenu,
            "CharacterSelectScene" => musicCharSelect,
            _                      => null
        };

        if (clip == null || musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    // ================================================================
    // EFFETTI SONORI
    // ================================================================
    public void PlayClick()   => sfxSource.PlayOneShot(sfxClick,   sfxVolume);
    public void PlayHover()   => sfxSource.PlayOneShot(sfxHover,   sfxVolume);
    public void PlayError()   => sfxSource.PlayOneShot(sfxError,   sfxVolume);
    public void PlaySuccess() => sfxSource.PlayOneShot(sfxSuccess, sfxVolume);

    // ================================================================
    // VOLUME — chiamati da SettingsUI
    // ================================================================
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void ToggleMute(bool muted)
    {
        musicSource.mute = muted;
        sfxSource.mute   = muted;
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume()   => sfxVolume;

    private void ApplyVolumes()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource   != null) sfxSource.volume   = sfxVolume;
    }
}
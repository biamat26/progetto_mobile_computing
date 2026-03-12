using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ================================================================
// SettingsUI.cs
// Posizione: Assets/Scripts/UI/
// ================================================================

public class SettingsUI : MonoBehaviour
{
    [Header("Volume Musica")]
    [SerializeField] private Slider   sliderMusica;
    [SerializeField] private TMP_Text txtVolumeMusica;

    [Header("Volume SFX")]
    [SerializeField] private Slider   sliderSFX;
    [SerializeField] private TMP_Text txtVolumeSFX;

    [Header("Muto")]
    [SerializeField] private Toggle toggleMuto;

    [Header("Pulsanti")]
    [SerializeField] private Button btnChiudi;

    private void Start()
    {
        // Nasconde il pannello impostazioni all'avvio
        gameObject.SetActive(false);

        // Carica valori salvati
        if (AudioManager.Instance != null)
        {
            if (sliderMusica != null) sliderMusica.value = AudioManager.Instance.GetMusicVolume();
            if (sliderSFX    != null) sliderSFX.value    = AudioManager.Instance.GetSFXVolume();
        }

        UpdateLabels();

        if (sliderMusica != null) sliderMusica.onValueChanged.AddListener(OnMusicaChanged);
        if (sliderSFX    != null) sliderSFX.onValueChanged.AddListener(OnSFXChanged);
        if (toggleMuto   != null) toggleMuto.onValueChanged.AddListener(OnMutoChanged);
        if (btnChiudi    != null) btnChiudi.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        // Aggiorna i valori ogni volta che il pannello viene aperto
        if (AudioManager.Instance != null)
        {
            if (sliderMusica != null) sliderMusica.value = AudioManager.Instance.GetMusicVolume();
            if (sliderSFX    != null) sliderSFX.value    = AudioManager.Instance.GetSFXVolume();
        }
        UpdateLabels();
    }

    private void OnMusicaChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        UpdateLabels();
    }

    private void OnSFXChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        UpdateLabels();
    }

    private void OnMutoChanged(bool muted)
    {
        AudioManager.Instance?.ToggleMute(muted);
        if (sliderMusica != null) sliderMusica.interactable = !muted;
        if (sliderSFX    != null) sliderSFX.interactable    = !muted;
    }

    private void UpdateLabels()
    {
        if (txtVolumeMusica != null && sliderMusica != null)
            txtVolumeMusica.text = $"{Mathf.RoundToInt(sliderMusica.value * 100)}%";
        if (txtVolumeSFX != null && sliderSFX != null)
            txtVolumeSFX.text = $"{Mathf.RoundToInt(sliderSFX.value * 100)}%";
    }
}
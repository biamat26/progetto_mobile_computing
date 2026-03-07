using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ================================================================
// SettingsUI.cs
// Posizione: Assets/Scripts/UI/
//
// SETUP IN UNITY:
// 1. Crea un pannello "PanelSettings" dentro il Canvas del MainMenu
// 2. Attacca questo script al pannello
// 3. Trascina i riferimenti negli slot dell'Inspector
// ================================================================

public class SettingsUI : MonoBehaviour
{
    [Header("Volume Musica")]
    [SerializeField] private Slider   sliderMusica;
    [SerializeField] private TMP_Text txtVolumeMusica; // Es: "75%"

    [Header("Volume SFX")]
    [SerializeField] private Slider   sliderSFX;
    [SerializeField] private TMP_Text txtVolumeSFX;

    [Header("Muto")]
    [SerializeField] private Toggle toggleMuto;

    [Header("Pulsanti")]
    [SerializeField] private Button btnChiudi;

    private void Start()
    {
        // Carica valori salvati
        if (AudioManager.Instance != null)
        {
            sliderMusica.value = AudioManager.Instance.GetMusicVolume();
            sliderSFX.value    = AudioManager.Instance.GetSFXVolume();
        }

        // Aggiorna label
        UpdateLabels();

        // Collega eventi
        sliderMusica.onValueChanged.AddListener(OnMusicaChanged);
        sliderSFX.onValueChanged.AddListener(OnSFXChanged);
        toggleMuto.onValueChanged.AddListener(OnMutoChanged);
        btnChiudi.onClick.AddListener(() => gameObject.SetActive(false));
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
        sliderMusica.interactable = !muted;
        sliderSFX.interactable    = !muted;
    }

    private void UpdateLabels()
    {
        if (txtVolumeMusica != null)
            txtVolumeMusica.text = $"{Mathf.RoundToInt(sliderMusica.value * 100)}%";
        if (txtVolumeSFX != null)
            txtVolumeSFX.text = $"{Mathf.RoundToInt(sliderSFX.value * 100)}%";
    }
}
using UnityEngine;
using UnityEngine.UI;

public class GestoreOpzioniInGioco : MonoBehaviour
{
    [Header("Riferimenti Slider")]
    public Slider sliderMusica;
    public Slider sliderEffetti;

    [Header("Impostazioni")]
    [Tooltip("Lascia la spunta se nel Main Menu i tuoi slider vanno da 0 a 100")]
    public bool usaScalaCento = true; 

    void OnEnable()
    {
        if (AudioManager.instance != null)
        {
            if (usaScalaCento)
            {
                // Impostiamo gli slider da 0 a 100 come nel Main Menu
                sliderMusica.minValue = 0f;
                sliderMusica.maxValue = 100f;
                sliderEffetti.minValue = 0f;
                sliderEffetti.maxValue = 100f;
                
                // Moltiplichiamo il volume di Unity (che è 0.5) per 100 (così diventa 50)
                sliderMusica.value = AudioManager.instance.GetMusicVolume() * 100f;
                sliderEffetti.value = AudioManager.instance.GetSFXVolume() * 100f;
            }
            else
            {
                sliderMusica.minValue = 0f;
                sliderMusica.maxValue = 1f;
                sliderEffetti.minValue = 0f;
                sliderEffetti.maxValue = 1f;
                
                sliderMusica.value = AudioManager.instance.GetMusicVolume();
                sliderEffetti.value = AudioManager.instance.GetSFXVolume();
            }

            // Colleghiamo le funzioni
            sliderMusica.onValueChanged.RemoveAllListeners();
            sliderMusica.onValueChanged.AddListener(CambiaVolumeMusica);

            sliderEffetti.onValueChanged.RemoveAllListeners();
            sliderEffetti.onValueChanged.AddListener(CambiaVolumeEffetti);
        }
    }

    private void CambiaVolumeMusica(float valore)
    {
        if (AudioManager.instance != null)
        {
            // Se usiamo la scala 100, dividiamo per 100 prima di mandarlo all'AudioSource
            float volumeFinale = usaScalaCento ? (valore / 100f) : valore;
            AudioManager.instance.SetMusicVolume(volumeFinale);
        }
    }

    private void CambiaVolumeEffetti(float valore)
    {
        if (AudioManager.instance != null)
        {
            float volumeFinale = usaScalaCento ? (valore / 100f) : valore;
            AudioManager.instance.SetSFXVolume(volumeFinale);
        }
    }
}
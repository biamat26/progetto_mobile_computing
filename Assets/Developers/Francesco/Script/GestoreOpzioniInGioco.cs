using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // <-- Fondamentale per usare l'AudioMixer

public class GestoreOpzioniInGioco : MonoBehaviour
{
    [Header("Riferimenti")]
    [Tooltip("Trascina qui il tuo AudioMixer principale")]
    public AudioMixer mainMixer; 
    public Slider sliderMusica;
    public Slider sliderEffetti;

    void OnEnable()
    {
        // 1. Andiamo a leggere ESATTAMENTE le chiavi del tuo Main Menu
        float volumeMusicaSalvato = PlayerPrefs.GetFloat("VolMusica", 30f);
        float volumeEffettiSalvato = PlayerPrefs.GetFloat("VolEffetti", 30f);

        // 2. Impostiamo la scala da 0 a 100 come nel tuo menu
        sliderMusica.minValue = 0f;
        sliderMusica.maxValue = 100f;
        sliderEffetti.minValue = 0f;
        sliderEffetti.maxValue = 100f;

        // 3. Posizioniamo la levetta nel punto esatto del salvataggio
        sliderMusica.value = volumeMusicaSalvato;
        sliderEffetti.value = volumeEffettiSalvato;

        // 4. Colleghiamo gli eventi
        sliderMusica.onValueChanged.RemoveAllListeners();
        sliderMusica.onValueChanged.AddListener(CambiaVolumeMusica);

        sliderEffetti.onValueChanged.RemoveAllListeners();
        sliderEffetti.onValueChanged.AddListener(CambiaVolumeEffetti);
    }

    private void CambiaVolumeMusica(float valoreSlider)
    {
        // Salva in memoria per quando tornerai al Main Menu
        PlayerPrefs.SetFloat("VolMusica", valoreSlider);
        PlayerPrefs.Save();

        // Applica al Mixer la stessa identica matematica del Main Menu
        if (mainMixer != null)
        {
            float valoreNormalizzato = Mathf.Clamp(valoreSlider / 100f, 0.0001f, 1f);
            float decibel = Mathf.Log10(valoreNormalizzato) * 20f;
            mainMixer.SetFloat("MusicaVol", decibel);
        }
    }

    private void CambiaVolumeEffetti(float valoreSlider)
    {
        PlayerPrefs.SetFloat("VolEffetti", valoreSlider);
        PlayerPrefs.Save();

        if (mainMixer != null)
        {
            float valoreNormalizzato = Mathf.Clamp(valoreSlider / 100f, 0.0001f, 1f);
            float decibel = Mathf.Log10(valoreNormalizzato) * 20f;
            mainMixer.SetFloat("EffettiVol", decibel);
        }
    }
}
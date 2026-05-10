using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ImpostazioniAudio : MonoBehaviour
{
    [Header("Riferimenti")]
    public AudioMixer mainMixer;
    public Slider sliderMusica;
    public Slider sliderEffetti;

    private void Start()
    {
        float volumeMusicaSalvato = PlayerPrefs.GetFloat("VolMusica", 30f);
        float volumeEffettiSalvato = PlayerPrefs.GetFloat("VolEffetti", 30f);

        // 1. Diciamo allo script di prendere il controllo dello slider Musica
        if (sliderMusica != null) 
        {
            sliderMusica.value = volumeMusicaSalvato;
            // LA SUPER-COLLA: colleghiamo la funzione via codice, così non si stacca più!
            sliderMusica.onValueChanged.AddListener(ImpostaVolumeMusica);
        }
        
        // 2. Diciamo allo script di prendere il controllo dello slider Effetti
        if (sliderEffetti != null) 
        {
            sliderEffetti.value = volumeEffettiSalvato;
            sliderEffetti.onValueChanged.AddListener(ImpostaVolumeEffetti);
        }

        ApplicaVolumeMusicaAlMixer(volumeMusicaSalvato);
        ApplicaVolumeEffettiAlMixer(volumeEffettiSalvato);
    }

    public void ImpostaVolumeMusica(float valoreSlider)
    {
        float valoreNormalizzato = Mathf.Clamp(valoreSlider / 100f, 0.0001f, 1f);
        float decibel = Mathf.Log10(valoreNormalizzato) * 20f;
        mainMixer.SetFloat("MusicaVol", decibel);
        
        Debug.Log("SUCCESSO: Slider agganciato! Musica a " + decibel + " dB");
        
        PlayerPrefs.SetFloat("VolMusica", valoreSlider);
        PlayerPrefs.Save();
    }

    public void ImpostaVolumeEffetti(float valoreSlider)
    {
        float valoreNormalizzato = Mathf.Clamp(valoreSlider / 100f, 0.0001f, 1f);
        float decibel = Mathf.Log10(valoreNormalizzato) * 20f;
        mainMixer.SetFloat("EffettiVol", decibel);
        
        PlayerPrefs.SetFloat("VolEffetti", valoreSlider);
        PlayerPrefs.Save();
    }

    private void ApplicaVolumeMusicaAlMixer(float valore)
    {
        float valoreNormalizzato = Mathf.Clamp(valore / 100f, 0.0001f, 1f);
        mainMixer.SetFloat("MusicaVol", Mathf.Log10(valoreNormalizzato) * 20f);
    }

    private void ApplicaVolumeEffettiAlMixer(float valore)
    {
        float valoreNormalizzato = Mathf.Clamp(valore / 100f, 0.0001f, 1f);
        mainMixer.SetFloat("EffettiVol", Mathf.Log10(valoreNormalizzato) * 20f);
    }
}
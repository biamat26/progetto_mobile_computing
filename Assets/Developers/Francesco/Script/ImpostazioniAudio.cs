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
        // 1. All'avvio, controlliamo la "Memory Card". Se non c'è un salvataggio, mettiamo 100 di default.
        float volumeMusicaSalvato = PlayerPrefs.GetFloat("VolMusica", 100f);
        float volumeEffettiSalvato = PlayerPrefs.GetFloat("VolEffetti", 100f);

        // 2. Spostiamo fisicamente le levette degli slider su questi numeri
        if (sliderMusica != null) sliderMusica.value = volumeMusicaSalvato;
        if (sliderEffetti != null) sliderEffetti.value = volumeEffettiSalvato;

        // 3. Applichiamo i volumi al Mixer per essere sicuri che si sentano giusti da subito
        ApplicaVolumeMusicaAlMixer(volumeMusicaSalvato);
        ApplicaVolumeEffettiAlMixer(volumeEffettiSalvato);
    }

    // Questa viene chiamata quando muovi lo Slider della Musica
    public void ImpostaVolumeMusica(float valoreSlider)
    {
        ApplicaVolumeMusicaAlMixer(valoreSlider);
        
        // SALVA IL DATO: così se chiudi il gioco se lo ricorderà!
        PlayerPrefs.SetFloat("VolMusica", valoreSlider);
        PlayerPrefs.Save();
    }

    // Questa viene chiamata quando muovi lo Slider degli Effetti
    public void ImpostaVolumeEffetti(float valoreSlider)
    {
        ApplicaVolumeEffettiAlMixer(valoreSlider);
        
        // SALVA IL DATO
        PlayerPrefs.SetFloat("VolEffetti", valoreSlider);
        PlayerPrefs.Save();
    }

    // --- FUNZIONI INTERNE PER LA MATEMATICA DEL MIXER ---

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
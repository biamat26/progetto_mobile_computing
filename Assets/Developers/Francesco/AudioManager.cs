using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource; // SFX sta per Sound Effects

    [Header("AudioClip")]
    public AudioClip background;
    public AudioClip click;

    private void Start() 
    {
        // 1. Assegna e fai partire solo la musica di sottofondo
        musicSource.clip = background;
       // 1. Diciamo all'AudioSource da che secondo iniziare (es. dal secondo 10)
        musicSource.time = 40f; 
        
        // 2. Facciamo partire l'audio
        musicSource.Play();
        
        // 3. Usiamo "Invoke" per chiamare il metodo che lo ferma dopo un tot di secondi.
        // In questo caso lo fermiamo dopo 5 secondi (quindi suonerà dal secondo 10 al 15)
        Invoke("StopAudio", 180f); 
    }

    // Metodo per fermare l'audio
    private void StopAudio() 
    {
        musicSource.Stop();
    }

    // 2. Crea un metodo PUBLIC per il pulsante
    public void PlayClickSound(AudioClip clip) 
    {
        // PlayOneShot è perfetto per i suoni brevi come i click
        SFXSource.PlayOneShot(clip);
    }
}
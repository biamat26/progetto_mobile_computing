using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("AudioSource")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Rimuoviamo il metodo Start()!

    // Creiamo un metodo pubblico per gestire la musica quando vogliamo noi
    public void PlayMusic(AudioClip clip, float startTime = 0f, float duration = 0f)
    {
        // Se c'è già una canzone in riproduzione, o c'era un vecchio Invoke, puliamo tutto
        CancelInvoke("StopAudio"); 
        
        musicSource.clip = clip;
        musicSource.time = startTime; // Parte dal secondo che gli dici tu
        musicSource.Play();

        // Se gli passiamo una durata maggiore di 0, impostiamo l'arresto
        if (duration > 0f)
        {
            Invoke("StopAudio", duration);
        }
    }

    private void StopAudio() 
    {
        musicSource.Stop();
    }

    public void PlayClickSound(AudioClip clip) 
    {
        SFXSource.PlayOneShot(clip);
    }
}
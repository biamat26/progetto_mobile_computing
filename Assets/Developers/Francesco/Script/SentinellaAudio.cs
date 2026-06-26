using UnityEngine;
using UnityEngine.Audio;

public class SentinellaAudio : MonoBehaviour
{
    [Header("Clip Audio")]
    public AudioClip suonoMovimento;
    public AudioClip suonoCattura;

    [Header("Audio Mixer")]
    [Tooltip("Trascina qui il gruppo 'SFX' del tuo Audio Mixer")]
    public AudioMixerGroup gruppoSFX;

    [Header("Impostazioni Range")]
    public float distanzaMassima = 10f; 
    
    private Transform player;
    private AudioSource audioMovimento;
    private AudioSource audioCattura;
    private bool giocatoreCatturato = false;

    void Start()
    {
        // Setup Movimento
        audioMovimento = gameObject.AddComponent<AudioSource>();
        audioMovimento.clip = suonoMovimento;
        audioMovimento.loop = true;
        audioMovimento.playOnAwake = true;
        audioMovimento.volume = 0f;
        
        if (gruppoSFX != null) audioMovimento.outputAudioMixerGroup = gruppoSFX;
        
        audioMovimento.Play();

        // Setup Cattura
        audioCattura = gameObject.AddComponent<AudioSource>();
        audioCattura.clip = suonoCattura;
        audioCattura.loop = false;
        audioCattura.playOnAwake = false;
        
        if (gruppoSFX != null) audioCattura.outputAudioMixerGroup = gruppoSFX;
        
        audioCattura.volume = 0.2f; 
        audioCattura.ignoreListenerPause = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // Abbiamo rimosso il controllo inaffidabile sul Time.timeScale!
        // Ora qui gestiamo solo il rumore di fondo del movimento.

        if (player != null && !giocatoreCatturato && Time.timeScale > 0f)
        {
            float distanza = Vector2.Distance(transform.position, player.position);
            float volume = 1f - Mathf.Clamp01(distanza / distanzaMassima);
            audioMovimento.volume = volume * 0.5f; 
        }
    }

    // Il cono di visione chiamerà questa funzione nel momento ESATTO della cattura
    public void AttivaSuonoCattura()
    {
        if (!giocatoreCatturato)
        {
            giocatoreCatturato = true; 
            
            if (audioMovimento != null) 
                audioMovimento.Stop(); 

            if (audioCattura != null && suonoCattura != null)
            {
                audioCattura.Play(); 
            }
        }
    }
}
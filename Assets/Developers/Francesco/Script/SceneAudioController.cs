using UnityEngine;
using System.Collections; 

public class SceneAudioController : MonoBehaviour
{
    [Header("Impostazioni Audio Scena")]
    [Tooltip("Trascina qui la canzone che deve suonare in questa scena")]
    public AudioClip musicaScena;

    [Tooltip("Da quale secondo deve iniziare?")]
    public float secondoDiPartenza = 0f; 

    // Abbiamo rimosso la variabile "volumeMassimo" perché ora 
    // lo script usa automaticamente il volume dello slider!

    void Start()
    {
        if (musicaScena == null)
        {
            Debug.LogError("ATTENZIONE: Manca la canzone sull'oggetto " + gameObject.name);
            return;
        }

        if (AudioManager.instance != null)
        {
            StartCoroutine(GestisciLoopAudio());
        }
    }

    private IEnumerator GestisciLoopAudio()
    {
        float tempoFade = 2f; 

        // 1. Facciamo partire la canzone tramite il TUO AudioManager
        AudioManager.instance.PlayMusic(musicaScena, secondoDiPartenza, 0f);

        // 2. Cerchiamo l'AudioSource della musica all'interno del tuo AudioManager
        AudioSource sorgenteMusica = null;
        AudioSource[] sorgentiAudio = AudioManager.instance.GetComponentsInChildren<AudioSource>();
        
        foreach (AudioSource s in sorgentiAudio)
        {
            if (s.clip == musicaScena)
            {
                sorgenteMusica = s;
                break;
            }
        }

        // Se per qualche motivo non lo trova, fermiamo lo script per evitare errori
        if (sorgenteMusica == null) yield break;

        while (true)
        {
            // Se siamo al secondo ciclo e la canzone è finita, la facciamo ripartire
            if (!sorgenteMusica.isPlaying)
            {
                AudioManager.instance.PlayMusic(musicaScena, secondoDiPartenza, 0f);
            }

            // 3. Leggiamo il volume in cui è impostato lo SLIDER in questo momento
            float volumeTarget = sorgenteMusica.volume;
            
            // Azzeriamo il volume istantaneamente per iniziare a salire
            sorgenteMusica.volume = 0f;

            // FADE-IN (Da 0 al volume dello slider)
            float t = 0f;
            while (t < tempoFade)
            {
                t += Time.unscaledDeltaTime; 
                sorgenteMusica.volume = Mathf.Lerp(0f, volumeTarget, t / tempoFade);
                yield return null; 
            }
            sorgenteMusica.volume = volumeTarget; 

            // ATTESA INTELLIGENTE
            while (sorgenteMusica.isPlaying && sorgenteMusica.time < musicaScena.length - tempoFade)
            {
                // Continuiamo a registrare il volume. Così se il giocatore muove 
                // lo slider mentre gioca, lo script usa il nuovo volume per l'uscita!
                volumeTarget = sorgenteMusica.volume;
                yield return null;
            }

            // FADE-OUT (Dal volume dello slider a 0)
            t = 0f;
            while (t < tempoFade)
            {
                t += Time.unscaledDeltaTime;
                sorgenteMusica.volume = Mathf.Lerp(volumeTarget, 0f, t / tempoFade);
                yield return null;
            }
            
            sorgenteMusica.volume = 0f;
            sorgenteMusica.Stop();

            // Trucchetto: Rimettiamo segretamente il volume al valore originale. 
            // In questo modo, durante la pausa, se il giocatore apre il menu,
            // lo slider avrà il valore corretto e non sarà bloccato a 0!
            sorgenteMusica.volume = volumeTarget;

            // Pausa di 1 secondo
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    public void RiprendiMusica()
    {
        // Qui devi inserire la logica che faceva ripartire la musica (quella che avevi nel metodo Start)
        // Esempio:
        if (musicaScena != null)
        {
            AudioSource source = GetComponent<AudioSource>();
            if (source != null) 
            {
                source.clip = musicaScena;
                source.Play();
            }
        }
    }
}
using UnityEngine;
using System.Collections; // Aggiunto: necessario per usare le Coroutine

public class SceneAudioController : MonoBehaviour
{
    [Header("Impostazioni Audio Scena")]
    [Tooltip("Trascina qui la canzone che deve suonare in questa scena")]
    public AudioClip musicaScena;

    [Tooltip("Da quale secondo deve iniziare?")]
    public float secondoDiPartenza = 0f; 

    void Start()
    {
        if (musicaScena == null)
        {
            Debug.LogError("ATTENZIONE: Manca la canzone sull'oggetto " + gameObject.name);
            return;
        }

        if (AudioManager.instance != null)
        {
            // Invece di chiamare direttamente l'audio, avviamo la Coroutine
            StartCoroutine(GestisciLoopAudio());
        }
    }

    // La Coroutine che si occupa di riprodurre l'audio, aspettare e rimetterlo in play
    private IEnumerator GestisciLoopAudio()
    {
        // Un ciclo infinito che continuerà a girare finché questo oggetto/scena è attivo
        while (true)
        {
            // 1. Facciamo partire la musica
            AudioManager.instance.PlayMusic(musicaScena, secondoDiPartenza, 0f);

            // 2. Calcoliamo quanto tempo la canzone suonerà effettivamente
            // (Lunghezza totale della canzone meno il secondo da cui parte)
            float tempoDiRiproduzione = musicaScena.length - secondoDiPartenza;

            // Sicurezza: se per sbaglio metti un secondo di partenza maggiore della canzone,
            // evitiamo che il tempo sia negativo (causerebbe un errore)
            if (tempoDiRiproduzione < 0f) 
            {
                tempoDiRiproduzione = 0f;
            }

            // 3. Diciamo a Unity di aspettare che la canzone finisca + 2 secondi di pausa
            yield return new WaitForSeconds(tempoDiRiproduzione + 2f);
            
            // Finito il tempo di attesa, il ciclo "while" ricomincia dall'inizio!
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
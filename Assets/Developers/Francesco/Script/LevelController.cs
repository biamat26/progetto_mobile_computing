using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Tooltip("Trascina qui la canzone del Livello!")]
    public AudioClip musicaLivello;

    void Start()
    {
        // Controllo di sicurezza
        if (musicaLivello == null)
        {
            Debug.LogError("ATTENZIONE: Non hai assegnato la musica del Livello 1!");
            return;
        }

        // Diciamo all'AudioManager globale di cambiare canzone!
        // Parte da 0 secondi, e suona all'infinito (0f)
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(musicaLivello, 0f, 0f);
        }
    }
}
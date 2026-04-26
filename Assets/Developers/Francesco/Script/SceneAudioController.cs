using UnityEngine;

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
            // Invece di scrivere 0f o 40f, passiamo la variabile!
            AudioManager.instance.PlayMusic(musicaScena, secondoDiPartenza, 0f);
        }
    }
}
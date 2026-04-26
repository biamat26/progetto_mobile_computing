using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [Tooltip("Inserisci qui il suono che deve fare questo bottone")]
    public AudioClip clickSound;

    // Questa è la funzione che il bottone chiamerà quando viene premuto
    public void RiproduciSuonoClick()
    {
        // Controlliamo che l'AudioManager esista (evita errori se provi la scena da sola)
        if (AudioManager.instance != null && clickSound != null)
        {
            // Chiamiamo il nostro manager globale!
            AudioManager.instance.PlayClickSound(clickSound);
        }
    }
}
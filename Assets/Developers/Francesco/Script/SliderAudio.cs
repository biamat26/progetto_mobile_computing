using UnityEngine;
using UnityEngine.UI; // FONDAMENTALE: Ti permette di usare gli elementi UI come lo Slider

public class SliderAudio : MonoBehaviour
{
    // Qui collegheremo il nostro Slider dall'Inspector
    public Slider sliderVolume;

    void Start()
    {
        // Questo dice allo script: "Ehi, se il valore dello slider cambia, chiama la funzione CambiaVolume"
        sliderVolume.onValueChanged.AddListener(delegate { CambiaVolume(sliderVolume.value); });
    }

    // Questa è la funzione che viene attivata ogni volta che muovi la levetta
    public void CambiaVolume(float nuovoValore)
    {
        Debug.Log("Il valore dello slider ora è: " + nuovoValore);
        
        // Qui scriverai il codice per abbassare/alzare l'audio vero e proprio, ad esempio:
        // AudioListener.volume = nuovoValore;
    }
}

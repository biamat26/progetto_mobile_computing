using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffettoVibrazione : MonoBehaviour
{
    [Header("1. Impostazioni Vibrazione")]
    public float durataVibrazione = 0.3f;   
    public float intensita = 0.5f;          
    public float velocitaScatto = 0.05f;    

    [Header("2. Impostazioni Lampeggio")]
    public int numeroLampeggi = 3;          
    public float velocitaLampeggio = 0.1f;  

    public void IniziaVibrazione()
    {
        StopAllCoroutines(); 
        StartCoroutine(EseguiDannoPokemon());
    }

    private IEnumerator EseguiDannoPokemon()
    {
        // ==========================================
        // FASE 1: VIBRAZIONE
        // ==========================================
        Vector3 posizioneOriginale = transform.localPosition;
        float tempoTrascorso = 0f;

        while (tempoTrascorso < durataVibrazione)
        {
            float spostamentoX = posizioneOriginale.x + Random.Range(-1f, 1f) * intensita;
            transform.localPosition = new Vector3(spostamentoX, posizioneOriginale.y, posizioneOriginale.z);
            
            yield return new WaitForSeconds(velocitaScatto);
            tempoTrascorso += velocitaScatto;
        }

        transform.localPosition = posizioneOriginale;

        // ==========================================
        // FASE 2: LAMPEGGIO (Ora trova anche i figli!)
        // ==========================================
        
        // Cerca TUTTE le immagini e gli sprite su questo oggetto e sui suoi figli
        Image[] immaginiUI = GetComponentsInChildren<Image>();
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < numeroLampeggi; i++)
        {
            // SPEGNI TUTTO
            foreach (var img in immaginiUI) { img.enabled = false; }
            foreach (var spr in sprites) { spr.enabled = false; }
            
            yield return new WaitForSeconds(velocitaLampeggio);

            // ACCENDI TUTTO
            foreach (var img in immaginiUI) { img.enabled = true; }
            foreach (var spr in sprites) { spr.enabled = true; }

            yield return new WaitForSeconds(velocitaLampeggio);
        }

        // Sicurezza finale: accendiamo tutto per evitare che rimanga invisibile
        foreach (var img in immaginiUI) { img.enabled = true; }
        foreach (var spr in sprites) { spr.enabled = true; }
    }
}
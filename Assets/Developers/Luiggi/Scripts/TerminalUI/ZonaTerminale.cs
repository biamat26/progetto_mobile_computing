using UnityEngine;

public class ZonaTerminale : MonoBehaviour
{
    [Header("Configurazione")]
    public string idMessaggio;
    public bool mostraSoloUnaVolta = true;

    private bool giàMostrato = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (mostraSoloUnaVolta && giàMostrato) return;

        giàMostrato = true;
        TerminalManager.Istanza.MostraAiuto(idMessaggio);
    }
}
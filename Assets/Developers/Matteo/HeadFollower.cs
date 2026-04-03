using UnityEngine;

public class HeadFollower : MonoBehaviour
{
    [Tooltip("Il GameObject della 'Testa' che ha l'alone neon e il movimento a 90 gradi")]
    public Transform headTransform;

    void LateUpdate()
    {
        // Questo script sposta semplicemente l'oggetto della scia nella stessa posizione della testa.
        // Poiché la TrailRenderer disegna la scia basandosi sul movimento del suo GameObject,
        // la scia seguirà perfettamente la testa e si curverà solo quando la testa curva.
        if (headTransform != null)
        {
            transform.position = headTransform.position;
        }
    }
}
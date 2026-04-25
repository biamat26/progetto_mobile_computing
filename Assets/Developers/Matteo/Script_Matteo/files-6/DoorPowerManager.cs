using UnityEngine;

/// <summary>
/// Mettilo sulla porta senza corrente.
/// Si apre quando WirePuzzleManager chiama PowerRestored().
/// </summary>
public class DoorPowerManager : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite spriteSpenta;   // porta senza corrente (scura)
    [SerializeField] private Sprite spriteAperta;   // porta aperta / con corrente

    [Header("Collider da disabilitare quando si apre")]
    [SerializeField] private Collider2D doorCollider;

    [Header("Tooltip interagisci (opzionale)")]
    [SerializeField] private GameObject tooltipNoCurrent; // es. "Nessuna alimentazione"

    private SpriteRenderer sr;
    private bool powered = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr && spriteSpenta) sr.sprite = spriteSpenta;
        if (tooltipNoCurrent) tooltipNoCurrent.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || powered) return;
        if (tooltipNoCurrent) tooltipNoCurrent.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (tooltipNoCurrent) tooltipNoCurrent.SetActive(false);
    }

    /// <summary>
    /// Chiamato da WirePuzzleManager quando tutti i fili sono collegati.
    /// </summary>
    public void PowerRestored()
    {
        powered = true;
        if (sr && spriteAperta) sr.sprite = spriteAperta;
        if (doorCollider) doorCollider.enabled = false;
        if (tooltipNoCurrent) tooltipNoCurrent.SetActive(false);
        Debug.Log("[DoorPowerManager] Porta aperta — alimentazione ripristinata.");
    }
}

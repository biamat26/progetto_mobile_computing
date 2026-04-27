using UnityEngine;

public class DoorPowerManager : MonoBehaviour
{
    [Header("Sprites Top")]
    [SerializeField] private SpriteRenderer topRenderer;
    [SerializeField] private Sprite spriteTopClosed;
    [SerializeField] private Sprite spriteTopOpen;

    [Header("Sprites Bottom")]
    [SerializeField] private SpriteRenderer bottomRenderer;
    [SerializeField] private Sprite spriteBottomClosed;
    [SerializeField] private Sprite spriteBottomOpen;

    [Header("Collider da disabilitare quando si apre")]
    [SerializeField] private Collider2D doorCollider;

    [Header("Tooltip nessuna corrente (opzionale)")]
    [SerializeField] private GameObject tooltipNoCurrent;

    private bool powered = false;

    void Awake()
    {
        if (topRenderer && spriteTopClosed) topRenderer.sprite = spriteTopClosed;
        if (bottomRenderer && spriteBottomClosed) bottomRenderer.sprite = spriteBottomClosed;
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

    public void PowerRestored()
    {
        powered = true;
        if (topRenderer && spriteTopOpen) topRenderer.sprite = spriteTopOpen;
        if (bottomRenderer && spriteBottomOpen) bottomRenderer.sprite = spriteBottomOpen;
        if (doorCollider) doorCollider.enabled = false;
        if (tooltipNoCurrent) tooltipNoCurrent.SetActive(false);
        Debug.Log("[DoorPowerManager] Porta aperta.");
    }

    public bool IsOpen() => powered;
}
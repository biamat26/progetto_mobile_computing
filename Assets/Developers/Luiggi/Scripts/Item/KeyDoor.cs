using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public GameObject tooltipPanel;
    public GameObject errorPanel;      // pannello "Non hai la chiave corretta"

    [Header("Riferimenti")]
    public Collider2D physicalCollider; // trascina qui il BoxCollider non-trigger
    [Header("Sprites")]
    public Sprite spriteClosed;
    public Sprite spriteOpen;

    [Header("Impostazioni")]
    public float interactionDistance = 3f;
    public Transform door;
    public Transform player;
    public KeyColor requiredColor;

    private bool isNearDoor = false;
    private bool isOpen = false;
    private float errorTimer = 0f;

    [Header("Porta aperta")]
    public GameObject portaSuperiore;
    public GameObject portaInferiore;

    [Header("Audio Porta")]
    public AudioSource audioSource;
    public AudioClip openSound;

    void Start()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);
        GetComponent<SpriteRenderer>().sprite = spriteClosed;
    }

    void Update()
    {
        CheckProximity();
        HandleInput();
        HandleErrorTimer();
    }

    void CheckProximity()
    {
        if (player == null || door == null) return;
        float dist = Vector2.Distance(player.position, door.position);
        isNearDoor = dist <= interactionDistance;
        if (tooltipPanel != null) tooltipPanel.SetActive(isNearDoor && !isOpen);
    }

    void HandleInput()
    {
        if (!isNearDoor || isOpen) return;
        if (Input.GetKeyDown(KeyCode.E)) TryOpenDoor();
    }

    void HandleErrorTimer()
    {
        if (errorTimer <= 0) return;
        errorTimer -= Time.deltaTime;
        if (errorTimer <= 0 && errorPanel != null)
            errorPanel.SetActive(false);
    }

    void TryOpenDoor()
    {
        for (int i = 0; i < 16; i++)
        {
            ItemData item = InventorySystem.Instance.GetItem(i);
            if (item != null && item.itemType == ItemType.Key && item.keyColor == requiredColor)
            {
                InventorySystem.Instance.RemoveItem(i);
                OpenDoor();
                return;
            }
        }

        // Nessuna chiave corretta
        ShowError();
    }

    void ShowError()
    {
        Debug.Log("ShowError chiamato | errorPanel null? " + (errorPanel == null));
        if (errorPanel == null) return;
        errorPanel.SetActive(true);
        errorTimer = 2f;
    }

    void OpenDoor()
    {
        isOpen = true;
        GetComponent<SpriteRenderer>().enabled = false; // nasconde porta chiusa
        if (portaSuperiore != null) portaSuperiore.SetActive(true);
        if (portaInferiore != null) portaInferiore.SetActive(true);
        if (physicalCollider != null) physicalCollider.enabled = false;
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);

        PlayDoorSound();
    }

    void PlayDoorSound()
    {
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
    }
}
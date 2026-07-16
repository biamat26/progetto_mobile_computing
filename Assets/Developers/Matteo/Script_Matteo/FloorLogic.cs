using UnityEngine;

public class FloorToggle : MonoBehaviour
{
    public Sprite offSprite;
    public Sprite onSprite;

    [Header("Audio Mattonella")]
    public AudioSource audioSource;
    public AudioClip soundOn;
    public AudioClip soundOff;

    private SpriteRenderer sr;
    public bool isOn = false;
    private bool playerInside = false;

    // quando true, la mattonella non risponde più al player
    private bool bloccata = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = offSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (bloccata) return;

        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            Toggle();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void Toggle()
    {
        isOn = !isOn;

        if (isOn)
        {
            sr.sprite = onSprite;
            if (audioSource != null && soundOn != null)
                audioSource.PlayOneShot(soundOn);
        }
        else
        {
            sr.sprite = offSprite;
            if (audioSource != null && soundOff != null)
                audioSource.PlayOneShot(soundOff);
        }
    }

    /// <summary>
    /// Chiamato da GestorePortaPuzzle quando il puzzle è risolto.
    /// Da questo momento la mattonella ignora il player.
    /// </summary>
    public void BloccaMattonella()
    {
        bloccata = true;
    }
}
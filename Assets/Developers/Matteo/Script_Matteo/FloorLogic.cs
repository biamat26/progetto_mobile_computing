using UnityEngine;

public class FloorToggle : MonoBehaviour
{
    public Sprite offSprite;
    public Sprite onSprite;

    [Header("Audio Mattonella")]
    public AudioSource audioSource;
    public AudioClip soundOn;   // Suono quando si accende
    public AudioClip soundOff;  // Suono quando si spegne
    

    private SpriteRenderer sr;
    public bool isOn = false;
    private bool playerInside = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = offSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            Toggle();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void Toggle()
    {
        isOn = !isOn;

        if(isOn)
        {
            sr.sprite = onSprite;

            if (audioSource != null && soundOn != null)
            {
                audioSource.PlayOneShot(soundOn);
            }
        }
        else
        {
            sr.sprite = offSprite;
            
            if (audioSource != null && soundOff != null)
            {
                audioSource.PlayOneShot(soundOff);
            }
        }
    }
}
using UnityEngine;

public class FloorToggle : MonoBehaviour
{
    public Sprite offSprite;
    public Sprite onSprite;

    private SpriteRenderer sr;
    private bool isOn = false;
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
            sr.sprite = onSprite;
        else
            sr.sprite = offSprite;
    }
}
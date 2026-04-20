using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;
    [SerializeField] private GameObject pickupPrompt;
    private bool playerNearby = false;

    void Start()
    {
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            bool picked = InventorySystem.Instance.AddItem(itemData);
            if (picked) Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (pickupPrompt != null) pickupPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (pickupPrompt != null) pickupPrompt.SetActive(false);
        }
    }
}
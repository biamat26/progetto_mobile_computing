using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;
    [SerializeField] private float pickupRange = 1.5f;
    private GameObject pickupPrompt;
    private Transform playerTransform;
    private bool playerNearby = false;

    void Start()
    {
        pickupPrompt = transform.Find("PickUpPrompt")?.gameObject;
        if (pickupPrompt != null) pickupPrompt.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            else return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        bool inRange = distance <= pickupRange;
        playerNearby = inRange;
        if (pickupPrompt != null) pickupPrompt.SetActive(inRange);

        if (playerNearby && InputManager.Interact)
        {
            if (InventorySystem.Instance == null) return;
            bool picked = InventorySystem.Instance.AddItem(itemData);
            if (picked) Destroy(gameObject);
        }
    }
}
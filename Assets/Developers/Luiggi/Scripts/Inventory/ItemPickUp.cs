using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inv = other.GetComponent<Inventory>();
            if (inv != null && inv.AddItem(item))
            {
                Destroy(gameObject);
            }
        }
    }
}
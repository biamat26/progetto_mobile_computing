using UnityEngine;

public class DropButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform playerTransform;

    public void OnDrop()
    {
        InventorySystem.Instance.DropSelected(itemPrefab, playerTransform.position);
    }
}
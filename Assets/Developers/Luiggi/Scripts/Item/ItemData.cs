using UnityEngine;

public enum ItemType { Generic, Heal, Key }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea(3, 10)] public string contenuto;
    public ItemType itemType;
    public int healAmount = 30;
}
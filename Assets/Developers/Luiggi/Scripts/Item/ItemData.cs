using UnityEngine;

public enum ItemType { Generic, Heal, Key }
public enum KeyColor { None, Purple, Blue } 



[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea(3, 10)] public string contenuto;

    public KeyColor keyColor;

    public Sprite immagineDocumento; // aggiungi questo
    public ItemType itemType;
    public int healAmount = 30;
}
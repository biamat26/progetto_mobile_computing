using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;
    public GameObject[] slots = new GameObject[16];
    private ItemData[] items = new ItemData[16];

    private int selectedSlot = -1;

public void SelectSlot(int index)
{
    selectedSlot = index;
}

public void DropSelected(GameObject dropPrefab, Vector3 playerPosition)
{
    if (selectedSlot == -1 || items[selectedSlot] == null) return;

    ItemData itemToDrop = items[selectedSlot];

    GameObject dropped = Instantiate(dropPrefab, playerPosition + Vector3.right, Quaternion.identity);
    
    // assegna l'itemData
    WorldItem wi = dropped.GetComponent<WorldItem>();
    if (wi != null) wi.itemData = itemToDrop;
    
    // assegna lo sprite corretto
    SpriteRenderer sr = dropped.GetComponent<SpriteRenderer>();
    if (sr != null) sr.sprite = itemToDrop.icon;

    // rimuovi dall'inventario
    items[selectedSlot] = null;
    Transform slot = slots[selectedSlot].transform;
    Transform parent = slot.Find("SlotBG");
    if (parent == null) parent = slot;
    Transform icon = parent.Find("Icon");
    if (icon != null) Destroy(icon.gameObject);

    
    if (selectedSlot != -1 && selectedSlot < slots.Length && slots[selectedSlot] != null)
{
    InventorySlotBorder slotBorder = slots[selectedSlot].GetComponent<InventorySlotBorder>();
    if (slotBorder != null) slotBorder.Deselect();
}
    selectedSlot = -1;
}

    void Awake() { Instance = this; }

public bool AddItem(ItemData item)
{
    for (int i = 0; i < items.Length; i++)
    {
        if (items[i] == null)
        {
            items[i] = item;
            // spawna icona solo se il canvas è attivo
            if (slots[i] != null && slots[i].activeInHierarchy)
                SpawnIcon(i, item);
            return true;
        }
    }
    Debug.Log("Inventario pieno!");
    return false;
}
public void RefreshUI()
{
    for (int i = 0; i < items.Length; i++)
    {
        if (items[i] != null)
            SpawnIcon(i, items[i]);
    }
}

   void SpawnIcon(int index, ItemData item)
{
    if (slots[index] == null) return;
    
    Transform slot = slots[index].transform;
    Transform parent = slot.Find("SlotBG");
    if (parent == null) parent = slot;

    Transform old = parent.Find("Icon");
    if (old != null) Destroy(old.gameObject);

    GameObject iconGO = new GameObject("Icon");
    iconGO.transform.SetParent(parent, false);

    RectTransform rt = iconGO.AddComponent<RectTransform>();
    rt.anchorMin = Vector2.zero;
    rt.anchorMax = Vector2.one;
    rt.offsetMin = new Vector2(4, 4);
    rt.offsetMax = new Vector2(-4, -4);

    Image img = iconGO.AddComponent<Image>();
    img.sprite = item.icon;
    img.preserveAspect = true;
}

}
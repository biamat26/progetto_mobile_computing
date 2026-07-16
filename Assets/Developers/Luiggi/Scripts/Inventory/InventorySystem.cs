using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;
    public GameObject[] slots = new GameObject[16];
    private ItemData[] items = new ItemData[16];

    [Header("Visibilità per Scena")]
    public string[] sceneDiGioco;
    public GameObject inventoryRoot;

    public int GetSelectedSlot() => selectedSlot;
    public ItemData GetItem(int index) => items[index];
    private int selectedSlot = -1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool èSceneDiGioco = System.Array.Exists(sceneDiGioco, nome => nome == scene.name);

        if (èSceneDiGioco)
        {
            FindSlotsInScene();
            RefreshUI();
        }

        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);
    }

    private void FindSlotsInScene()
    {
        if (inventoryRoot == null)
            inventoryRoot = GameObject.Find("InventoryRoot");

        if (inventoryRoot == null) return;

        InventorySlotBorder[] foundSlots = inventoryRoot.GetComponentsInChildren<InventorySlotBorder>(true);
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < foundSlots.Length)
                slots[i] = foundSlots[i].gameObject;
            else
                slots[i] = null;
        }
        Debug.Log("InventorySystem: Slot ricollegati automaticamente.");
    }

    public void SelectSlot(int index) => selectedSlot = index;

    public void DeselectCurrentSlot()
    {
        if (selectedSlot != -1 && selectedSlot < slots.Length && slots[selectedSlot] != null)
        {
            InventorySlotBorder slotBorder = slots[selectedSlot].GetComponent<InventorySlotBorder>();
            if (slotBorder != null) slotBorder.Deselect();
        }
        selectedSlot = -1;
    }

    public void DropSelected(GameObject dropPrefab, Vector3 playerPosition)
    {
        if (selectedSlot == -1 || items[selectedSlot] == null) return;

        ItemData itemToDrop = items[selectedSlot];
        GameObject dropped = Instantiate(dropPrefab, playerPosition + Vector3.right, Quaternion.identity);

        // Applica la scala definita sull'asset ItemData
        dropped.transform.localScale = itemToDrop.scalaDrop;

        WorldItem wi = dropped.GetComponent<WorldItem>();
        if (wi != null) wi.itemData = itemToDrop;

        SpriteRenderer sr = dropped.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = itemToDrop.icon;

        items[selectedSlot] = null;
        Transform slot = slots[selectedSlot].transform;
        Transform parent = slot.Find("SlotBG") != null ? slot.Find("SlotBG") : slot;
        Transform icon = parent.Find("Icon");
        if (icon != null) Destroy(icon.gameObject);

        DeselectCurrentSlot();
    }

    public void RemoveItem(int index)
    {
        items[index] = null;
        Transform slot = slots[index].transform;
        Transform parent = slot.Find("SlotBG") != null ? slot.Find("SlotBG") : slot;
        Transform icon = parent.Find("Icon");
        if (icon != null) Destroy(icon.gameObject);

        if (selectedSlot == index) DeselectCurrentSlot();
        else selectedSlot = -1;
    }

    public void ClearInventory()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                items[i] = null;
                if (slots[i] != null)
                {
                    Transform slot = slots[i].transform;
                    Transform parent = slot.Find("SlotBG") != null ? slot.Find("SlotBG") : slot;
                    Transform icon = parent.Find("Icon");
                    if (icon != null) Destroy(icon.gameObject);
                }
            }
        }
        DeselectCurrentSlot();
    }

    public bool AddItem(ItemData item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
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
        Transform parent = slot.Find("SlotBG") != null ? slot.Find("SlotBG") : slot;

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
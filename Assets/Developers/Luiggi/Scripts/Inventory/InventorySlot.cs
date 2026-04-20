using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventorySlotBorder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Color normalBorderColor = new Color(0.227f, 0.478f, 0.353f, 1f);
    [SerializeField] private Color hoverBorderColor  = new Color(0.18f, 0.8f, 0.44f, 1f);
    [SerializeField] private Color selectedBorderColor = new Color(0.4f, 1f, 0.6f, 1f);

    [SerializeField] private Color normalBgColor   = new Color(0.031f, 0.047f, 0.039f, 1f);
    [SerializeField] private Color hoverBgColor    = new Color(0.05f, 0.13f, 0.07f, 1f);
    [SerializeField] private Color selectedBgColor = new Color(0.07f, 0.18f, 0.1f, 1f);

    [SerializeField] private int borderSize = 2;
    [SerializeField] private float fadeSpeed = 10f;

    private Image borderImage;
    private Image bgImage;
    private bool isSelected = false;
    private Color targetBorder;
    private Color targetBg;

    [SerializeField] public int slotIndex = -1;

    void Start()
    {
        borderImage = GetComponent<Image>();
        borderImage.color = normalBorderColor;
        targetBorder = normalBorderColor;

        GameObject inner = new GameObject("SlotBG");
        inner.transform.SetParent(transform, false);

        RectTransform rt = inner.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(borderSize, borderSize);
        rt.offsetMax = new Vector2(-borderSize, -borderSize);

        bgImage = inner.AddComponent<Image>();
        bgImage.color = normalBgColor;
        targetBg = normalBgColor;
    }

    public void Deselect()
{
    isSelected = false;
    targetBorder = normalBorderColor;
    targetBg = normalBgColor;
}

    void Update()
    {
        if (borderImage) borderImage.color = Color.Lerp(borderImage.color, targetBorder, Time.deltaTime * fadeSpeed);
        if (bgImage) bgImage.color = Color.Lerp(bgImage.color, targetBg, Time.deltaTime * fadeSpeed);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (isSelected) return;
        targetBorder = hoverBorderColor;
        targetBg = hoverBgColor;
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (isSelected) return;
        targetBorder = normalBorderColor;
        targetBg = normalBgColor;
    }

   public void OnPointerClick(PointerEventData e)
{
    isSelected = !isSelected;
    targetBorder = isSelected ? selectedBorderColor : normalBorderColor;
    targetBg     = isSelected ? selectedBgColor     : normalBgColor;

    if (isSelected)
        InventorySystem.Instance.SelectSlot(slotIndex);
    else
        InventorySystem.Instance.SelectSlot(-1);
}
}
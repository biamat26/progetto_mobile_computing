using UnityEngine;
using UnityEngine.UI;

public class InventorySlotBorder : MonoBehaviour
{
    [SerializeField] private Color borderColor = new Color(0.227f, 0.478f, 0.353f, 1f);
    [SerializeField] private Color bgColor = new Color(0.031f, 0.047f, 0.039f, 1f);
    [SerializeField] private int borderSize = 2;

    void Start()
    {
        GetComponent<Image>().color = borderColor;

        GameObject inner = new GameObject("SlotBG");
        inner.transform.SetParent(transform, false);

        RectTransform rt = inner.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(borderSize, borderSize);
        rt.offsetMax = new Vector2(-borderSize, -borderSize);

        Image img = inner.AddComponent<Image>();
        img.color = bgColor;
    }
}
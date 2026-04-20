using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    public Image borderImage;
    public Image bgImage;

    [Header("Colors")]
    public Color normalBorderColor = new Color(0.1f, 0.36f, 0.18f, 1f);   // #1A5C2E
    public Color hoverBorderColor  = new Color(0.18f, 0.8f, 0.44f, 1f);   // #2ECC71
    public Color normalBgColor     = new Color(0.024f, 0.059f, 0.035f, 1f); // #060F09
    public Color hoverBgColor      = new Color(0.05f, 0.13f, 0.07f, 1f);   // #0D2018

    [Header("Settings")]
    public float fadeSpeed = 8f;
    public bool isSelected = false;

    private bool isHovered = false;
    private Color targetBorder;
    private Color targetBg;

    void Start()
    {
        targetBorder = normalBorderColor;
        targetBg = normalBgColor;
        if (borderImage) borderImage.color = normalBorderColor;
        if (bgImage) bgImage.color = normalBgColor;
    }

    void Update()
    {
        if (borderImage) borderImage.color = Color.Lerp(borderImage.color, targetBorder, Time.deltaTime * fadeSpeed);
        if (bgImage) bgImage.color = Color.Lerp(bgImage.color, targetBg, Time.deltaTime * fadeSpeed);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        isHovered = true;
        targetBorder = hoverBorderColor;
        targetBg = hoverBgColor;
    }

    public void OnPointerExit(PointerEventData e)
    {
        isHovered = false;
        if (!isSelected)
        {
            targetBorder = normalBorderColor;
            targetBg = normalBgColor;
        }
    }

    public void OnPointerClick(PointerEventData e)
    {
        isSelected = !isSelected;
        if (!isSelected)
        {
            targetBorder = normalBorderColor;
            targetBg = normalBgColor;
        }
    }
}
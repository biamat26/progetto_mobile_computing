using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Color normalBorderColor = new Color(0.1f, 0.36f, 0.18f, 1f);
    [SerializeField] private Color hoverBorderColor  = new Color(0.18f, 0.8f, 0.44f, 1f);
    [SerializeField] private Color normalBgColor     = new Color(0.024f, 0.059f, 0.035f, 1f);
    [SerializeField] private Color hoverBgColor      = new Color(0.05f, 0.13f, 0.07f, 1f);
    [SerializeField] private int borderSize          = 2;
    [SerializeField] private float fadeSpeed         = 10f;

    private Image borderImage;
    private Image bgImage;
    private Color targetBorder;
    private Color targetBg;

    void Start()
    {
        // evita duplicati se Start() viene chiamato più volte
        Transform existing = transform.Find("BtnBG");
        if (existing != null)
        {
            bgImage = existing.GetComponent<Image>();
            borderImage = GetComponent<Image>();
            borderImage.color = normalBorderColor;
            targetBorder = normalBorderColor;
            targetBg = normalBgColor;
            return;
        }

        borderImage = GetComponent<Image>();
        borderImage.color = normalBorderColor;
        targetBorder = normalBorderColor;

        GameObject inner = new GameObject("BtnBG");
        inner.transform.SetParent(transform, false);
        inner.transform.SetAsFirstSibling();

        RectTransform rt = inner.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(borderSize, borderSize);
        rt.offsetMax = new Vector2(-borderSize, -borderSize);

        bgImage = inner.AddComponent<Image>();
        bgImage.color = normalBgColor;
        targetBg = normalBgColor;
    }

    void Update()
    {
        if (borderImage) borderImage.color = Color.Lerp(borderImage.color, targetBorder, Time.unscaledDeltaTime * fadeSpeed);
        if (bgImage) bgImage.color = Color.Lerp(bgImage.color, targetBg, Time.unscaledDeltaTime * fadeSpeed);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        targetBorder = hoverBorderColor;
        targetBg = hoverBgColor;
    }

    public void OnPointerExit(PointerEventData e)
    {
        targetBorder = normalBorderColor;
        targetBg = normalBgColor;
    }

    public void OnPointerClick(PointerEventData e)
    {
        borderImage.color = Color.white;
        bgImage.color = hoverBorderColor;

        // ---> ECCO LA NOSTRA SICURA! <---
        // Se l'inventario si è appena chiuso e ha spento questo bottone, 
        // usciamo dalla funzione senza far partire la Coroutine, evitando l'errore.
        if (!gameObject.activeInHierarchy) return;

        StartCoroutine(ResetAfterClick());
    }

    private System.Collections.IEnumerator ResetAfterClick()
    {
        yield return new WaitForSecondsRealtime(0.08f);
        targetBorder = normalBorderColor;
        targetBg = normalBgColor;
    }

    private void ResetColors()
    {
        targetBorder = normalBorderColor;
        targetBg = normalBgColor;
    }

    private System.Collections.IEnumerator ClickFlash()
    {
        borderImage.color = Color.white;
        bgImage.color = hoverBorderColor;
        yield return new WaitForSeconds(0.08f);
        targetBorder = hoverBorderColor;
        targetBg = hoverBgColor;
    }
}
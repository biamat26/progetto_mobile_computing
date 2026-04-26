using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WireConnector : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public enum Side { Left, Right }

    [Header("Configurazione")]
    public string wireColor;
    public Side side;

    [Header("Riferimenti")]
    public Image connectorImage;
    public RectTransform lineStart;

    [HideInInspector] public WirePuzzleManager manager;
    [HideInInspector] public bool isConnected = false;

    private static readonly Color colorRed   = new Color(0.85f, 0.15f, 0.15f);
    private static readonly Color colorBlue  = new Color(0.15f, 0.35f, 0.85f);
    private static readonly Color colorGreen = new Color(0.10f, 0.70f, 0.22f);

    public Color GetColorForWire(string c)
    {
        return c switch
        {
            "red"   => colorRed,
            "blue"  => colorBlue,
            "green" => colorGreen,
            _       => Color.white
        };
    }

    // IPointerDownHandler serve a Unity per "preparare" il drag
    public void OnPointerDown(PointerEventData e)
    {
          Debug.Log("ENTER " + gameObject.name);
        // deve esserci ma può essere vuoto
    }

    public void OnBeginDrag(PointerEventData e)
    {
        Debug.Log("BeginDrag: " + gameObject.name + " manager=" + manager);
        if (side != Side.Left || isConnected) return;
        if (manager == null) { Debug.LogError("Manager è null!"); return; }
        manager.BeginDrag(this, e);
    }

    public void OnDrag(PointerEventData e)
    {
        if (side != Side.Left) return;
        manager.OnDrag(e);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (side != Side.Left) return;
        manager.EndDrag(e);
    }

    public void OnDrop(PointerEventData e)
    {
        Debug.Log("OnDrop su " + gameObject.name + " side=" + side);
        if (side != Side.Right || isConnected) return;
        manager.TryConnect(this);
    }

    public void SetConnected(bool connected)
    {
        isConnected = connected;
        if (connectorImage != null)
            connectorImage.color = connected ? Color.gray : GetColorForWire(wireColor);
    }

    public void OnPointerEnter(PointerEventData e)
{
    Debug.Log("ENTER " + gameObject.name);
}
}
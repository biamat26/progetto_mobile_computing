using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragTest : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnPointerDown(PointerEventData e)
    {
        Debug.Log("POINTER DOWN su " + gameObject.name);
    }

    public void OnBeginDrag(PointerEventData e)
    {
        Debug.Log("BEGIN DRAG su " + gameObject.name);
    }

    public void OnDrag(PointerEventData e)
    {
        Debug.Log("DRAGGING");
    }

    public void OnEndDrag(PointerEventData e)
    {
        Debug.Log("END DRAG");
    }
}

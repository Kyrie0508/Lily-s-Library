using UnityEngine;
using UnityEngine.EventSystems;

public class MapDragger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform target; 

    private Vector2 lastMousePosition;
    private bool dragging = false;

    public float dragSpeed = 1f;
    public float minY = -1000f;
    public float maxY = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        lastMousePosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || target == null) return;

        Vector2 delta = eventData.position - lastMousePosition;
        Vector2 newPos = target.anchoredPosition + new Vector2(0, delta.y * dragSpeed);
        
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        target.anchoredPosition = newPos;

        lastMousePosition = eventData.position;
    }
}
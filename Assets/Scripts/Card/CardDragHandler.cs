using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // 최상위 Canvas 찾기
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform, true); // 드래그 시 최상단으로 이동
        canvasGroup.blocksRaycasts = false; // 드래그 중에는 Raycast 막기 끔
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent, true); // 원래 부모로 복귀
        canvasGroup.blocksRaycasts = true;
    }
}
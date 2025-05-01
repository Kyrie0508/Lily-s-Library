using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private BattleManager battleManager;
    private Card cardData;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        cardData = GetComponent<Card>();
        battleManager = FindAnyObjectByType<BattleManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetCanvasScaleFactor();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (IsOnValidField(eventData))
        {
            Debug.Log($"{cardData.cardType} 카드 사용 예약!");

            if (battleManager != null && cardData != null)
            {
                battleManager.RegisterUsedCard(cardData.gameObject);
            }

            // 이제 드래그 종료해도 Destroy 안 함!
            // 그냥 필드에 남아있게 한다
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    private bool IsOnValidField(PointerEventData eventData)
    {
        return true;
    }

    private float GetCanvasScaleFactor()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas != null ? canvas.scaleFactor : 1f;
    }
}

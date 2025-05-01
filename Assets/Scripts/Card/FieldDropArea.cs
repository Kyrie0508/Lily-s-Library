using UnityEngine;
using UnityEngine.EventSystems;

public class FieldDropArea : MonoBehaviour, IDropHandler
{
    public BattleManager battleManager; // 연결 필요

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;

        if (droppedCard != null)
        {
            Debug.Log($"{droppedCard.name} 카드 사용!");

            droppedCard.transform.SetParent(transform, false);

            // 카드 등록
            Card card = droppedCard.GetComponent<Card>();
            if (card != null)
            {
                battleManager.RegisterUsedCard(droppedCard);
            }
        }
    }
}
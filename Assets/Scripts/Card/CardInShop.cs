using UnityEngine;
using UnityEngine.EventSystems;

public class CardInShop : MonoBehaviour, IPointerClickHandler
{
    public UnitCardSO cardData;

    public void OnPointerClick(PointerEventData eventData)
    {
        BattleManager bm = BattleManager.Instance;
        if (bm == null) return;
        
        if (bm.IsHandFull())
        {
            Debug.Log("손패가 가득 찼습니다.");
            return;
        }

        if (bm.playerGold < cardData.cost)
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }
        // 골드 차감
        bm.playerGold -= cardData.cost;
        bm.UpdateUI();
        SoundManager.Instance.PlayPurchaseSound();
        // 카드 인스턴스 생성
        GameObject newCard = Instantiate(bm.cardPrefab);
        Card card = newCard.GetComponent<Card>();
        card.SetCardData(cardData);

        // 손패에 추가
        bm.AddCardToHand(card);

        // 상점에서 제거
        Destroy(gameObject);
    }
}
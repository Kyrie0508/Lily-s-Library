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

        // 카드 인스턴스 생성
        GameObject newCard = Instantiate(bm.cardPrefab);
        Card card = newCard.GetComponent<Card>();
        card.cardName = cardData.cardName;
        card.cost = cardData.cost;
        card.attack = cardData.attack;
        card.hp = cardData.hp;
        card.effectTrigger = cardData.effectTrigger;
        card.effectType = cardData.effectType;
        card.effectValue = cardData.effectValue;

        // 손패에 추가
        bm.AddCardToHand(card);

        // 상점에서 제거
        Destroy(gameObject);
    }
}
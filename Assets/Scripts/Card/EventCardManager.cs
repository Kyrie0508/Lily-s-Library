using System.Collections.Generic;
using UnityEngine;

public class EventCardManager : MonoBehaviour
{
    public static EventCardManager Instance;

    public List<EventCardData> playerEventDeck = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddToDeck(EventCardData card)
    {
        playerEventDeck.Add(card);
        Debug.Log($"이벤트 카드 추가됨: {card}");
    }
    
    public void UpgradeCard(EventCardData card)
    {
        if (card == null) return;

        switch (card.type)
        {
            case "Sword":
            case "Shield":
                card.value += 3;
                break;
            case "Star":
                card.value *= 2;
                break;
            case "Book":
                return; // 강화 불가 카드
        }
        
        switch (card.type)
        {
            case "Sword":
            case "Shield":
                if (card.value > 6) card.value = 6;
                break;
            case "Star":
                if (card.value > 4) card.value = 4;
                break;
        }
    }


}
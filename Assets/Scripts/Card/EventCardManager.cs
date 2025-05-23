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

    public EventCardData GetRandomCardFromDeck()
    {
        if (playerEventDeck.Count == 0) return null;
        return playerEventDeck[Random.Range(0, playerEventDeck.Count)];
    }
}
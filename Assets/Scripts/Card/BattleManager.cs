using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("카드 프리팹 (Sword, Book, Shield, Star 순서)")]
    public GameObject swordPrefab;
    public GameObject bookPrefab;
    public GameObject shieldPrefab;
    public GameObject starPrefab;

    [Header("카드 배치 영역")]
    public Transform playerHandArea;
    public Transform enemyHandArea;

    private List<DeckCardData> deck = new List<DeckCardData>();
    private List<GameObject> usedCards = new();
    public Transform playerFieldTransform;
    private int currentDrawIndex = 0;
    private bool isPlayerTurn = true;

    void Start()
    {
        Debug.Log("BattleManager Start 호출됨");
        GenerateDeck();
        DealInitialHands();
    }

    void GenerateDeck()
    {
        deck.Clear();
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            for (int i = 0; i < 15; i++)
            {
                deck.Add(new DeckCardData
                {
                    cardType = type,
                    power = Random.Range(1, 7) // 1~6 랜덤 수치
                });
            }
        }

        // 셔플
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    void DealInitialHands()
    {
        for (int i = 0; i < 10; i++) // 플레이어 5장, 적 5장
        {
            DrawNextCard();
        }
    }
    public void RegisterUsedCard(GameObject cardObj)
    {
        if (!usedCards.Contains(cardObj))
        {
            usedCards.Add(cardObj);
        }
    }
    
    public bool IsPlayerCard(Card card)
    {
        return card.transform.parent == playerHandArea;
    }


    public void DrawNextCard()
    {
        if (currentDrawIndex >= deck.Count) return;

        DeckCardData data = deck[currentDrawIndex++];
        GameObject prefab = GetPrefabByType(data.cardType);
        Transform parent = isPlayerTurn ? playerHandArea : enemyHandArea;

        GameObject cardObj = Instantiate(prefab, parent);
        Card card = cardObj.GetComponent<Card>();
        card.cardType = data.cardType;
        card.power = data.power;
        card.UpdateUI();
        Debug.Log($"드로우: {data.cardType} {data.power}");
        isPlayerTurn = !isPlayerTurn;
    }

    GameObject GetPrefabByType(CardType type)
    {
        return type switch
        {
            CardType.Sword => swordPrefab,
            CardType.Book => bookPrefab,
            CardType.Shield => shieldPrefab,
            CardType.Star => starPrefab,
            _ => swordPrefab
        };
    }
}

public class DeckCardData
{
    public CardType cardType;
    public int power;
}

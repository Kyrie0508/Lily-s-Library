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
    public Transform playerFieldTransform;
    public Transform enemyHandArea;

    [Header("손패 슬롯 6개")]
    public Transform[] handSlots = new Transform[6];

    private bool[] handOccupied = new bool[6];
    private List<Card> handCards = new();
    private List<Card> fieldCards = new();
    private List<DeckCardData> deck = new();
    private int currentDrawIndex = 0;
    private bool isPlayerTurn = true;

    void Start()
    {
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
                    power = Random.Range(1, 7)
                });
            }
        }

        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    void DealInitialHands()
    {
        for (int i = 0; i < 6; i++) // 최대 6장까지 손패
        {
            DrawNextCard();
        }
    }

    public void DrawNextCard()
    {
        if (currentDrawIndex >= deck.Count || handCards.Count >= 6)
            return;

        DeckCardData data = deck[currentDrawIndex++];
        GameObject prefab = GetPrefabByType(data.cardType);
        GameObject cardObj = Instantiate(prefab);
        Card card = cardObj.GetComponent<Card>();
        card.cardType = data.cardType;
        card.power = data.power;
        card.UpdateUI();

        AddCardToHand(card);
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

    void AddCardToHand(Card card)
    {
        for (int i = 0; i < handSlots.Length; i++)
        {
            if (!handOccupied[i])
            {
                handOccupied[i] = true;
                card.handIndex = i;
                card.currentLocation = CardLocation.Hand;
                card.transform.SetParent(handSlots[i], false);
                card.transform.localPosition = Vector3.zero;
                handCards.Add(card);
                break;
            }
        }
    }

    public void MoveCardToField(Card card)
    {
        if (card.currentLocation != CardLocation.Hand) return;

        handOccupied[card.handIndex] = false;
        handCards.Remove(card);

        card.currentLocation = CardLocation.Field;
        card.transform.SetParent(playerFieldTransform, false);
        card.handIndex = -1;

        fieldCards.Add(card);

        SortHand();
        SortFieldCards(); 
    }


    public void ReturnCardToHand(Card card)
    {
        if (card.currentLocation != CardLocation.Field) return;

        for (int i = 0; i < handSlots.Length; i++)
        {
            if (!handOccupied[i])
            {
                handOccupied[i] = true;
                card.handIndex = i;
                card.currentLocation = CardLocation.Hand;
                card.transform.SetParent(handSlots[i], false);
                card.transform.localPosition = Vector3.zero;

                fieldCards.Remove(card);
                handCards.Add(card);

                SortHand();
                break;
            }
        }
    }

    public void SortHand()
    {
        handCards.Sort((a, b) => a.handIndex.CompareTo(b.handIndex)); // Optional

        for (int i = 0; i < handOccupied.Length; i++)
            handOccupied[i] = false;

        for (int i = 0; i < handCards.Count; i++)
        {
            handCards[i].handIndex = i;
            handCards[i].transform.SetParent(handSlots[i], false);
            handCards[i].transform.localPosition = Vector3.zero;
            handOccupied[i] = true;
        }
    }
    
    void SortFieldCards()
    {
        float spacing = 140f; 
        float startX = -((fieldCards.Count - 1) * spacing) / 2f;
        float yOffset = 250f; 

        for (int i = 0; i < fieldCards.Count; i++)
        {
            Vector3 targetPos = new Vector3(startX + i * spacing, yOffset, 0f);
            fieldCards[i].transform.localPosition = targetPos;
        }
    }
    
    public void OnOKButtonClick()
    {
        Debug.Log("OK 버튼 클릭됨 - 필드 카드 분석 시작");

        // 카드 타입별 합계 저장
        int swordSum = 0;
        int bookSum = 0;
        int shieldSum = 0;
        int starSum = 0;

        foreach (Card card in fieldCards)
        {
            switch (card.cardType)
            {
                case CardType.Sword: swordSum += card.power; break;
                case CardType.Book: bookSum += card.power; break;
                case CardType.Shield: shieldSum += card.power; break;
                case CardType.Star: starSum += card.power; break;
            }
        }

        Debug.Log($"Sword: {swordSum}, Book: {bookSum}, Shield: {shieldSum}, Star: {starSum}");

        // 예시 스킬 조건: Sword 합이 5 이상이면 컷씬
        if (swordSum >= 5)
        {
            Debug.Log("스킬 조건 충족: Sword ≥ 5");
            FindAnyObjectByType<SkillCutinManager>()?.ShowSkillCutin();
        }
        else
        {
            Debug.Log("스킬 조건 미충족");
        }

        // (선택) 턴 전환 로직 들어갈 자리
        // StartNextTurn();
        
        // 필드 비우기
        foreach (Card card in fieldCards)
        {
            Destroy(card.gameObject);
        }
        fieldCards.Clear();
    }


}

public class DeckCardData
{
    public CardType cardType;
    public int power;
}

using System.Collections.Generic;
using System.Collections;
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
    
    [SerializeField] private Transform enemyFieldTransform;
    [SerializeField] private List<Card> enemyCards = new List<Card>();

    private bool[] handOccupied = new bool[6];
    private List<Card> handCards = new();
    private List<Card> fieldCards = new();
    private List<DeckCardData> deck = new();
    private int currentDrawIndex = 0;
    private bool isPlayerTurn = true;
    private bool isTurnProcessing = false;


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
    
    public void DrawUntilHandFull()
    {
        while (handCards.Count < 6 && currentDrawIndex < deck.Count)
        {
            DrawNextCard();
        }
    }

    
    public void OnOKButtonClick()
    {
        if (!isPlayerTurn || isTurnProcessing) return;

        isTurnProcessing = true;
        isPlayerTurn = false;

        Debug.Log("OK 버튼 클릭 - 플레이어 턴 종료, 적 턴 시작");

        // 카드 분석, 컷씬, 카드 삭제는 모두 적 턴 종료 시점에서 수행
        StartCoroutine(StartEnemyTurn());
    }
    
    private IEnumerator StartEnemyTurn()
    {
        Debug.Log("적의 턴 시작");

        yield return new WaitForSeconds(1f);

        // 🔹 적 카드 생성
        int enemyCardCount = Random.Range(1, 3);
        enemyCards.Clear();

        for (int i = 0; i < enemyCardCount; i++)
        {
            CardType type = (CardType)Random.Range(0, 4);
            int power = Random.Range(1, 6);

            GameObject prefab = GetPrefabByType(type);
            GameObject go = Instantiate(prefab, enemyFieldTransform);
            Card card = go.GetComponent<Card>();

            card.SetData(type, power);
            card.SetInteractable(false);

            // 위치 정렬 (좌우 배치)
            float spacing = 140f;
            float startX = -(enemyCardCount - 1) * spacing / 2f;
            card.transform.localPosition = new Vector3(startX + i * spacing, 80f, 0f);

            enemyCards.Add(card);

            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(0.5f);

        // 🔹 카드 수치 계산 (플레이어 + 적)
        int swordSum = 0, bookSum = 0, shieldSum = 0, starSum = 0;

        foreach (var card in fieldCards)
        {
            switch (card.cardType)
            {
                case CardType.Sword: swordSum += card.power; break;
                case CardType.Book: bookSum += card.power; break;
                case CardType.Shield: shieldSum += card.power; break;
                case CardType.Star: starSum += card.power; break;
            }
        }

        foreach (var card in enemyCards)
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

        // 🔹 컷씬 출력 조건
        if (swordSum >= 5)
        {
            Debug.Log("스킬 조건 충족: Sword ≥ 5");
            FindAnyObjectByType<SkillCutinManager>()?.ShowSkillCutin();
            yield return new WaitForSeconds(1.2f); // 컷씬 연출 여유
        }
        else
        {
            Debug.Log("스킬 조건 미충족");
            yield return new WaitForSeconds(0.5f);
        }

        // 🔹 카드 삭제
        foreach (var card in fieldCards)
            Destroy(card.gameObject);
        foreach (var card in enemyCards)
            Destroy(card.gameObject);

        fieldCards.Clear();
        enemyCards.Clear();

        yield return new WaitForSeconds(0.2f);

        // 🔹 턴 전환
        isPlayerTurn = true;
        isTurnProcessing = false;

        Debug.Log("플레이어 턴 시작");

        DrawUntilHandFull(); // 손패가 6장이 되도록 자동 보충
    }

    
    public bool IsPlayerTurn() => isPlayerTurn;


}

public class DeckCardData
{
    public CardType cardType;
    public int power;
}


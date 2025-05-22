using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public List<Transform> playerHandSlots;
    public List<Transform> playerFieldSlots;

    public GameObject cardPrefab;
    public TMP_Text goldText;
    public TMP_Text xpText;
    public TMP_Text levelText;

    public float turnDuration = 30f;
    private float turnTimer;

    public int playerGold = 0;
    public int playerXP = 0;
    public int playerLevel = 1;

    private bool isPlayerTurn = true;
    private bool isTurnProcessing = false;

    public List<Card> playerHandCards = new List<Card>();
    public List<Card> playerFieldCards = new List<Card>();

    private readonly int maxHandSize = 6;

    private Dictionary<int, int> xpTable = new Dictionary<int, int>()
    {
        {1, 2}, {2, 2}, {3, 6}, {4, 10}, {5, 20}, {6, 36},
        {7, 48}, {8, 76}, {9, 76}
    };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(PlayerTurnLoop());
    }

    IEnumerator PlayerTurnLoop()
    {
        while (true)
        {
            yield return StartCoroutine(StartPlayerTurn());
            yield return StartCoroutine(StartEnemyTurn());
        }
    }

    IEnumerator StartPlayerTurn()
    {
        isPlayerTurn = true;
        isTurnProcessing = false;

        playerGold = Mathf.Min(playerGold + 1, 10);
        playerXP += 2;
        TryLevelUp();

        UpdateUI();

        DrawEventCard();
        RefreshShop();

        turnTimer = turnDuration;
        while (turnTimer > 0f)
        {
            turnTimer -= Time.deltaTime;
            yield return null;
        }

        OnOKButtonClick(); // 자동 턴 종료
    }

    public void OnOKButtonClick()
    {
        if (!isPlayerTurn || isTurnProcessing) return;

        Debug.Log("OK 버튼 클릭 - 턴 종료");

        isPlayerTurn = false;
        isTurnProcessing = true;

        StartCoroutine(StartEnemyTurn());
    }

    IEnumerator StartEnemyTurn()
    {
        Debug.Log("적 턴 시작");

        yield return new WaitForSeconds(1f);

        // 전투 처리 (예: 카드 간 비교/자동 데미지 등)

        foreach (var card in playerFieldCards)
            Destroy(card.gameObject);
        playerFieldCards.Clear();

        yield return new WaitForSeconds(1f);

        Debug.Log("적 턴 종료 → 다음 턴");
    }

    public void TryLevelUp()
    {
        if (playerLevel >= 10) return;

        int neededXP = xpTable.ContainsKey(playerLevel) ? xpTable[playerLevel] : int.MaxValue;
        if (playerXP >= neededXP)
        {
            playerXP -= neededXP;
            playerLevel++;
            Debug.Log("레벨업! 현재 레벨: " + playerLevel);
        }
    }

    public void BuyXP()
    {
        if (playerGold >= 4)
        {
            playerGold -= 4;
            playerXP += 4;
            TryLevelUp();
            UpdateUI();
        }
    }

    public void DrawEventCard()
    {
        // 이벤트 카드 시스템과 연동될 부분
        Debug.Log("이벤트 카드 1장 드로우 (가상)");
    }

    public void RefreshShop()
    {
        Debug.Log("상점 카드 자동 갱신");
    }

    public void UpdateUI()
    {
        goldText.text = $"Gold: {playerGold}";
        xpText.text = $"XP: {playerXP}";
        levelText.text = $"Lv {playerLevel}";
    }

    public bool IsHandFull()
    {
        return playerHandCards.Count >= maxHandSize;
    }

    public void AddCardToHand(Card card)
    {
        if (IsHandFull()) return;

        playerHandCards.Add(card);
        card.transform.SetParent(playerHandSlots[playerHandCards.Count - 1], false);
    }

    public void MoveCardToField(Card card)
    {
        if (!isPlayerTurn || playerFieldCards.Contains(card)) return;

        playerHandCards.Remove(card);
        playerFieldCards.Add(card);

        card.transform.SetParent(null);
        card.transform.SetParent(FindAvailableFieldSlot(), false);
        card.isPlaced = true;
    }

    private Transform FindAvailableFieldSlot()
    {
        GameObject slot = new GameObject("FieldSlot");
        slot.transform.SetParent(this.transform);
        RectTransform rt = slot.AddComponent<RectTransform>();
        return slot.transform;
    }
}

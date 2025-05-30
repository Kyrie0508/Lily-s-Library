using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] private List<UnitCardSO> allEnemyUnitCardSO;
    [SerializeField] private List<EventCardData> eventCardDeck = new();

    public GameObject cardPrefab;
    public List<Transform> playerHandSlots;
    public List<Transform> playerFieldSlots;
    public List<Transform> enemyFieldSlots;

    public TMP_Text goldText, xpText, levelText, hpText, relicGoldText, enemyHpText, bossHPText;
    public int playerGold = 0;
    public int playerXP = 0;
    public int playerLevel = 1;
    public int playerHP = 30;
    public int relicGold = 0; 
    public StageType currentStageType;
    public int enemyHP = 20;
    public int playerMaxHP = 30; 
    public int bossHP = 50;

    public GameObject restUI;      
    [SerializeField] private GameObject gameClearUI;    
    public bool isBattleOver = false;
    public int turnCount = 1;
    
    public float turnDuration = 30f;
    private float turnTimer;

    private readonly int maxHandSize = 6;
    private readonly int maxFieldSize = 5;

    private bool isPlayerTurn = true;
    private bool isTurnProcessing = false;
    public bool keepShop = false;
    public bool isBossStage = false;

    public List<Card> playerHandCards = new();
    public List<Card> playerFieldCards = new();
    public List<Card> enemyFieldCards = new();
    public List<UnitCardSO> enemyHandCards = new();


    private Dictionary<int, int> xpTable = new()
    {
        {1, 2}, {2, 2}, {3, 6}, {4, 10}, {5, 20}, {6, 36},
        {7, 48}, {8, 76}, {9, 76}
    };

    void Awake() => Instance = this;

    void Start()
    {
        InitializeEventDeck();
        StartCoroutine(PlayerTurnLoop());
    }
    
    void InitializeEventDeck()
    {
        eventCardDeck.Clear();
        eventCardDeck.Add(new EventCardData("Sword", 1));
        eventCardDeck.Add(new EventCardData("Shield", 1));
        eventCardDeck.Add(new EventCardData("Book", 1));
    }
    
    void GameOver()
    {
        Debug.Log("게임 오버!");
        Time.timeScale = 0;
    }


    IEnumerator PlayerTurnLoop()
    {
        while (true)
        {
            yield return StartCoroutine(StartPlayerTurn());
            yield return StartCoroutine(StartEnemyTurn());
        }
    }
    
    public void StartBattle()
    {
        StopAllCoroutines();
        StartCoroutine(PlayerTurnLoop());
    }

    IEnumerator StartPlayerTurn()
    {
        isPlayerTurn = true;
        isTurnProcessing = false;
        turnCount++;
        
        playerGold = Mathf.Min(playerGold + 1, 10);
        playerXP += 2;
        TryLevelUp();
        UpdateUI();

        ApplyDelayedEffects();

        DrawEventCard();
        ShopManager.Instance.RefreshShop();

        turnTimer = turnDuration;
        while (turnTimer > 0f)
        {
            turnTimer -= Time.deltaTime;
            yield return null;
        }

        ApplyTurnEndEffects();

        isPlayerTurn = false;
        isTurnProcessing = true;
    }

    IEnumerator StartEnemyTurn()
    {
        EnemyAIManager.Instance.StartTurn();
        EnemyAIManager.Instance.DeployUnitsToField();
        
        DeployEnemyUnits();
        yield return new WaitForSeconds(1f);

        ApplyBattleStartEffects(); // 이미 있다면 유지
        yield return new WaitForSeconds(1f);

        ResolveCombat();
        yield return new WaitForSeconds(1f);
        ApplyPostCombatResult();

        Debug.Log("적 턴 종료");
    }


    public void MoveCardToField(Card card)
    {
        if (playerHandCards.Contains(card))
            playerHandCards.Remove(card);

        if (playerFieldCards.Count >= maxFieldSize) return;

        playerFieldCards.Add(card);
        card.transform.SetParent(playerFieldSlots[playerFieldCards.Count - 1], false);
        card.isPlaced = true;

        if (card.effectTrigger == CardEffectTrigger.OnSummon)
        {
            ApplySummonEffect(card);
        }
    }

    void ApplySummonEffect(Card card)
    {
        switch (card.effectType)
        {
            case CardEffectType.BuffRandomAlly:
                if (playerFieldCards.Count > 1)
                {
                    List<Card> targets = new(playerFieldCards);
                    targets.Remove(card);
                    Card target = targets[Random.Range(0, targets.Count)];
                    target.attack += 1;
                    target.hp += 1;
                }
                break;
            case CardEffectType.GainGold:
                playerGold += card.effectValue;
                UpdateUI();
                break;
            case CardEffectType.HealPlayer:
                playerHP = Mathf.Min(playerHP + card.effectValue, 20);
                break;
            case CardEffectType.DuplicateSelf:
                for (int i = 0; i < 2; i++)
                {
                    if (playerHandCards.Count >= maxHandSize) break;
                    GameObject copy = Instantiate(cardPrefab);
                    Card c = copy.GetComponent<Card>();
                    c.cardName = card.cardName;
                    c.cost = card.cost;
                    c.attack = card.attack;
                    c.hp = card.hp;
                    c.effectTrigger = card.effectTrigger;
                    c.effectType = card.effectType;
                    c.effectValue = card.effectValue;
                    AddCardToHand(c);
                }
                break;
            case CardEffectType.BuffAllAllies:
                foreach (Card ally in playerFieldCards)
                    if (ally != card)
                    {
                        ally.attack += 1;
                        ally.hp += 1;
                    }
                break;
        }
    }

    void ApplyBattleStartEffects()
    {
        foreach (Card card in playerFieldCards)
        {
            if (card.effectTrigger != CardEffectTrigger.OnBattleStart) continue;

            switch (card.effectType)
            {
                case CardEffectType.DamageRandomEnemy:
                    if (enemyFieldCards.Count > 0)
                    {
                        Card target = enemyFieldCards[Random.Range(0, enemyFieldCards.Count)];
                        target.hp -= card.effectValue;
                    }
                    break;
                case CardEffectType.DamageAllEnemies:
                    foreach (Card enemy in enemyFieldCards)
                        enemy.hp -= card.effectValue;
                    break;
                case CardEffectType.DestroyRandomEnemy:
                    if (enemyFieldCards.Count > 0)
                    {
                        int idx = Random.Range(0, enemyFieldCards.Count);
                        Card target = enemyFieldCards[idx];
                        enemyFieldCards.RemoveAt(idx);
                        Destroy(target.gameObject);
                    }
                    break;
                case CardEffectType.WeakenAllEnemies:
                    foreach (Card enemy in enemyFieldCards)
                        enemy.attack -= card.effectValue;
                    break;
            }
        }
    }

    void ApplyTurnEndEffects()
    {
        foreach (Card card in playerFieldCards)
        {
            if (card.effectTrigger != CardEffectTrigger.OnTurnEnd) continue;

            if (card.effectType == CardEffectType.HealSelf)
            {
                card.hp += card.effectValue;
                Debug.Log($"{card.cardName} → 체력 {card.effectValue} 회복");
            }
        }
    }

    void ApplyDelayedEffects()
    {
        List<Card> executed = new();

        foreach (Card card in playerFieldCards)
        {
            if (card.effectTrigger != CardEffectTrigger.Delayed) continue;

            if (card.effectType == CardEffectType.DelayedGoldGain)
            {
                playerGold += card.effectValue;
                Debug.Log($"{card.cardName} → 다음 턴에 골드 {card.effectValue} 획득");
            }

            executed.Add(card);
        }

        foreach (var card in executed)
        {
            card.effectTrigger = CardEffectTrigger.None;
        }
    }

    public void ApplySplashDamage(Card attacker, int splashDamage)
    {
        int idx = playerFieldCards.IndexOf(attacker);
        if (idx == -1 || idx >= enemyFieldCards.Count) return;

        Card main = enemyFieldCards[idx];
        main.hp -= attacker.attack;
        Debug.Log($"{attacker.cardName} → {main.cardName}에게 {attacker.attack} 피해");

        if (idx > 0)
        {
            Card left = enemyFieldCards[idx - 1];
            left.hp -= splashDamage;
            Debug.Log($"← {left.cardName}에게 스플래시 {splashDamage} 피해");
        }

        if (idx + 1 < enemyFieldCards.Count)
        {
            Card right = enemyFieldCards[idx + 1];
            right.hp -= splashDamage;
            Debug.Log($"→ {right.cardName}에게 스플래시 {splashDamage} 피해");
        }
    }
    
    private void ApplyPostCombatResult()
    {
        int playerRemainingCost = 0;
        int enemyRemainingCost = 0;

        foreach (Card card in playerFieldCards)
            playerRemainingCost += card.cost;
        foreach (Card card in enemyFieldCards)
            enemyRemainingCost += card.cost;

        if (playerRemainingCost > enemyRemainingCost)
        {
            enemyHP -= playerRemainingCost;
            if (enemyHP <= 0)
            {
                enemyHP = 0;
                ClearBattlefield();

                if (isBossStage)
                {
                    GameClear();
                }
                else
                {
                    GrantStageClearRewards();
                }
            }
        }
        else if (enemyRemainingCost > playerRemainingCost)
        {
            playerHP -= enemyRemainingCost;
            if (playerHP <= 0)
            {
                playerHP = 0;
                GameOver();
            }
        }

        UpdateUI();
    }



    
    public void ApplyEventCardEffect(EventCardData card)
    {
        switch (card.type)
        {
            case "Sword":
                foreach (var unit in playerFieldCards)
                    unit.attack += card.value;
                Debug.Log($"Sword 이벤트: 아군 전체 공격력 +{card.value}");
                break;

            case "Shield":
                // 방어도 시스템 없으므로 임시로 HP 회복
                playerHP += card.value;
                Debug.Log($"Shield 이벤트: 플레이어 체력 +{card.value}");
                break;

            case "Star":
                playerGold += card.value;
                Debug.Log($"Star 이벤트: 골드 +{card.value}");
                break;

            case "Book":
                SpawnRandomCardOfTier(card.value);
                Debug.Log($"Book 이벤트: 티어 {card.value} 카드 1장 생성");
                break;
        }

        UpdateUI();
    }
    
    public void SellCard(Card card)
    {
        if (card == null) return;
        if (card.isEventCard) return; 
        
        playerGold += 1;
        UpdateUI();
        if (playerFieldCards.Contains(card))
        {
            playerFieldCards.Remove(card);
        }
        
        ShopManager.Instance.ReturnCardToStock(card.cardData); 
        
        Destroy(card.gameObject);
    }

    
    void SpawnRandomCardOfTier(int tier)
    {
        List<UnitCardSO> pool = ShopManager.Instance.GetCardsOfTier(tier);
        if (pool == null || pool.Count == 0) return;

        UnitCardSO so = pool[Random.Range(0, pool.Count)];
        GameObject obj = Instantiate(cardPrefab);
        Card card = obj.GetComponent<Card>();
        card.cardName = so.cardName;
        card.cost = so.cost;
        card.attack = so.attack;
        card.hp = so.hp;
        card.effectTrigger = so.effectTrigger;
        card.effectType = so.effectType;
        card.effectValue = so.effectValue;
        AddCardToHand(card);
    }

    
    public void OnClick_ReRoll()
    {
        if (playerGold < 1) return;
        playerGold -= 1;
        UpdateUI();
        ShopManager.Instance.RefreshShop();
    }

    public void OnClick_AddXP()
    {
        if (playerGold < 4) return;
        playerGold -= 4;
        playerXP += 4;
        TryLevelUp();
        UpdateUI();
    }

    public void OnClick_Keep()
    {
        keepShop = true;
    }

    public void AddCardToHand(Card card)
    {
        if (playerHandCards.Count >= maxHandSize) return;

        playerHandCards.Add(card);
        card.transform.SetParent(playerHandSlots[playerHandCards.Count - 1], false);
    }
    
    
    void RemoveDeadUnits(List<Card> field)
    {
        for (int i = field.Count - 1; i >= 0; i--)
        {
            if (field[i].hp <= 0)
            {
                Destroy(field[i].gameObject);
                field.RemoveAt(i);
            }
        }
    }

    
    IEnumerator ResolveCombat()
    {
        int playerIdx = 0;
        int enemyIdx = 0;
        bool playerTurn = true;

        while (playerFieldCards.Count > 0 && enemyFieldCards.Count > 0)
        {
            yield return new WaitForSeconds(0.5f);

            if (playerTurn)
            {
                if (playerIdx >= playerFieldCards.Count) playerIdx = 0;
                Card attacker = playerFieldCards[playerIdx];
                if (enemyFieldCards.Count == 0) break;

                int targetIdx = Random.Range(0, enemyFieldCards.Count);
                Card target = enemyFieldCards[targetIdx];

                target.hp -= attacker.attack;
                attacker.hp -= target.attack;

                Debug.Log($"[Player] {attacker.cardName} → {target.cardName} 공격! ({attacker.attack}/{attacker.hp}) vs ({target.attack}/{target.hp})");

                RemoveDeadUnits(enemyFieldCards);
                RemoveDeadUnits(playerFieldCards);
                playerIdx++;
            }
            else
            {
                if (enemyIdx >= enemyFieldCards.Count) enemyIdx = 0;
                Card attacker = enemyFieldCards[enemyIdx];
                if (playerFieldCards.Count == 0) break;

                int targetIdx = Random.Range(0, playerFieldCards.Count);
                Card target = playerFieldCards[targetIdx];

                target.hp -= attacker.attack;
                attacker.hp -= target.attack;

                Debug.Log($"[Enemy] {attacker.cardName} → {target.cardName} 공격! ({attacker.attack}/{attacker.hp}) vs ({target.attack}/{target.hp})");

                RemoveDeadUnits(enemyFieldCards);
                RemoveDeadUnits(playerFieldCards);
                enemyIdx++;
            }

            playerTurn = !playerTurn;
        }
    }
    
    public void ClearBattlefield()
    {
        foreach (var card in playerHandCards)
            Destroy(card.gameObject);
        foreach (var card in playerFieldCards)
            Destroy(card.gameObject);
        foreach (var card in enemyFieldCards)
            Destroy(card.gameObject);

        playerHandCards.Clear();
        playerFieldCards.Clear();
        enemyFieldCards.Clear();
    }

    
    UnitCardSO GetRandomEnemyUnit()
    {
        if (allEnemyUnitCardSO == null || allEnemyUnitCardSO.Count == 0) return null;
        return allEnemyUnitCardSO[Random.Range(0, allEnemyUnitCardSO.Count)];
    }

    
    void DeployEnemyUnits()
    {
        enemyFieldCards.Clear();

        List<UnitCardSO> finalCards = EnemyAIManager.Instance.GetFinalFieldUnits();

        for (int i = 0; i < finalCards.Count && i < enemyFieldSlots.Count; i++)
        {
            UnitCardSO so = finalCards[i];
            GameObject obj = Instantiate(cardPrefab, enemyFieldSlots[i]);
            Card c = obj.GetComponent<Card>();

            c.cardName = so.cardName;
            c.cost = so.cost;
            c.attack = so.attack;
            c.hp = so.hp;
            c.effectTrigger = so.effectTrigger;
            c.effectType = so.effectType;
            c.effectValue = so.effectValue;
            c.isPlaced = true;
            c.cardData = so;

            enemyFieldCards.Add(c); // 이제 Card 타입으로 Add
        }
    }

    
    void GrantStageClearRewards()
    {
        switch (currentStageType)
        {
            case StageType.Normal:
                relicGold += 1;
                Debug.Log("Normal 몬스터 처치 → 골드 +1");
                break;
            case StageType.Elite:
                relicGold += 3;
                Debug.Log("Elite 몬스터 처치 → 골드 +3");
                break;
        }

        UpdateUI();
    }
    public void HealPlayer(int amount)
    {
        playerHP = Mathf.Min(playerMaxHP, playerHP + amount);
        UpdateUI();
    }

    // 유물 스테이지 진입 처리
    public void EnterRelicRoom()
    {
        relicGold += 5;
        StageManager.Instance.MoveToNextStage();
    }

    // 휴식 스테이지 진입 처리
    public void EnterRestRoom()
    {
        restUI.SetActive(true);
        StageManager.Instance.MoveToNextStage();
    }
    
    
    public void GameClear()
    {
        isBattleOver = true;
        StopAllCoroutines();
        if (gameClearUI != null)
            gameClearUI.SetActive(true);
        Time.timeScale = 0f;
    }




    public bool IsHandFull() => playerHandCards.Count >= maxHandSize;
    public bool IsFieldFull() => playerFieldCards.Count >= maxFieldSize;
    public bool IsPlayerTurn() => isPlayerTurn;

    public void TryLevelUp()
    {
        if (playerLevel >= 10) return;
        int neededXP = xpTable.ContainsKey(playerLevel) ? xpTable[playerLevel] : int.MaxValue;
        if (playerXP >= neededXP)
        {
            playerXP -= neededXP;
            playerLevel++;
        }
    }

    public void DrawEventCard()
    {
        if (eventCardDeck.Count == 0 || playerHandCards.Count >= 6) return;

        EventCardData cardData = eventCardDeck[Random.Range(0, eventCardDeck.Count)];
        GameObject obj = Instantiate(cardPrefab);
        Card card = obj.GetComponent<Card>();

        
        card.cardName = $"{cardData.type} {cardData.value}";
        card.cost = 0; 
        card.attack = 0;
        card.hp = 0;
        card.isEventCard = true;
        card.eventCardData = cardData;

        card.effectTrigger = CardEffectTrigger.None;
        card.effectType = CardEffectType.None;
        card.effectValue = 0;

        AddCardToHand(card);
        Debug.Log($"이벤트 카드 드로우: {cardData}");
    }
    
    
    public void UpdateUI()
    {
        goldText.text = $"Gold: {playerGold}";
        xpText.text = $"XP: {playerXP}";
        levelText.text = $"Lv {playerLevel}";
        hpText.text = $"HP: {playerHP}";
        relicGoldText.text = $"Relic: {relicGold}";
        enemyHpText.text = $"Enemy: {enemyHP}";
    }

}

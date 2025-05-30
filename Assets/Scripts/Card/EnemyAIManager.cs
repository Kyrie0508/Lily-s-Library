using System.Collections.Generic;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance;

    public int enemyGold = 0;
    public int enemyXP = 0;
    public int enemyLevel = 1;
    private int maxGoldPerTurn = 1;
    public bool isBoss = false;
    
    private List<UnitCardSO> enemyHand = new();
    private List<UnitCardSO> enemyField = new();
    private List<UnitCardSO> enemyFinalField = new();

    private Dictionary<int, int> xpTable = new()
    {
        { 1, 2 }, { 2, 2 }, { 3, 6 }, { 4, 10 }, { 5, 20 },
        { 6, 36 }, { 7, 48 }, { 8, 76 }, { 9, 76 }
    };
    
    private void Awake()
    {
        Instance = this;
    }
    
    public void StartTurn()
    {
        enemyGold = maxGoldPerTurn;
        enemyXP += 2;
        TryLevelUp();

        EnemyShopManager.Instance.RollShop();
        BuyCardsFromShop();
        maxGoldPerTurn++;
    }
    
    public void StartBossTurn()
    {
        isBoss = true;
        enemyGold = 5;
        enemyXP += 2;
        TryLevelUp();

        EnemyShopManager.Instance.RollShop();
        BuyCardsFromShop();
        DeployUnitsToField();
    }
    
    public int CalculateBossDamageToPlayer()
    {
        int costSum = 0;
        foreach (var card in BattleManager.Instance.enemyFieldCards)
        {
            costSum += card.cost;
        }

        return costSum + BattleManager.Instance.turnCount;
    }


    private void TryLevelUp()
    {
        while (enemyLevel < 10 && enemyXP >= GetXPToNextLevel())
        {
            enemyXP -= GetXPToNextLevel();
            enemyLevel++;
            EnemyShopManager.Instance.shopLevel = enemyLevel;
        }
    }

    private int GetXPToNextLevel()
    {
        return xpTable.TryGetValue(enemyLevel, out var xp) ? xp : 9999;
    }
    
    private void BuyCardsFromShop()
    {
        List<UnitCardSO> shopCards = EnemyShopManager.Instance.GetCurrentShopCards();
        List<UnitCardSO> boughtCards = new();

        // 높은 코스트 순으로 정렬
        shopCards.Sort((a, b) => b.cost.CompareTo(a.cost));

        foreach (var card in shopCards)
        {
            if (enemyGold >= card.cost)
            {
                enemyGold -= card.cost;
                BattleManager.Instance.enemyHandCards.Add(card);
                boughtCards.Add(card);
                EnemyShopManager.Instance.ReturnCardToStock(card);
            }
        }

        // 남은 골드가 있으면 리롤 후 반복
        if (enemyGold > 0 && BattleManager.Instance.enemyHandCards.Count < 5)
        {
            EnemyShopManager.Instance.RollShop();
            BuyCardsFromShop(); // 재귀적 구매 반복
        }
    }

    public List<UnitCardSO> GetEnemyHand() => BattleManager.Instance.enemyHandCards;
    public void TryEnemyLevelUp()
    {
        while (enemyLevel < 10 && enemyXP >= GetXpToNextLevel())
        {
            enemyXP -= GetXpToNextLevel();
            enemyLevel++;
            EnemyShopManager.Instance.shopLevel = enemyLevel;
        }
    }

    private int GetXpToNextLevel()
    {
        return xpTable.ContainsKey(enemyLevel) ? xpTable[enemyLevel] : 9999;
    }

    public void GainXP(int amount)
    {
        enemyXP += amount;
        TryEnemyLevelUp();
    }

    public void SpendGold(int amount)
    {
        enemyGold -= amount;
        if (enemyGold < 0) enemyGold = 0;
    }

    public int GetLevel() => enemyLevel;
    
    public void DeployUnitsToField()
    {
        enemyFinalField.Clear();

        List<UnitCardSO> hand = BattleManager.Instance.enemyHandCards;
        hand.Sort((a, b) => b.cost.CompareTo(a.cost));

        foreach (var card in hand)
        {
            if (enemyFinalField.Count < 5)
            {
                enemyFinalField.Add(card);
            }
            else
            {
                SellCard(card);
            }
        }
    }

    public List<UnitCardSO> GetFinalFieldUnits()
    {
        return enemyFinalField;
    }
    private void SellCard(UnitCardSO card)
    {
        enemyGold += 1;
        EnemyShopManager.Instance.ReturnCardToStock(card);
    }

}

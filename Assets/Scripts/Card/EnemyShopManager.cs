using System.Collections.Generic;
using UnityEngine;

public class EnemyShopManager : MonoBehaviour
{
    public static EnemyShopManager Instance;

    [SerializeField] private List<UnitCardSO> allCards;
    [SerializeField] private List<ShopCardStock> allCardStocks;
    [SerializeField] public int shopLevel = 1;

    private List<UnitCardSO> currentShopCards = new();

    private void Awake()
    {
        Instance = this;
    }

    public void InitShop()
    {
        shopLevel = 1;
        currentShopCards.Clear();
        RollShop();
    }

    public void RollShop()
    {
        currentShopCards.Clear();

        float[] chances = GetTierChances(shopLevel);
        for (int i = 0; i < 5; i++)
        {
            int tier = GetRandomTier(chances);
            var tierCards = GetCardsOfTier(tier);
            if (tierCards.Count == 0) continue;

            int index = Random.Range(0, tierCards.Count);
            currentShopCards.Add(tierCards[index]);
        }
    }

    public List<UnitCardSO> GetCurrentShopCards()
    {
        return currentShopCards;
    }

    public List<UnitCardSO> GetCardsOfTier(int tier)
    {
        List<UnitCardSO> result = new();
        foreach (var card in allCards)
        {
            if (card.cost == tier)
                result.Add(card);
        }
        return result;
    }

    public int GetRandomTier(float[] chances)
    {
        float roll = Random.value;
        float cumulative = 0f;

        for (int i = 0; i < chances.Length; i++)
        {
            cumulative += chances[i];
            if (roll < cumulative)
                return i + 1;
        }
        return 1;
    }


    private float[] GetTierChances(int level)
    {
        if (ShopManager.Instance.tierChances.TryGetValue(level, out var chances))
            return chances;
        return new float[] { 1f, 0f, 0f, 0f, 0f };
    }


    public void ReturnCardToStock(UnitCardSO cardData)
    {
        foreach (var stock in allCardStocks)
        {
            if (stock.card == cardData)
            {
                stock.remaining++;
                break;
            }
        }
    }
}

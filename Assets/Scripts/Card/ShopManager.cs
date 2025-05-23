using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    [SerializeField] private Transform[] shopSlots; // 상점 슬롯 5개
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<ShopCardStock> allCardStocks;
    [SerializeField] private List<UnitCardSO> allCards;


    private Dictionary<int, float[]> tierChances = new()
    {
        { 1, new float[] { 1f, 0f, 0f, 0f, 0f } },
        { 2, new float[] { 1f, 0f, 0f, 0f, 0f } },
        { 3, new float[] { 0.75f, 0.25f, 0f, 0f, 0f } },
        { 4, new float[] { 0.55f, 0.3f, 0.15f, 0f, 0f } },
        { 5, new float[] { 0.45f, 0.33f, 0.2f, 0.02f, 0f } },
        { 6, new float[] { 0.3f, 0.4f, 0.25f, 0.05f, 0f } },
        { 7, new float[] { 0.19f, 0.35f, 0.35f, 0.1f, 0.01f } },
        { 8, new float[] { 0.18f, 0.25f, 0.36f, 0.18f, 0.03f } },
        { 9, new float[] { 0.1f, 0.2f, 0.25f, 0.35f, 0.1f } },
        { 10, new float[] { 0.05f, 0.1f, 0.2f, 0.4f, 0.25f } }
    };

    public void RefreshShop()
    {
        ClearShopSlots();

        for (int i = 0; i < shopSlots.Length; i++)
        {
            UnitCardSO selectedCardSO = GetRandomCardBasedOnLevel();
            if (selectedCardSO == null) continue;

            GameObject cardObj = Instantiate(cardPrefab, shopSlots[i]);
            Card card = cardObj.GetComponent<Card>();
            card.cardName = selectedCardSO.cardName;
            card.cost = selectedCardSO.cost;
            card.attack = selectedCardSO.attack;
            card.hp = selectedCardSO.hp;
            card.effectTrigger = selectedCardSO.effectTrigger;
            card.effectType = selectedCardSO.effectType;
            card.effectValue = selectedCardSO.effectValue;
            CardInShop cardShop = cardObj.AddComponent<CardInShop>();
            cardShop.cardData = selectedCardSO;
        }
    }

    private UnitCardSO GetRandomCardBasedOnLevel()
    {
        int level = BattleManager.Instance != null ? BattleManager.Instance.playerLevel : 1;
        float[] chances = tierChances[Mathf.Clamp(level, 1, 10)];
        float roll = Random.value;

        int chosenTier = -1;
        float cumulative = 0f;
        for (int i = 0; i < chances.Length; i++)
        {
            cumulative += chances[i];
            if (roll <= cumulative)
            {
                chosenTier = i + 1;
                break;
            }
        }

        var candidates = allCardStocks
            .FindAll(cs => cs.card.tier == chosenTier && cs.remaining > 0);

        if (candidates.Count == 0) return null;

        var chosen = candidates[Random.Range(0, candidates.Count)];
        chosen.remaining--;
        return chosen.card;
    }

    private void ClearShopSlots()
    {
        foreach (Transform slot in shopSlots)
        {
            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    public List<UnitCardSO> GetCardsOfTier(int tier)
    {
        List<UnitCardSO> result = new();

        foreach (var card in allCards)
        {
            if (card.cost == tier)
            {
                result.Add(card);
            }
        }

        return result;
    }

}

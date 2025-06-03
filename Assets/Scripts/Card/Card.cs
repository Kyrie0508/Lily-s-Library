using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CardEffectTrigger
{
    None,
    OnSummon,
    OnBattleStart,
    OnTurnEnd,
    OnAttack,
    Delayed
}

public enum CardEffectType
{
    None,
    BuffRandomAlly,
    GainGold,
    HealPlayer,
    DuplicateSelf,
    BuffAllAllies,
    DamageRandomEnemy,
    DamageAllEnemies,
    DestroyRandomEnemy,
    WeakenAllEnemies,
    HealSelf,
    AttackSplashDamage,
    DelayedGoldGain
}

public class Card : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text costText;
    public TMP_Text nameText;
    public TMP_Text atkText;
    public TMP_Text hpText;
    public Image cardImage;
    
    public string cardName;
    public int cost;
    public int attack;
    public int hp;
    public int originalHp;
    public CardEffectTrigger effectTrigger;
    public CardEffectType effectType;
    public int effectValue;

    public bool isPlaced = false;
    public bool isEventCard = false;
    public EventCardData eventCardData; 
    public UnitCardSO cardData; 
    public void SetCardData(UnitCardSO data)
    {
        cardData = data;

        cardName = data.cardName;
        cost = data.cost;
        attack = data.attack;
        hp = data.hp;
        effectTrigger = data.effectTrigger;
        effectType = data.effectType;
        effectValue = data.effectValue;
        originalHp = data.hp;
        
        cardImage.GetComponent<Image>().sprite = data.art;
        nameText.text = cardName;
        costText.text = cost.ToString();
        atkText.text = attack.ToString();
        hpText.text = hp.ToString();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (BattleManager.Instance.playerFieldCards.Contains(this))
            {
                BattleManager.Instance.SellCard(this);
            }
            return;
        }
        
        if (!isPlaced 
            && BattleManager.Instance.IsPlayerTurn() 
            && !BattleManager.Instance.IsFieldFull() 
            && BattleManager.Instance.playerHandCards.Contains(this))
        {
            BattleManager.Instance.MoveCardToField(this);
        }
        
        if (isEventCard && BattleManager.Instance.IsPlayerTurn())
        {
            BattleManager.Instance.ApplyEventCardEffect(eventCardData);
            BattleManager.Instance.playerHandCards.Remove(this);
            Destroy(gameObject);
        }

    }

    public void TriggerOnAttack()
    {
        if (effectTrigger != CardEffectTrigger.OnAttack) return;

        if (effectType == CardEffectType.AttackSplashDamage)
        {
            BattleManager.Instance.ApplySplashDamage(this, effectValue);
        }
    }
    

}
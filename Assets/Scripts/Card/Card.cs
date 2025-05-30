using UnityEngine;
using UnityEngine.EventSystems;

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
    public string cardName;
    public int cost;
    public int attack;
    public int hp;

    public CardEffectTrigger effectTrigger;
    public CardEffectType effectType;
    public int effectValue;

    public bool isPlaced = false;
    public bool isEventCard = false;
    public EventCardData eventCardData; 
    public UnitCardSO cardData; 

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
        
        if (!isPlaced && BattleManager.Instance.IsPlayerTurn() && !BattleManager.Instance.IsFieldFull())
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
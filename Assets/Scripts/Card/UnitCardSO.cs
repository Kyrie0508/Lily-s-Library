using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitCard", menuName = "Cards/UnitCard")]
[System.Serializable]
public class UnitCardSO : ScriptableObject
{
    public string cardName;
    public int cost;
    public int attack;
    public int hp;
    public Sprite art;
    public CardEffectTrigger effectTrigger;
    public CardEffectType effectType;
    public int effectValue;
    public int tier; // 1~5
}

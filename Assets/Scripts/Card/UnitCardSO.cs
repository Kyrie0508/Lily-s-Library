using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitCard", menuName = "Cards/UnitCard")]
public class UnitCardSO : ScriptableObject
{
    public string cardName;
    public Sprite fullImage; // 일러스트+프레임 통합 이미지
    public int cost;
    public int attack;
    public int hp;
}
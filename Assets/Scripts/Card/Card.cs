using UnityEngine;

public enum CardType
{
    Sword,
    Shield,
    Magic
    //추후 추가 예정
}

public class Card : MonoBehaviour
{
    public CardType cardType;
    public int power;
}
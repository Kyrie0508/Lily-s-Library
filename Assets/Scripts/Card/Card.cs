using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public enum CardLocation { Hand, Field }

public class Card : MonoBehaviour, IPointerClickHandler
{
    public CardType cardType;
    public int power;

    public TextMeshProUGUI powerText;
    public int handIndex = -1;
    public CardLocation currentLocation = CardLocation.Hand;

    public void UpdateUI()
    {
        if (powerText != null)
            powerText.text = power.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BattleManager bm = FindAnyObjectByType<BattleManager>();
        if (bm == null) return;

        if (currentLocation == CardLocation.Hand)
        {
            bm.MoveCardToField(this);
        }
        else if (currentLocation == CardLocation.Field)
        {
            bm.ReturnCardToHand(this);
        }
    }
}
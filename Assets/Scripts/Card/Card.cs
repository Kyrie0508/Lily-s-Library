using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public enum CardLocation { Hand, Field }

public class Card : MonoBehaviour, IPointerClickHandler
{
    private bool interactable = true;
    public CardType cardType;
    public int power;

    public TextMeshProUGUI powerText;
    public int handIndex = -1;
    public CardLocation currentLocation = CardLocation.Hand;
    
    public void SetData(CardType type, int power)
    {
        this.cardType = type;
        this.power = power;
        UpdateUI();
    }


    public void UpdateUI()
    {
        if (powerText != null)
            powerText.text = power.ToString();
    }
    
    public void SetInteractable(bool canClick)
    {
        this.interactable = canClick;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        BattleManager bm = FindAnyObjectByType<BattleManager>();
        if (bm == null) return;

        if (!interactable || bm == null || !bm.IsPlayerTurn()) return;  // 턴이 아닐 때는 무시

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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public UnitCardSO cardData;

    public TMP_Text costText;
    public TMP_Text nameText;
    public TMP_Text atkText;
    public TMP_Text hpText;
    public Image backgroundImage;

    public void Init(UnitCardSO data)
    {
        cardData = data;
        costText.text = data.cost.ToString();
        nameText.text = data.cardName;
        atkText.text = data.attack.ToString();
        hpText.text = data.hp.ToString();
        backgroundImage.sprite = data.fullImage;
    }

    public bool isPlaced = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPlaced)
        {
            if (BattleManager.Instance.IsHandFull()) return;

            BattleManager.Instance.MoveCardToField(this);
        }
    }
}
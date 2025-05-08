using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public CardType cardType;
    public int power;

    public TextMeshProUGUI powerText;
    private bool isUsed = false;

    public void UpdateUI()
    {
        if (powerText != null)
            powerText.text = power.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUsed) return;

        BattleManager bm = FindAnyObjectByType<BattleManager>();
        if (bm == null || !bm.IsPlayerCard(this)) return;

        transform.SetParent(bm.playerFieldTransform, false);
        bm.RegisterUsedCard(gameObject);
        isUsed = true;
    }
}
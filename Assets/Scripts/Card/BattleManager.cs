using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("카드 관련")]
    public GameObject cardPrefab;
    public Transform playerHandArea;
    public Transform fieldArea;

    [Header("턴 관련")]
    public Button endTurnButton;
    public int maxHandSize = 5;
    public int drawPerTurn = 1;

    [Header("스킬 컷씬 관련")]
    public SkillCutinManager skillCutinManager;

    private List<GameObject> playerHand = new List<GameObject>();
    private List<GameObject> usedCards = new List<GameObject>(); // 드랍된 카드 저장
    private int swordPowerSum = 0;
    private int turnCount = 1;

    void Start()
    {
        SetupBattle();
    }

    void SetupBattle()
    {
        Debug.Log("배틀 시작");

        endTurnButton.onClick.AddListener(EndTurn);

        for (int i = 0; i < maxHandSize; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        if (playerHand.Count >= maxHandSize)
        {
            Debug.Log("손패가 가득 찼습니다!");
            return;
        }

        GameObject cardObj = Instantiate(cardPrefab, playerHandArea);
        Card card = cardObj.GetComponent<Card>();

        if (card != null)
        {
            card.cardType = (CardType)Random.Range(0, System.Enum.GetValues(typeof(CardType)).Length);
            card.power = Random.Range(1, 4);
        }

        playerHand.Add(cardObj);
    }

    public void RegisterUsedCard(GameObject cardObj)
    {
        if (!usedCards.Contains(cardObj))
        {
            usedCards.Add(cardObj);
        }
    }

    public void EndTurn()
    {
        Debug.Log($"턴 종료: {turnCount}");

        swordPowerSum = 0;

        foreach (GameObject cardObj in usedCards)
        {
            Card card = cardObj.GetComponent<Card>();
            if (card != null && card.cardType == CardType.Sword)
            {
                swordPowerSum += card.power;
            }
        }

        Debug.Log($"이번 턴 누적 칼 수치: {swordPowerSum}");

        CheckSkillActivation();

        // 사용한 카드 삭제
        foreach (GameObject cardObj in usedCards)
        {
            Destroy(cardObj);
        }
        usedCards.Clear();

        // 다음 턴 시작
        turnCount++;
        Debug.Log($"턴 시작: {turnCount}");

        for (int i = 0; i < drawPerTurn; i++)
        {
            DrawCard();
        }
    }

    void CheckSkillActivation()
    {
        if (swordPowerSum >= 5)
        {
            Debug.Log("스킬 발동!");
            if (skillCutinManager != null)
            {
                skillCutinManager.ShowSkillCutin();
            }
        }
        else
        {
            Debug.Log("스킬 발동 실패");
        }
    }
}

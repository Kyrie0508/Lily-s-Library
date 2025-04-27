using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("카드 관련")]
    public GameObject cardPrefab; // 생성할 카드 프리팹
    public Transform playerHandArea; // 손패가 놓이는 곳
    public Transform fieldArea; // 카드가 사용될 필드

    [Header("턴 관련")]
    public Button endTurnButton;
    public int maxHandSize = 5;
    public int drawPerTurn = 1;

    [Header("스킬 컷씬 관련")]
    public SkillCutinManager skillCutinManager; // 스킬 컷씬 매니저 연결

    private List<GameObject> playerHand = new List<GameObject>();
    private List<Card> usedCardsThisTurn = new List<Card>();

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
            // 랜덤 타입 지정
            card.cardType = (CardType)Random.Range(0, System.Enum.GetValues(typeof(CardType)).Length);
            // 랜덤 파워 지정 (1~3)
            card.power = Random.Range(1, 4);

            // 카드 위에 표시하고 싶으면 Text 설정 가능
            // 예) cardObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{card.cardType} {card.power}";
        }

        playerHand.Add(cardObj);
    }

    public void EndTurn()
    {
        Debug.Log("턴 종료: " + turnCount);

        CheckSkillActivation();

        // 다음 턴 시작
        turnCount++;
        Debug.Log("턴 시작: " + turnCount);

        // 턴이 끝나면 사용한 카드 초기화
        usedCardsThisTurn.Clear();

        // 카드 드로우
        for (int i = 0; i < drawPerTurn; i++)
        {
            DrawCard();
        }
    }

    void CheckSkillActivation()
    {
        int swordPower = 0;

        foreach (Card card in usedCardsThisTurn)
        {
            if (card.cardType == CardType.Sword)
            {
                swordPower += card.power;
            }
        }

        if (swordPower >= 5)
        {
            Debug.Log($"스킬 발동! (칼 수치 합계: {swordPower})");

            // 스킬 컷씬 연출
            if (skillCutinManager != null)
            {
                skillCutinManager.ShowSkillCutin();
            }
        }
        else
        {
            Debug.Log($"스킬 조건 미달 (칼 수치 합계: {swordPower})");
        }
    }

    public void RegisterUsedCard(Card card)
    {
        usedCardsThisTurn.Add(card);
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    public GameObject rewardPanel;
    public Transform cardSlotGroup;
    public GameObject cardPreviewPrefab; // 이벤트 카드 프리뷰 프리팹
    public List<EventCardData> rewardOptions = new();

    private void Awake() => Instance = this;

    public void ShowRewards()
    {
        rewardPanel.SetActive(true);
        GenerateRewardOptions();
        DisplayOptions();
    }

    void GenerateRewardOptions()
    {
        rewardOptions.Clear();
        for (int i = 0; i < 3; i++)
        {
            rewardOptions.Add(EventCardGenerator.GetRandomRewardCard());
        }
    }

    void DisplayOptions()
    {
        foreach (Transform child in cardSlotGroup)
        {
            Destroy(child.gameObject);
        }

        foreach (var card in rewardOptions)
        {
            GameObject preview = Instantiate(cardPreviewPrefab, cardSlotGroup);
            TMP_Text[] texts = preview.GetComponentsInChildren<TMP_Text>();
            texts[0].text = card.type;
            texts[1].text = card.value.ToString();

            var button = preview.GetComponent<UnityEngine.UI.Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectCard(card));
        }
    }

    public void SelectCard(EventCardData selected)
    {
        EventCardManager.Instance.AddToDeck(selected);
        rewardPanel.SetActive(false);
        Debug.Log($"선택한 카드: {selected}");
    }
}
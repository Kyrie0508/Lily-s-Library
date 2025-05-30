using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestUIManager : MonoBehaviour
{
    public static RestUIManager Instance;

    [SerializeField] private GameObject restOptionPanel;
    [SerializeField] private Button healButton;
    [SerializeField] private Button upgradeButton;

    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform upgradeCardParent;
    [SerializeField] private GameObject cardButtonPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenRestOptions()
    {
        restOptionPanel.SetActive(true);

        healButton.onClick.RemoveAllListeners();
        healButton.onClick.AddListener(() => {
            RestManager.Instance.HealPlayer();
        });

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => {
            ShowUpgradeOptions(EventCardManager.Instance.playerEventDeck);
        });
    }

    public void ShowUpgradeOptions(List<EventCardData> cards)
    {
        restOptionPanel.SetActive(false);
        upgradePanel.SetActive(true);

        foreach (Transform child in upgradeCardParent)
            Destroy(child.gameObject);

        foreach (var card in cards)
        {
            if (card.type == "Book") continue; // 강화 불가

            GameObject obj = Instantiate(cardButtonPrefab, upgradeCardParent);
            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();
            txt.text = $"{card.type} {card.value}";

            obj.GetComponent<Button>().onClick.AddListener(() => {
                RestManager.Instance.UpgradeSelectedCard(card);
            });
        }
    }

    public void CloseAll()
    {
        restOptionPanel.SetActive(false);
        upgradePanel.SetActive(false);

        foreach (Transform child in upgradeCardParent)
            Destroy(child.gameObject);
    }
}
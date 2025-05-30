using UnityEngine;

public class RestManager : MonoBehaviour
{
    public static RestManager Instance;

    [SerializeField] private RestUIManager restUIManager;

    private void Awake()
    {
        Instance = this;
    }

    public void StartRest()
    {
        restUIManager.OpenRestOptions();
    }

    public void HealPlayer()
    {
        BattleManager.Instance.HealPlayer(8);
        restUIManager.CloseAll();
        StageManager.Instance.MoveToNextStage();
    }

    public void UpgradeSelectedCard(EventCardData card)
    {
        if (card == null) return;
        if (BattleManager.Instance.relicGold < 3)
        {
            // 강화 실패 처리: 부족한 경우 UI 표시만 하고 리턴
            // 예: NotificationPanel.Instance.Show("강화에 필요한 유물 골드가 부족합니다.");
            return;
        }
        
        BattleManager.Instance.relicGold -= 3;
        BattleManager.Instance.UpdateUI();

        EventCardManager.Instance.UpgradeCard(card);
        restUIManager.CloseAll();
        StageManager.Instance.MoveToNextStage();
    }
}
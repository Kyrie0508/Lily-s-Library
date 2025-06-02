using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public List<StageData> stages = new();
    public int currentStageIndex = 0; 

    void Awake()
    {
        Instance = this;
        FindAnyObjectByType<FadeInController>().StartFadeIn();
        GenerateStages();
    }

    void GenerateStages()
    {
        stages.Clear();

        // 고정된 스테이지
        stages.Add(new StageData(0, StageType.Normal));   // 1번
        for (int i = 1; i < 8; i++) // 2~8
            stages.Add(new StageData(i, GetRandomStageType()));
        stages.Add(new StageData(8, StageType.Relic));    // 9번
        for (int i = 9; i < 14; i++) // 10~14
            stages.Add(new StageData(i, GetRandomStageType()));
        stages.Add(new StageData(14, StageType.Rest));    // 15번
        stages.Add(new StageData(15, StageType.Boss));    // 16번
    }

    StageType GetRandomStageType()
    {
        float rand = Random.value;
        if (rand < 0.45f) return StageType.Normal;
        if (rand < 0.65f) return StageType.Elite;
        if (rand < 0.85f) return StageType.Relic;
        return StageType.Rest;
    }
    
    public void EnterStage(StageData stageData)
    {
        Debug.Log($"[Stage {stageData.index + 1}] 타입: {stageData.stageType}");

        switch (stageData.stageType)
        {
            case StageType.Normal:
            case StageType.Elite:
                StartCombat(stageData.stageType);
                break;
            case StageType.Relic:
                GrantRelicReward();
                break;
            case StageType.Rest:
                ShowRestUI();
                break;
            case StageType.Boss:
                StartBossCombat();
                break;
        }
        
        void StartCombat(StageType type)
        {
            BattleManager.Instance.currentStageType = type;
            BattleManager.Instance.StartBattle();
        }


        void GrantRelicReward()
        {
            BattleManager.Instance.relicGold += 5;
            BattleManager.Instance.UpdateUI();
            Debug.Log("Relic 스테이지 진입 → 강화 골드 +5");
        }

        void ShowRestUI()
        {
            Debug.Log("휴식 장소 진입: 회복 or 강화 선택 UI 표시");
            // RestPopupController.Instance.Show(); 등으로 UI 표시 예정
        }

        void StartBossCombat()
        {
            Debug.Log("보스 전투 시작!");
            // 보스 전용 카드 세팅 필요
            BattleManager.Instance.StartBattle();
        }

    }
    
    public class StageButton : MonoBehaviour
    {
        public int stageIndex;

        public void OnClickEnterStage()
        {
            var stageData = StageManager.Instance.stages[stageIndex];
            StageManager.Instance.EnterStage(stageData);
        }
    }
    public void MoveToNextStage()
    {
        currentStageIndex++;

        if (currentStageIndex >= stages.Count)
        {
            BattleManager.Instance.GameClear();
            return;
        }

        StageData nextStage = stages[currentStageIndex];
        BattleManager.Instance.currentStageType = nextStage.stageType;

        switch (nextStage.stageType)
        {
            case StageType.Normal:
            case StageType.Elite:
                BattleManager.Instance.isBossStage = false;
                EnemyAIManager.Instance.StartTurn();
                break;
            case StageType.Relic:
                BattleManager.Instance.EnterRelicRoom();
                break;
            case StageType.Rest:
                BattleManager.Instance.EnterRestRoom();
                break;
            case StageType.Boss:
                BattleManager.Instance.isBossStage = true;
                EnemyAIManager.Instance.StartBossTurn();
                break;
        }
    }



}
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public List<StageData> stages = new();
    public static int currentStageIndex = 0; 
    [SerializeField] private Transform stageNodesParent;
    [SerializeField] private Sprite normalSprite, eliteSprite, relicSprite, restSprite, bossSprite;

    void Awake()
    {
        Instance = this;
        FindAnyObjectByType<FadeInController>().StartFadeIn();
        GenerateStages();
    }

    private void Start()
    {
        DrawStageMap();
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
    
    public void DrawStageMap()
    {
        for (int i = 0; i < stages.Count; i++)
        {
            int reverseIndex = stages.Count - 1 - i;
            Transform slot = stageNodesParent.GetChild(reverseIndex);

            StageNodeUI ui = slot.GetComponent<StageNodeUI>();
            if (ui != null)
                ui.Setup(stages[i], normalSprite, eliteSprite, relicSprite, restSprite, bossSprite);
        }

    }
    
    public void EnterStage(StageData stageData)
    {
        Debug.Log($"[Stage {stageData.index + 1}] 타입: {stageData.stageType}");
        currentStageIndex++;
        switch (stageData.stageType)
        {
            case StageType.Normal:
                FindAnyObjectByType<FadeOutController>().StartFadeOut("Normal");
                break;
            case StageType.Elite:
                FindAnyObjectByType<FadeOutController>().StartFadeOut("Elite");
                break;
            case StageType.Relic:
                FindAnyObjectByType<FadeOutController>().StartFadeOut("Relic");
                break;
            case StageType.Rest:
                FindAnyObjectByType<FadeOutController>().StartFadeOut("Rest");
                break;
            case StageType.Boss:
                FindAnyObjectByType<FadeOutController>().StartFadeOut("Boss");
                break;
        }
        
    }


}
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public List<StageData> stages = new();

    void Awake()
    {
        Instance = this;
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
}
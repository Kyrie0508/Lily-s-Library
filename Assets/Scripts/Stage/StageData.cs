[System.Serializable]
public class StageData
{
    public int index; // 0 ~ 15
    public StageType stageType;

    public StageData(int index, StageType type)
    {
        this.index = index;
        this.stageType = type;
    }
}
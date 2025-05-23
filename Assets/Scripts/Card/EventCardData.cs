[System.Serializable]
public class EventCardData
{
    public string type; 
    public int value;

    public EventCardData(string type, int value)
    {
        this.type = type;
        this.value = value;
    }

    public override string ToString() => $"{type} {value}";
}
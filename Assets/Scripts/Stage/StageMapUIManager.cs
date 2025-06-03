using TMPro;
using UnityEngine;

public class StageMapUIManager : MonoBehaviour
{
    public static StageMapUIManager Instance;

    [SerializeField] private TMP_Text relicText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateRelicText();
    }

    public void UpdateRelicText()
    {
        if (relicText != null)
        {
            relicText.text = $"유물 코인: {PlayerData.relicGold}";
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageNodeUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    private StageData myData;

    public void Setup(StageData data, Sprite normal, Sprite elite, Sprite relic, Sprite rest, Sprite boss)
    {
        myData = data;
        switch (data.stageType)
        {
            case StageType.Normal: iconImage.sprite = normal; break;
            case StageType.Elite: iconImage.sprite = elite; break;
            case StageType.Relic: iconImage.sprite = relic; break;
            case StageType.Rest: iconImage.sprite = rest; break;
            case StageType.Boss: iconImage.sprite = boss; break;
        }
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => StageManager.Instance.EnterStage(data));
    }
}
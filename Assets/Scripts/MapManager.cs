using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public void OnClickRegion(string regionName)
    {
        PlayerPrefs.SetString("SelectedRegion", regionName);
        FindAnyObjectByType<FadeOutController>().StartFadeOut("BattleScene"); //실제 전투 씬 이름으로 교체
    }
}
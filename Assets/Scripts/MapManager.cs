using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public void OnClickRegion(string regionName)
    {
        PlayerPrefs.SetString("SelectedRegion", regionName);
        FindAnyObjectByType<FadeOutController>().StartFadeOut("BattleScene");
    }
}
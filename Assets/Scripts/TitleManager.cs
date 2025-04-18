using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void OnClickStart()
    {
        FindAnyObjectByType<FadeOutController>().StartFadeOut("Library");
    }

    public void OnClickSettings()
    {
        Debug.Log("설정 창 열기 (추후 구현)");
    }

    public void OnClickRanking()
    {
        Debug.Log("랭킹 시스템 호출 (추후 구현)");
    }

    public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }
}
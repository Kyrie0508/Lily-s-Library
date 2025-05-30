using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearUIManager : MonoBehaviour
{
    // 메인 화면으로 이동 (씬 이름은 실제 메인 씬 이름으로 수정 필요)
    public void OnClickGoToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title"); // 예: "MainMenu" 또는 "TitleScene"
    }

    // 게임 종료 처리
    public void OnClickQuit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
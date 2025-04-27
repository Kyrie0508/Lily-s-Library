using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillCutinManager : MonoBehaviour
{
    public GameObject cutinPanel;
    public Image cutinImage;
    public Sprite skillSprite;
    public float fadeDuration = 0.5f; // 페이드 인/아웃 시간
    public float displayTime = 2.0f; // 컷씬이 보이는 시간

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (cutinPanel != null)
        {
            canvasGroup = cutinPanel.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                // 없으면 자동 추가
                canvasGroup = cutinPanel.AddComponent<CanvasGroup>();
            }

            cutinPanel.SetActive(false);
        }
    }

    public void ShowSkillCutin()
    {
        if (cutinPanel != null && cutinImage != null)
        {
            cutinImage.sprite = skillSprite;
            StartCoroutine(ShowCutinCoroutine());
        }
    }

    private IEnumerator ShowCutinCoroutine()
    {
        cutinPanel.SetActive(true);
        canvasGroup.alpha = 0f;

        // Fade In
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = timer / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Cut-in 표시 유지
        yield return new WaitForSeconds(displayTime);

        // Fade Out
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1f - (timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        cutinPanel.SetActive(false);
    }
}
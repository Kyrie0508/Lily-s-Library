using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeOutController : MonoBehaviour
{
    public Image fadeImage;              // FadePanel의 Image
    public float fadeDuration = 1f;
    public static bool isFading = false;

    private void Awake()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;

            fadeImage.raycastTarget = false; // 처음엔 입력 차단 X
        }
    }
    

    public void StartFadeOut(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }



    IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;

        float t = 0f;
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = t / fadeDuration;
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
        SceneManager.LoadScene(sceneName);
    }
}
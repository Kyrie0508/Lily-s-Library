using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public GameObject textPanel;    
    public GameObject characterImage;
    public GameObject characterImage_;
    public RectTransform characterImageRect;
    public RectTransform characterImageRect_;
    public TextMeshProUGUI Text;
    public TextMeshProUGUI Text_;
    public string[] dialogueLines;
    public string[] dialogueLines_;
    public float typingSpeed = 0.05f;

    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    void Start()
    {
        textPanel.SetActive(true);
        JumpAnimation();
        StartTyping();
    }
    
    void Update()
    {
        if (FadeInController.isFading)
            return;
        
        // Z 키 또는 마우스 클릭 시 동작
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                NextLine();
            }
        }
    }

    void StartTyping()
    {
        typingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        Text.text = "";

        foreach (char c in dialogueLines[currentLine])
        {
            Text.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        Text.text = dialogueLines[currentLine];
        isTyping = false;
    }

    void NextLine()
    {
        if (currentLine < dialogueLines.Length - 1)
        {
            currentLine++;
            StartTyping();
        }
        else
        {
            FindAnyObjectByType<FadeOutController>().StartFadeOut("StageMap");
            textPanel.SetActive(false); 
            characterImage.SetActive(false);
        }
    }
    
    IEnumerator JumpAnimation()
    {
        Vector2 originalPos = characterImageRect.anchoredPosition;

        for (int i = 0; i < 3; i++)
        {
            yield return characterImageRect.DOAnchorPos(originalPos + new Vector2(0, 30), 0.1f)
                .SetEase(Ease.OutQuad)
                .WaitForCompletion();

            yield return characterImageRect.DOAnchorPos(originalPos, 0.1f)
                .SetEase(Ease.InQuad)
                .WaitForCompletion();
        }
    }
    
}

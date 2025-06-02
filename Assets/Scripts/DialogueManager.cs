using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;  

public class DialogueManager : MonoBehaviour
{
    public GameObject textPanel;    
    public GameObject characterImage;
    public Sprite angrySprite;
    public RectTransform characterImageRect;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Text;
    public string[] dialogueLines;
    public float typingSpeed = 0.05f;
    private Scene scene;
    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        scene = SceneManager.GetActiveScene();
    }

    void Start()
    {
        textPanel.SetActive(true);
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
            if (currentLine == 3)
            {
                characterImage.GetComponent<Image>().sprite = angrySprite;
                StartCoroutine(JumpAnimation());
            }
            if (currentLine == 4)
            {
                StartCoroutine(JumpAnimation());
            }
            StartTyping();
        }
        else
        {
            if (scene.name == "Library")
            {
                FindAnyObjectByType<FadeOutController>().StartFadeOut("StageMap");
            }
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
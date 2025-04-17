using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject textPanel;                // ← TextPanel 전체를 숨기기 위해 필요
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Text;
    public string[] dialogueLines;
    public float typingSpeed = 0.05f;

    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        textPanel.SetActive(true);
        StartTyping();
    }

    void Update()
    {
        // Z 키 또는 마우스 클릭 시 동작
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                SkipTyping(); // 타이핑 중이면 전체 출력
            }
            else
            {
                NextLine();   // 다음 대사로 진행
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
            textPanel.SetActive(false); // 대사 끝 → 패널 숨김
        }
    }
}
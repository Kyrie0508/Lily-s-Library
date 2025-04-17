using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Text;
    public string[] dialogueLines;
    public float typingSpeed = 0.05f;

    private int currentLine = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (dialogueLines.Length > 0)
        {
            typingCoroutine = StartCoroutine(TypeLine());
        }
    }

    IEnumerator TypeLine()
    {
        Debug.Log("타이핑 시작");
        Text.text = "";
        foreach (char c in dialogueLines[currentLine])
        {
            Text.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }


    public void NextLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (currentLine < dialogueLines.Length - 1)
        {
            currentLine++;
            typingCoroutine = StartCoroutine(TypeLine());
        }
    }
}

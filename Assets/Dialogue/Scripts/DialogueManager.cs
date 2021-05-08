using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets._2D;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public Text dialogueText;

    public TextAsset textFile;
    public string[] textLines;

    public int currentLine;
    public int endAtLine;

    public bool isActive;
    public bool stopPlayerMovement;

    private bool isTyping = false;
    private bool cancelTyping = false;
    public float typeSpeed;

    private int maxCharCounter;
    public int maxCharCount;

    public static DialogueManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        if (textFile != null)
            textLines = (textFile.text.Split('\n'));

        if (endAtLine == 0)
            endAtLine = textLines.Length - 1;

        if (isActive)
            EnableDialogueBox();
        else
            DisableDialogueBox();
    }

    void LateUpdate()
    {
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!isTyping)
                {
                    currentLine += 1;
                    if (currentLine > endAtLine)
                        DisableDialogueBox();
                    else
                        StartCoroutine(TextScroll(textLines[currentLine]));
                }
                else if (isTyping && !cancelTyping)
                    cancelTyping = true;
            }
        }
    }

    IEnumerator TextScroll(string lineOfText)
    {
        int curChar = 0;
        maxCharCounter = maxCharCount;
        dialogueText.text = "";

        isTyping = true;
        cancelTyping = false;

        while (isTyping && (curChar < lineOfText.Length))
        {
            dialogueText.text += lineOfText[curChar];
            curChar += 1;

            AudioManager.instance.PlaySound("Click_Select");
            if (curChar >= maxCharCounter && curChar != lineOfText.Length)
            {
                yield return StartCoroutine(WaitForKeyDown(KeyCode.Space));
                cancelTyping = false;
                dialogueText.text = string.Empty;
                maxCharCounter += maxCharCount;
            }

            if (!cancelTyping)
                yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        cancelTyping = false;
    }

    private IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;
    }

    public void EnableDialogueBox()
    {
        dialogueBox.SetActive(true);
        isActive = true;

        StartCoroutine(TextScroll(textLines[currentLine]));
    }

    public void DisableDialogueBox()
    {
        dialogueBox.SetActive(false);
        isActive = false;
        stopPlayerMovement = false;
    }

    public void ReloadScript(TextAsset textFile)
    {
        if (textFile != null)
        {
            textLines = new string[1];
            textLines = (textFile.text.Split('\n'));
        }
    }
}

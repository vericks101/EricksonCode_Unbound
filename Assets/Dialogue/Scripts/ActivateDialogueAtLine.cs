using UnityEngine;
using System.Collections;

public class ActivateDialogueAtLine : MonoBehaviour
{
    public TextAsset textFile;

    public int startLine;
    public int endLine;
    public int casualStartLine;
    public int casualEndLine;
    public bool stopPlayerMovement;
    public bool isCasualTalk;
    private bool isInRadiusToTalk = false;

    public DialogueManager dialogueManager;

    public bool destroyWhenActivated;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Update()
    {
        if (isInRadiusToTalk && Input.GetKeyDown(KeyCode.E) && !ChatManager.Instance.chatBoxActive && !dialogueManager.dialogueBox.activeSelf)
        {
            if (GetComponent<InventoryLink>() != null && GetComponent<InventoryLink>().linkedInventory.canvasGroup.alpha == 0f)
            {
                dialogueManager.ReloadScript(textFile);
                dialogueManager.currentLine = startLine;
                dialogueManager.endAtLine = endLine;
                dialogueManager.stopPlayerMovement = stopPlayerMovement;
                dialogueManager.EnableDialogueBox();

                if (destroyWhenActivated)
                    Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            isInRadiusToTalk = true;

            if (isCasualTalk)
            {
                startLine = Random.Range(casualStartLine, casualEndLine + 1);
                endLine = startLine;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "Player")
            isInRadiusToTalk = false;
    }
}

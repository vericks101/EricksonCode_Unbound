using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public bool chatBoxActive = false;
    [SerializeField] private ChatScrollList chatScrollList;
    [SerializeField] private InputField inputField;

    private static ChatManager instance;
    public static ChatManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ChatManager>();

            return instance;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && chatBoxActive)
            AddChatLine(inputField.text);

        if (Input.GetKeyDown(KeyCode.Return) && !chatBoxActive && !DialogueManager.Instance.isActive && !QuestNotice.Instance.isActive)
            ActivateChatBox();
        else if (Input.GetKeyDown(KeyCode.Escape) && chatBoxActive)
            DeactivateChatBox();
    }

    void ActivateChatBox()
    {
        GetComponent<CanvasGroup>().alpha = 1f;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        inputField.ActivateInputField();

        chatBoxActive = true;
    }

    public void DeactivateChatBox()
    {
        GetComponent<CanvasGroup>().alpha = 0f;
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void AddChatLine(string content)
    {
        if (content != string.Empty)
        {
            if (content.Contains("/"))
            {
                string[] contentArray = content.Split(' ');
                GetComponent<ChatCommands>().CheckCommand(content, contentArray);
                inputField.text = string.Empty;
            }
            else
            {
                CItem cItem = new CItem();
                cItem.lineContent = content;
                cItem.time = Time.time;

                chatScrollList.AddLine(cItem, chatScrollList);

                inputField.text = string.Empty;
            }

            inputField.ActivateInputField();
        }
    }
}

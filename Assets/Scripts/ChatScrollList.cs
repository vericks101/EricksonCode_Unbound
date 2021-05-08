using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CItem
{
    public string lineContent;
    public float time;
}

public class ChatScrollList : MonoBehaviour
{
    public List<CItem> chatList;
    public Transform contentPanel;

    public GameObject chatLineObject;

    private static ChatScrollList instance;
    public static ChatScrollList Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<ChatScrollList>();

            return instance;
        }
    }

    public void AddLine(CItem lineToAdd, ChatScrollList scrollList)
    {
        scrollList.chatList.Add(lineToAdd);

        GameObject newButton = Instantiate(chatLineObject);
        newButton.transform.SetParent(contentPanel);
        newButton.transform.localScale = new Vector3(1f, 1f, 1f);

        ChatButton cButton = newButton.GetComponent<ChatButton>();
        cButton.Setup(lineToAdd, this);
    }
}
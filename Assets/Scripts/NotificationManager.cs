using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public GameObject visualNoticeObj;

    private static NotificationManager instance;
    public static NotificationManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<NotificationManager>();
            return instance;
        }
    }

    private void AddVisualNotice(Sprite noticeImage, string noticeTitle, string noticeDesc)
    {
        GameObject tmp = Instantiate(visualNoticeObj, transform.position, Quaternion.identity);
        tmp.transform.SetParent(transform);
        tmp.GetComponent<NoticeObject>().noticeImage.sprite = noticeImage;
        tmp.GetComponent<NoticeObject>().noticeTitle.text = noticeTitle;
        tmp.GetComponent<NoticeObject>().noticeDesc.text = noticeDesc;
    }

    public void AddQuestNotice(string questTitle, Sprite noticeImage)
    {
        string questDesc = string.Empty;
        questDesc += "You've obtained, " + questTitle + ". Check the quests tab in ESC for more information.";

        AddVisualNotice(noticeImage, questTitle, questDesc);
    }

    public void AddQuestCompleteNotice(string questTitle, Sprite noticeImage)
    {
        string questDesc = string.Empty;
        questDesc += "You've completed, " + questTitle + ". Any rewards have been added to your inventory/character."; 

        AddVisualNotice(noticeImage, questTitle, questDesc);
    }

    public void AddRecipeNotice(string recipeName, Sprite noticeImage)
    {
        string recipeDesc = string.Empty;
        recipeDesc += "You've obtained the recipe for " + recipeName + ". Check the crafting tab for more detail.";

        AddVisualNotice(noticeImage, recipeName, recipeDesc);
    }
}

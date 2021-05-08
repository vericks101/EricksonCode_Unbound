using UnityEngine;
using System.Collections;

public class QuestTrigger : MonoBehaviour
{
    public int questIndex;
    private QuestObject questObj;

    public bool startQuest;
    public bool endQuest;

    public bool requiresOtherQuest;
    public int[] requiredQuestIndexes;

    void Start()
    {
        questObj = QuestManager.Instance.quests[questIndex];

        QuestManager.questTriggers.Add(this);
        UpdateIcon();
    }

    public void UpdateIcon()
    {
        try
        {
            var noticeObj = gameObject.transform.Find("NoticeIcon");
            if (noticeObj != null)
            {
                noticeObj.gameObject.SetActive(false);
                if (!QuestManager.Instance.questCompleted[questIndex])
                {
                    if (requiresOtherQuest && !QuestUIManager.Instance.activeQuests.Contains(questObj))
                    {
                        bool notCompleted = false;
                        for (int i = 0; i < requiredQuestIndexes.Length; i++)
                        {
                            if (!QuestManager.Instance.questCompleted[requiredQuestIndexes[i]])
                                notCompleted = true;
                        }
                        if (!notCompleted)
                            noticeObj.gameObject.SetActive(true);
                    }
                    else if (!requiresOtherQuest && !QuestUIManager.Instance.activeQuests.Contains(questObj))
                        noticeObj.gameObject.SetActive(true);
                }
            }

            var navigationObj = gameObject.transform.Find("NavigationIcon");
            if (navigationObj != null)
            {
                navigationObj.gameObject.SetActive(false);
                if (!QuestManager.Instance.questCompleted[questIndex])
                {
                    if (requiresOtherQuest && QuestUIManager.Instance.activeQuests.Contains(questObj))
                        navigationObj.gameObject.SetActive(true);
                    else if (!requiresOtherQuest && QuestUIManager.Instance.activeQuests.Contains(questObj))
                        navigationObj.gameObject.SetActive(true);
                }
            }
        }
        catch(MissingReferenceException)
        { }
        catch (System.NullReferenceException)
        { }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!QuestManager.Instance.questCompleted[questIndex])
            {
                if (startQuest && !QuestManager.Instance.quests[questIndex].gameObject.activeSelf)
                {
                    if (requiresOtherQuest)
                    {
                        bool notCompleted = false;
                        for (int i = 0; i < requiredQuestIndexes.Length; i++)
                        {
                            if (!QuestManager.Instance.questCompleted[requiredQuestIndexes[i]])
                                notCompleted = true;
                        }
                        if (!notCompleted)
                        {
                            QuestNotice.Instance.EnableQuestNotice(this, QuestManager.Instance.quests[questIndex].questTitle, QuestManager.Instance.quests[questIndex].startText);
                        }
                    }
                    else if (!requiresOtherQuest)
                    {
                        QuestNotice.Instance.EnableQuestNotice(this, QuestManager.Instance.quests[questIndex].questTitle, QuestManager.Instance.quests[questIndex].startText);
                    }
                }

                if (endQuest && QuestManager.Instance.quests[questIndex].gameObject.activeSelf)
                {
                    QuestUIManager.Instance.RemoveFromActiveQuests(QuestManager.Instance.quests[questIndex]);
                    QuestScrollList.Instance.RemoveQuest(QuestManager.Instance.quests[questIndex].qItem, QuestScrollList.Instance);

                    QuestManager.Instance.quests[questIndex].EndQuest();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!QuestManager.Instance.questCompleted[questIndex])
            {
                if (startQuest && !QuestManager.Instance.quests[questIndex].gameObject.activeSelf)
                {
                    if (requiresOtherQuest)
                    {
                        bool notCompleted = false;
                        for (int i = 0; i < requiredQuestIndexes.Length; i++)
                        {
                            if (!QuestManager.Instance.questCompleted[requiredQuestIndexes[i]])
                                notCompleted = true;
                        }
                        if (!notCompleted)
                        {
                            QuestNotice.Instance.EnableQuestNotice(this, QuestManager.Instance.quests[questIndex].questTitle, QuestManager.Instance.quests[questIndex].startText);
                        }
                    }
                    else if (!requiresOtherQuest)
                    {
                        QuestNotice.Instance.EnableQuestNotice(this, QuestManager.Instance.quests[questIndex].questTitle, QuestManager.Instance.quests[questIndex].startText);
                    }
                }

                if (endQuest && QuestManager.Instance.quests[questIndex].gameObject.activeSelf)
                {
                    QuestUIManager.Instance.RemoveFromActiveQuests(QuestManager.Instance.quests[questIndex]);
                    QuestScrollList.Instance.RemoveQuest(QuestManager.Instance.quests[questIndex].qItem, QuestScrollList.Instance);

                    QuestManager.Instance.quests[questIndex].EndQuest();
                }
            }
        }
    }

    public void TriggerQuest()
    {
        if (!QuestManager.Instance.questCompleted[questIndex])
        {
            if (startQuest && !QuestManager.Instance.quests[questIndex].gameObject.activeSelf)
            {
                if (requiresOtherQuest)
                {
                    bool notCompleted = false;
                    for (int i = 0; i < requiredQuestIndexes.Length; i++)
                    {
                        if (!QuestManager.Instance.questCompleted[requiredQuestIndexes[i]])
                            notCompleted = true;
                    }
                    if (!notCompleted)
                    {
                        QuestManager.Instance.quests[questIndex].gameObject.SetActive(true);
                        QuestManager.Instance.quests[questIndex].StartQuest();
                        QuestUIManager.Instance.AddToActiveQuests(QuestManager.Instance.quests[questIndex]);

                        QItem tmp = new QItem();
                        tmp.questName = QuestManager.Instance.quests[questIndex].questTitle;
                        tmp.questDesc = QuestManager.Instance.quests[questIndex].startText;

                        QuestScrollList.Instance.AddQuest(tmp, QuestScrollList.Instance, QuestManager.Instance.quests[questIndex], questIndex);
                        QuestManager.Instance.quests[questIndex].qItem = tmp;

                        QuestManager.Instance.UpdateIcons();
                        gameObject.SetActive(false);
                    }
                }
                else if (!requiresOtherQuest)
                {
                    QuestManager.Instance.quests[questIndex].gameObject.SetActive(true);
                    QuestManager.Instance.quests[questIndex].StartQuest();
                    QuestUIManager.Instance.AddToActiveQuests(QuestManager.Instance.quests[questIndex]);

                    QItem tmp = new QItem();
                    tmp.questName = QuestManager.Instance.quests[questIndex].questTitle;
                    tmp.questDesc = QuestManager.Instance.quests[questIndex].startText;

                    QuestScrollList.Instance.AddQuest(tmp, QuestScrollList.Instance, QuestManager.Instance.quests[questIndex], questIndex);
                    QuestManager.Instance.quests[questIndex].qItem = tmp;


                    QuestManager.Instance.UpdateIcons();
                    gameObject.SetActive(false);
                }
            }

            if (endQuest && QuestManager.Instance.quests[questIndex].gameObject.activeSelf)
            {
                QuestUIManager.Instance.RemoveFromActiveQuests(QuestManager.Instance.quests[questIndex]);
                QuestScrollList.Instance.RemoveQuest(QuestManager.Instance.quests[questIndex].qItem, QuestScrollList.Instance);

                QuestManager.Instance.quests[questIndex].EndQuest();
            }
        }
    }
}
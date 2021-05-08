using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QItem
{
    public string questName;
    public string questDesc;
    public string targetObj;
    public string killObj;
    public string itemObj;
    public string keyObj;
    public string rewardText;
    public Sprite icon;
    public GameObject button;
}

public class QuestScrollList : MonoBehaviour
{
    public List<QItem> questList;
    public Transform contentPanel;
    public QuestButtonPooler buttonObjPooler;

    private static QuestScrollList instance;
    public static QuestScrollList Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<QuestScrollList>();

            return instance;
        }
    }

    void Start()
    {
        RefreshDisplay();

        for (int i = 0; i < QuestManager.Instance.quests.Length; i++)
        {
            if (QuestManager.Instance.quests[i].gameObject.activeSelf)
            {
                QItem tmp = new QItem();
                tmp.questName = QuestManager.Instance.quests[i].questTitle;
                tmp.questDesc = QuestManager.Instance.quests[i].startText;

                tmp.targetObj = TargetObjective(QuestManager.Instance.quests[i]);
                tmp.killObj = KillObjective(QuestManager.Instance.quests[i]);
                tmp.itemObj = ItemObjective(QuestManager.Instance.quests[i]);
                tmp.keyObj = KeyText(QuestManager.Instance.quests[i]);
                tmp.rewardText = RewardText(QuestManager.Instance.quests[i]);

                AddQuest(tmp, this, QuestManager.Instance.quests[i], i);
            }
        }
    }

    public string TargetObjective(QuestObject questObj)
    {
        string targetObjString = string.Empty;
        targetObjString += "Target Objective: \n";
        targetObjString += questObj.targetObjective + "\n";
        targetObjString += "\n";

        return targetObjString;
    }

    public string KillObjective(QuestObject questObj)
    {
        string killObjString = string.Empty;
        killObjString += "Kill Objective: \n";
        killObjString += questObj.targetEnemy + "\n";
        killObjString += questObj.enemyKillCount + " / " + questObj.enemiesToKill + "\n";
        killObjString += "\n";

        return killObjString;
    }

    public string ItemObjective(QuestObject questObj)
    {
        string itemObjString = string.Empty;
        itemObjString += "Item Objective: \n";
        itemObjString += questObj.targetItem + "\n";
        itemObjString += "\n";

        return itemObjString;
    }

    public string RewardText(QuestObject questObj)
    {
        string rewardsText = string.Empty;
        rewardsText += "Money Reward: " + questObj.moneyReward + "\n";
        rewardsText += "\n";
        rewardsText += "Experience Reward: " + questObj.experienceReward + "\n";
        rewardsText += "\n";

        rewardsText += "Item Rewards: \n";
        for (int i = 0; i < questObj.itemRewards.Length; i++)
        {
            rewardsText += questObj.itemRewards[i].itemName + "\n";
        }

        return rewardsText;
    }

    public string KeyText(QuestObject questObj)
    {
        string keyText = string.Empty;
        keyText += "Key Objective: \n";
        keyText += questObj.keyCode.ToString() + "\n";
        keyText += "\n";

        return keyText;
    }

    public void RefreshDisplay()
    {
        AddButtons();
    }

    void AddButtons()
    {
        for (int i = 0; i < questList.Count; i++)
        {
            QItem questItem = questList[i];
            GameObject newButton = buttonObjPooler.GetObject();
            newButton.transform.SetParent(contentPanel);
            newButton.transform.localScale = new Vector3(1f, 1f, 1f);

            QuestButton questButton = newButton.GetComponent<QuestButton>();
            questButton.Setup(questItem, this, QuestManager.Instance.quests[i]);
        }
    }

    public void AddQuest(QItem questToAdd, QuestScrollList scrollList, QuestObject questObj, int index)
    {
        scrollList.questList.Add(questToAdd);

        if (buttonObjPooler == null)
            buttonObjPooler = FindObjectOfType<QuestButtonPooler>();
        GameObject newButton = Instantiate(buttonObjPooler.prefab);//buttonObjPooler.GetObject();
        newButton.transform.SetParent(contentPanel);
        newButton.transform.localScale = new Vector3(1f, 1f, 1f);
        questToAdd.button = newButton;
        QuestManager.Instance.quests[index].qItem.button = questToAdd.button;

        QuestButton questButton = newButton.GetComponent<QuestButton>();
        questButton.Setup(questToAdd, this, questObj);
    }

    public void RemoveQuest(QItem questToRemove, QuestScrollList scrollList)
    {
        for (int i = scrollList.questList.Count - 1; i >= 0; i--)
        {
            if (scrollList.questList[i].questName == questToRemove.questName)
            {
                scrollList.questList.RemoveAt(i);

                if (questToRemove.button != null)
                    Destroy(questToRemove.button);
                else
                {
                    QuestButton[] questButtons = QuestScrollList.instance.transform.GetComponentsInChildren<QuestButton>();
                    for (int j = 0; j < questButtons.Length; j++)
                    {
                        if (questButtons[j].nameLabel.text == questToRemove.questName)
                            Destroy(questButtons[j].gameObject);
                    }
                }
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QuestUIElement
{
    public Text questTitle;
    public Text questDescription;
    public bool active;
    public Button questButton;
    public QuestObject questObj;
}

public class QuestUIManager : MonoBehaviour
{
    public QuestUIElement[] questUIElements;
    public List<QuestObject> activeQuests;

    public int currentAtIndex = 0;
    [SerializeField] private int currentPage = 0;
    [SerializeField] private Text currentPageText;

    public Text infoTitle;
    public Text infoObjective;
    public Text infoRewards;

    private static QuestUIManager instance;
    public static QuestUIManager Instance
    {
        get
        {
            instance = GameObject.FindObjectOfType<QuestUIManager>();
            return instance;
        }
    }

    void Awake()
    {
        foreach (var questElement in questUIElements)
        {
            questElement.questTitle.text = "";
            questElement.questDescription.text = "";
        }

        infoTitle.text = "";
        infoObjective.text = "";
        infoRewards.text = "";
    }

    public string[] ActiveQuestSerializer()
    {
        string[] activeQuests = new string[this.activeQuests.Count];
        int index = 0;
        
        foreach(var quest in this.activeQuests)
        {
            string questToString = string.Empty;
            questToString += quest.questTitle + "-" + quest.rewardsText + "-" + quest.questIndex + "-" + quest.startText + "-"+ quest.endText + "-" + 
                quest.isItemQuest + "-" + quest.targetItem + "-" + quest.isEnemyQuest + "-" + quest.targetEnemy + "-" +
                quest.enemiesToKill + "-" + quest.enemyKillCount + "-;";

            activeQuests[index] = questToString;
            index++;
        }

        return activeQuests;
    }

    public void UpdateInfoUI(QuestObject questObj)
    {
        infoTitle.text = questObj.questTitle;
        infoObjective.text = questObj.startText;
        infoRewards.text = questObj.rewardsText;
    }
    
    public void AddToActiveQuests(QuestObject quest)
    {
        activeQuests.Add(quest);
        UpdateQuestUI(0, false, quest);

        currentAtIndex++;
    }

    public void RemoveFromActiveQuests(QuestObject quest)
    {
        questUIElements[quest.atIndex].active = false;
        questUIElements[quest.atIndex].questTitle.text = "";
        questUIElements[quest.atIndex].questDescription.text = "";
        questUIElements[quest.atIndex].questObj = null;
        questUIElements[quest.atIndex].questButton.onClick.RemoveAllListeners();

        activeQuests.Remove(quest);

        infoTitle.text = "";
        infoObjective.text = "";
        infoRewards.text = "";

        currentAtIndex--;
    }

    public void UpdateQuestUI(int addToindex, bool turnPage, QuestObject quest)
    {
        if (turnPage)
        {
            foreach (var questElement in questUIElements)
            {
                questElement.questTitle.text = activeQuests[currentAtIndex].questTitle;
                questElement.questDescription.text = activeQuests[currentAtIndex].startText;
                questElement.active = true;
                questElement.questObj = quest;
                questElement.questButton.onClick.AddListener(delegate { UpdateInfoUI(quest); });
                quest.atIndex = currentAtIndex;

                currentAtIndex += addToindex;
            }
        }
        else
        {
            foreach (var questElement in questUIElements)
            {
                if (!questElement.active)
                {
                    questElement.questTitle.text = activeQuests[currentAtIndex].questTitle;
                    questElement.questDescription.text = activeQuests[currentAtIndex].startText;
                    questElement.active = true;
                    questElement.questObj = quest;
                    questElement.questButton.onClick.AddListener(delegate { UpdateInfoUI(quest); });
                    quest.atIndex = currentAtIndex;

                    return;
                }
            }
        }
    }

    public void TurnPageRight()
    {
        if (currentAtIndex + questUIElements.Length > activeQuests.Count)
            return;
        else
        {
            foreach (var questElement in questUIElements)
            {
                questElement.active = false;
            }

            currentPage++;
            currentPageText.text = currentPage.ToString();
            UpdateQuestUI(1, true, null);
        }
    }

    public void TurnPageLeft()
    {
        if (currentAtIndex - questUIElements.Length < 0)
            return;
        else
        {
            foreach (var questElement in questUIElements)
            {
                questElement.active = false;
            }

            currentPage--;
            currentPageText.text = currentPage.ToString();
            UpdateQuestUI(-1, true, null);
        }
    }
}

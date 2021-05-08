using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public QuestObject[] quests;
    public List<QItem> activeQuests;
    public bool[] questCompleted;

    public string itemCollected;

    public string enemyKilled;

    public static List<QuestTrigger> questTriggers = new List<QuestTrigger>();

    public GameObject asteroidObj;

    public Sprite recipeImage;

    public static QuestManager Instance;

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

        questCompleted = new bool[quests.Length];
        
    }

    void Start()
    {
        CodecManager.Instance.LoadQuestData();
        UpdateIcons();

        QuestScrollList.Instance.RefreshDisplay();
        for (int i = 0; i < QuestManager.Instance.quests.Length; i++)
        {
            if (QuestManager.Instance.quests[i].gameObject.activeSelf)
            {
                QItem tmp = new QItem();
                tmp.questName = QuestManager.Instance.quests[i].questTitle;
                tmp.questDesc = QuestManager.Instance.quests[i].startText;

                tmp.targetObj = QuestScrollList.Instance.TargetObjective(QuestManager.Instance.quests[i]);
                tmp.killObj = QuestScrollList.Instance.KillObjective(QuestManager.Instance.quests[i]);
                tmp.itemObj = QuestScrollList.Instance.ItemObjective(QuestManager.Instance.quests[i]);
                tmp.keyObj = QuestScrollList.Instance.KeyText(QuestManager.Instance.quests[i]);
                tmp.rewardText = QuestScrollList.Instance.RewardText(QuestManager.Instance.quests[i]);

                QuestScrollList.Instance.AddQuest(tmp, QuestScrollList.Instance, QuestManager.Instance.quests[i], i);
            }
        }
    }

    void OnLevelWasLoaded()
    {
        UpdateIcons();
    }

    public void ShowQuestDialogue(string questText)
    {
        if (!DialogueManager.Instance.dialogueBox.activeSelf)
        {
            DialogueManager.Instance.textLines = new string[1];
            DialogueManager.Instance.textLines[0] = questText;

            DialogueManager.Instance.currentLine = 0;
            DialogueManager.Instance.endAtLine = 0;
            DialogueManager.Instance.EnableDialogueBox();

            DialogueManager.Instance.stopPlayerMovement = true;
        }
    }

    public void UpdateIcons()
    {
        foreach (QuestTrigger questTrigger in questTriggers)
        {
            questTrigger.UpdateIcon();
        }
    }
}

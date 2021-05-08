using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestButton : MonoBehaviour
{
    public Button button;
    public Text nameLabel;
    public Text descriptionLabel;
    private QuestObject questObj;

    private QItem quest;

    public void Setup(QItem currentItem, QuestScrollList currentScrollList, QuestObject questObj)
    {
        quest = currentItem;
        this.questObj = questObj;
        nameLabel.text = quest.questName;
        descriptionLabel.text = quest.questDesc;
        button.onClick.AddListener(delegate { UpdateQuestUI(); });
    }

    private void UpdateQuestUI()
    {
        QuestUIManager.Instance.infoObjective.text = string.Empty;
        QuestUIManager.Instance.infoRewards.text = string.Empty;

        quest.targetObj = QuestScrollList.Instance.TargetObjective(questObj);
        quest.killObj = QuestScrollList.Instance.KillObjective(questObj);
        quest.itemObj = QuestScrollList.Instance.ItemObjective(questObj);
        quest.keyObj = QuestScrollList.Instance.KeyText(questObj);
        quest.rewardText = QuestScrollList.Instance.RewardText(questObj);

        QuestUIManager.Instance.infoObjective.text += quest.targetObj;
        QuestUIManager.Instance.infoObjective.text += quest.killObj;
        QuestUIManager.Instance.infoObjective.text += quest.itemObj;
        QuestUIManager.Instance.infoObjective.text += quest.keyObj;
        QuestUIManager.Instance.infoRewards.text += quest.rewardText;
    }
}
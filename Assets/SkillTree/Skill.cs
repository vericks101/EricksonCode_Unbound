using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Skill : MonoBehaviour
{
    public string skillName;
    public string skillDescription;
    public Sprite skillImage;
    public Button skillButton;
    public int skillCost;
    public int ofIndex;
    public int requiredNodes;
    public bool unlockedByDefault = false;
    public bool hasSkill;

    private GameObject skillMaskObj;

    void Start()
    {
        skillButton = gameObject.GetComponent<Button>();
        skillButton.onClick.AddListener(delegate { OnNodePress(); });

        skillMaskObj = transform.Find("SkillMask").gameObject;
        if (hasSkill || unlockedByDefault || SkillTreeManager.Instance.skillTrees[ofIndex].acuiredNodes >= requiredNodes)
        {
            skillMaskObj.SetActive(false);
            if (hasSkill)
                GetComponent<Image>().color = SkillTreeManager.Instance.obtainedSkillColor;
        }
    }

    public void CheckNodeUnlockStatus()
    {
        if (hasSkill || unlockedByDefault || SkillTreeManager.Instance.skillTrees[ofIndex].acuiredNodes >= requiredNodes)
        {
            skillMaskObj.SetActive(false);
            if (hasSkill)
                GetComponent<Image>().color = SkillTreeManager.Instance.obtainedSkillColor;
        }
    }

    public void OnNodePress()
    {
        SkillTreeManager.Instance.previewName.text = skillName;
        SkillTreeManager.Instance.previewDescription.text = skillDescription;
        SkillTreeManager.Instance.previewCost.text = "Cost: " + skillCost;

        SkillTreeManager.Instance.selectedSkill = null;
        SkillTreeManager.Instance.selectedSkill = this;
    }

    public void ActivateNode()
    {
        if (SkillTreeManager.Instance.skillPoints >= skillCost && !hasSkill && SkillTreeManager.Instance.skillTrees[ofIndex].acuiredNodes >= requiredNodes)
        {
            SkillTreeManager.Instance.skillPoints -= skillCost;
            SkillTreeManager.Instance.AdjustCurrentPoints();
            TriggerNode();
            hasSkill = true;
            SkillTreeManager.Instance.skillTrees[ofIndex].acuiredNodes++;

            for (int i = 0; i < SkillTreeManager.Instance.skillTrees[ofIndex].nodes.Length; i++)
            {
                SkillTreeManager.Instance.skillTrees[ofIndex].nodes[i].CheckNodeUnlockStatus();
            }
        }
    }

    public abstract void TriggerNode();
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class SkillTree
{
    public string treeName;
    public int acuiredNodes;
    public GameObject treeObj;
    public Skill[] nodes;
}

public class SkillTreeManager : MonoBehaviour
{
    public SkillTree[] skillTrees;

    public Text previewName;
    public Text previewDescription;
    public Text previewPoints;
    public Text previewCost;

    public Button confirmButton;
    public Skill selectedSkill;

    public int skillPoints = 0;

    public Color obtainedSkillColor;

    private static SkillTreeManager instance;
    public static SkillTreeManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<SkillTreeManager>();

            return instance;
        }
    }

    void OnEnable()
    {
        confirmButton.onClick.AddListener(delegate { OnConfirmPress(); });

        for (int i = 0; i < skillTrees.Length; i++)
        {
            skillTrees[i].treeObj.transform.Find("TreeTitle").GetComponent<Text>().text = skillTrees[i].treeName;
            var nodes = skillTrees[i].nodes = skillTrees[i].treeObj.GetComponentsInChildren<Skill>();
            
            for (int j = 0; j < nodes.Length; j++)
            {
                nodes[j].ofIndex = i;
            }
        }

        previewName.text = string.Empty;
        previewDescription.text = string.Empty;
        previewCost.text = "Cost: ";

        AdjustCurrentPoints();
    }

    public void AdjustCurrentPoints()
    {
        previewPoints.text = string.Format("CURRENT SKILL POINTS: {0}", skillPoints);
    }

    public void OnConfirmPress()
    {
        if (selectedSkill != null)
            selectedSkill.ActivateNode();
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EXPManager : MonoBehaviour
{
    public int maxLevel;
    public int curLevel;

    public float experienceScale = 1f;
    public int experienceToNextLvl;
    public int currentExperience;

    public Text curLevelText;

    void Awake()
    {
        curLevelText = GameObject.Find("LevelText").GetComponent<Text>();
    }

    void Start()
    {
        CodecManager.Instance.LoadProfile("Profile", 0);
        //CodecManager.Instance.SavePlayerData();
        CodecManager.Instance.LoadPlayerData();
        CodecManager.Instance.LoadPreviewData();

        Player.Instance.experience.MaxVal = experienceToNextLvl;
        Player.Instance.experience.CurrentVal = currentExperience;

        curLevelText.text = curLevel.ToString();
    }

    public void UpdateCurrentExperience(int experience)
    {
        currentExperience += experience;
        Player.Instance.experience.CurrentVal = currentExperience;

        while (currentExperience >= experienceToNextLvl && curLevel < maxLevel)
        {
            curLevel++;
            SkillTreeManager.Instance.skillPoints++;
            curLevelText.text = curLevel.ToString();

            experienceToNextLvl = Mathf.RoundToInt(experienceToNextLvl * experienceScale);
            Player.Instance.experience.MaxVal = experienceToNextLvl;
        }
        SkillTreeManager.Instance.AdjustCurrentPoints();

        Player.Instance.experience.CurrentVal = currentExperience;
    }
}
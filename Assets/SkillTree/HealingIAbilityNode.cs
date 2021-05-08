using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class HealingIAbilityNode : Skill
{
    public override void TriggerNode()
    {
        for (int i = 0; i < AbilityManager.Instance.abilityDropdowns.Length; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData(AbilityManager.Instance.abilities[1].aName, AbilityManager.Instance.abilities[1].aSprite);
            AbilityManager.Instance.abilityDropdowns[i].options.Add(data);
        }
    }
}

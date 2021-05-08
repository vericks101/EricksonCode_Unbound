using UnityEngine;
using System.Collections;
using System;

public class StrengthNode : Skill
{
    public float percentageIncrease;

    public override void TriggerNode()
    {
        Player.Instance.damageIncreaseFactor = percentageIncrease;
    }
}

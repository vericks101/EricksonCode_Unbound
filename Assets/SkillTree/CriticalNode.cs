using UnityEngine;
using System.Collections;
using System;

public class CriticalNode : Skill
{
    public float percentageIncrease;

    public override void TriggerNode()
    {
        Player.Instance.criticalStrikeChance = percentageIncrease;
    }
}

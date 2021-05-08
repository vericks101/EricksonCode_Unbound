using UnityEngine;
using System.Collections;
using System;

public class SwiftnessNode : Skill
{
    public float percentageIncrease;

    public override void TriggerNode()
    {
        Player.Instance.swiftnessChance = percentageIncrease;
    }
}

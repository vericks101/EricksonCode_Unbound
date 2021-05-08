using UnityEngine;
using System.Collections;
using System;

public class RegenerationNode : Skill
{
    public float increasePercentage;

    public override void TriggerNode()
    {
        var numToSubtract = increasePercentage * Player.Instance.baseHealthRegen;
        Player.Instance.healthRegenCD -= Player.Instance.healthRegenCD;
        Player.Instance.baseHealthRegen -= numToSubtract;
    }
}

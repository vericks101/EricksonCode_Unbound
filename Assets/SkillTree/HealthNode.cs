using UnityEngine;
using System.Collections;
using System;

public class HealthNode : Skill
{
    public float healthIncrease;

    public override void TriggerNode()
    {
        Player.Instance.health.MaxVal += healthIncrease;
    }
}

using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections;

public class MovementNode : Skill
{
    public float percentageIncrease;

    public override void TriggerNode()
    {
        var numToAdd = percentageIncrease * Player.Instance.GetComponent<PlatformerCharacter2D>().m_MaxSpeed;
        Player.Instance.GetComponent<PlatformerCharacter2D>().m_MaxSpeed += numToAdd;
    }
}
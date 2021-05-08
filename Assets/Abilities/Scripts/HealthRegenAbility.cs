using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Ability/HealthRegenAbility")]
public class HealthRegenAbility : Ability
{
    public float regenCD;
    public float regenTime;

    public override void Initialize(GameObject obj)
    { }

    public override void TriggerAbility()
    {
        Player.Instance.healthRegenCD = regenCD;
        Player.Instance.regenCDChangeTimer = regenTime + Time.time;
    }
}

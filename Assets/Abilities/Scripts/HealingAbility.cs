using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Ability/HealingAbility")]
public class HealingAbility : Ability
{
    public int healingAmount = 1;

    public override void Initialize(GameObject obj)
    { }

    public override void TriggerAbility()
    {
        Player.Instance.health.CurrentVal += healingAmount;
    }
}

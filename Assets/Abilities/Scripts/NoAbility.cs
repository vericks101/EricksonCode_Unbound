using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Ability/NoAbility")]
public class NoAbility : Ability
{
    public override void Initialize(GameObject obj)
    { }

    public override void TriggerAbility()
    { }
}

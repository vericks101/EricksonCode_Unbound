using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu (menuName = "Ability/ProjectileAbility")]
public class ProjectileAbility : Ability
{
    public float projectileForce = 100f;

    private Gun gunLauncher;

    public override void Initialize(GameObject obj)
    {
        gunLauncher = obj.GetComponent<Gun>();
    }

    public override void TriggerAbility()
    {
        gunLauncher.Shoot();
    }
}

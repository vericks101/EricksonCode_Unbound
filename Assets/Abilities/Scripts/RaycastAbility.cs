using UnityEngine;
using System;
using System.Collections;


[CreateAssetMenu (menuName = "Ability/RaycastAbility")]
public class RaycastAbility : Ability
{
    public int gunDamage = 1;
    public float weaponRange = 50f;
    public float hitForce = 100f;
    public Color laserColor = Color.white;

    private Gun gun;

    public override void Initialize(GameObject obj)
    {
        gun = obj.GetComponent<Gun>();

        gun.damage = gunDamage;
    }

    public override void TriggerAbility()
    {
        gun.Shoot();
    }
}

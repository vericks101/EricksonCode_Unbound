using UnityEngine;
using System.Collections;

public abstract class Ability : ScriptableObject
{
    public string aName = "New Ability";
    public Sprite aSprite;
    public AudioClip aSound;
    public float aBaseCooldown = 1f;
    public float manaCost;

    public abstract void Initialize(GameObject obj);
    public abstract void TriggerAbility();
}

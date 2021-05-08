using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class AbilityHolder
{
    public GameObject abilityObj;
    public Dropdown abilityDropdown;
    public int curDropdownIndex;
}

public class AbilityManager : MonoBehaviour
{
    public AbilityHolder[] abilityHolders;

    public Ability[] abilities;
    public Dropdown[] abilityDropdowns;

    private static AbilityManager instance;
    public static AbilityManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<AbilityManager>();
            return instance;
        }
    }

    void Awake()
    {
        for (int i = 0; i < abilityHolders.Length; i++)
        {
            abilityHolders[i].abilityObj.GetComponent<AbilityCooldown>().ability = Player.Instance.playerAbilities[0].ability;
            abilityHolders[i].abilityObj.GetComponent<AbilityCooldown>().objHolder = Player.Instance.playerAbilities[0].objHolder;
            abilityHolders[i].curDropdownIndex = 0;
        }
    }

    public void UpdateAbilityHolder(int dropdownIndex)
    {
        int selectedIndex = AbilityManager.instance.abilityDropdowns[dropdownIndex].value;
        for (int index = 0; index < AbilityManager.instance.abilities.Length; index++)
        {
            if (abilities[index].aSprite.name == AbilityManager.instance.abilityDropdowns[dropdownIndex].options[selectedIndex].image.name)
            {
                abilityHolders[dropdownIndex].abilityObj.GetComponent<AbilityCooldown>().ability = abilities[index];
                abilityHolders[dropdownIndex].abilityObj.GetComponent<AbilityCooldown>().objHolder = Player.Instance.playerAbilities
                    [abilityHolders[dropdownIndex].abilityDropdown.value].objHolder;
                abilityHolders[dropdownIndex].curDropdownIndex = abilityHolders[dropdownIndex].abilityDropdown.value;

                abilityHolders[dropdownIndex].abilityObj.GetComponent<AbilityCooldown>().Initialize(abilityHolders[dropdownIndex].abilityObj.GetComponent<AbilityCooldown>().ability,
                    abilityHolders[dropdownIndex].abilityObj.GetComponent<AbilityCooldown>().objHolder);
            }
        }
    }

    public void LoadAbilityHolder(int index, int curDropdownIndex)
    {
        abilityHolders[index].abilityObj.GetComponent<AbilityCooldown>().ability = Player.Instance.playerAbilities
            [curDropdownIndex].ability;
        abilityHolders[index].abilityObj.GetComponent<AbilityCooldown>().objHolder = Player.Instance.playerAbilities
            [curDropdownIndex].objHolder;
        abilityHolders[index].curDropdownIndex = curDropdownIndex;
        abilityHolders[index].abilityDropdown.value = curDropdownIndex;

        abilityHolders[index].abilityObj.GetComponent<AbilityCooldown>().Initialize(abilityHolders[index].abilityObj.GetComponent<AbilityCooldown>().ability,
            abilityHolders[index].abilityObj.GetComponent<AbilityCooldown>().objHolder);
    }
}
using UnityEngine;
using System.Collections;
using System;

public class CharacterPanel : Inventory 
{
    public Slot[] equipmentSlots;

    public Element weaponElement;
    public float weaponDamage;
    public Slot[] armorSlots;
    public float armorDefense = 1f;

    private static CharacterPanel instance;
    public static CharacterPanel Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<CharacterPanel> ();

            return CharacterPanel.instance; 
        }
    }

    public Slot WeaponSlot
    {
        get { return equipmentSlots[9]; }
    }

    public Slot OffHandSlot
    {
        get { return equipmentSlots[10]; }
    }


    void Awake()
    {
		equipmentSlots = transform.GetComponentsInChildren<Slot> ();
    }

    void OnLevelWasLoaded()
    {
        if (WeaponSlot.Items.Count > 0)
            weaponDamage = WeaponSlot.CurrentItem.Item.AttackDamage;
    }

    public override void CreateLayout()
    { }

    public void EquipItem(Slot slot, ItemScript item)
    {
        if (item.Item.ItemType == ItemType.MAINHAND || item.Item.ItemType == ItemType.TWOHAND && OffHandSlot.IsEmpty)
        {
            Slot.SwapItems(slot, WeaponSlot);
            weaponElement = item.GetElement();
            weaponDamage = item.GetAttackDamage();
        }
        else
        {
			Slot.SwapItems (slot, Array.Find (equipmentSlots, x => x.canContain == item.Item.ItemType));
            armorDefense = CalculateArmorDefense(item);
            if (item.Item.ItemName == "Jetpack")
                Player.Instance.jetpack.SetActive(true);
            if (armorSlots[2].Items.Count <= 0 || (armorSlots[2].Items.Count > 0 && armorSlots[2].CurrentItem.Item.ItemName != "Jetpack"))
                Player.Instance.jetpack.SetActive(false);
        }
    }

    public float CalculateArmorDefense(ItemScript item)
    {
        float defenseCount = 1;
        foreach(Slot slot in armorSlots)
        {
            if (slot.Items.Count > 0 && (slot.CurrentItem.GetElement() == CurrentSceneManager.Instance.planetElement || slot.CurrentItem.GetElement() == Element.LUMINANT
                || slot.CurrentItem.GetElement() == Element.COMMON))
                defenseCount += slot.CurrentItem.GetDefense();
            else if (slot.Items.Count > 0)
                defenseCount++;

            if (slot.Items.Count > 0 && (slot.CurrentItem.GetElement() == CurrentSceneManager.Instance.planetElement || slot.CurrentItem.GetElement() == Element.LUMINANT
                || slot.CurrentItem.GetElement() == Element.COMMON))
                defenseCount++;
        }

        return (1 / defenseCount);
    }

    public override void ShowToolTip(GameObject slot)
    {
		Slot tmpSlot = slot.GetComponent<Slot> ();

		if (slot.GetComponentInParent<Inventory> ().IsOpen && !tmpSlot.IsEmpty && InventoryManager.Instance.HoverObject == null 
			&& !InventoryManager.Instance.selectStackSize.activeSelf)
        {
			InventoryManager.Instance.visualTextObject.text = tmpSlot.CurrentItem.GetTooltip (this);

			InventoryManager.Instance.sizeTextObject.text = InventoryManager.Instance.visualTextObject.text;

			InventoryManager.Instance.tooltipObject.SetActive (true);
			InventoryManager.Instance.tooltipObject.transform.position = slot.transform.position;
        }
    }

    public void CalcStats()
    {
        int agility = 0;
        int strength = 0;
        int stamina = 0;
        int intellect = 0;
        int defense = 0;

        foreach (Slot slot in equipmentSlots)
        {
            if (!slot.IsEmpty)
            {
				Equipment e = (Equipment)slot.CurrentItem.Item;
                agility += e.Agility;
                strength += e.Strength;
                stamina += e.Stamina;
                intellect += e.Intellect;
                defense += (int)e.Defense;
            }
        }

        Player.Instance.SetStats(agility, strength, stamina, intellect, defense);
    }

    public override void SaveInventory()
    {
        string content = string.Empty;

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (!equipmentSlots[i].IsEmpty) content += i + "-" + equipmentSlots [i].Items.Peek ().Item.ItemName + ";";
        }

        PlayerPrefs.SetString("CharPanel", content); 
        PlayerPrefs.Save();
    }

    public override void LoadInventory()
    {
        foreach (Slot slot in equipmentSlots) 
        {
            slot.ClearSlot();
        }

        string content = PlayerPrefs.GetString("CharPanel");

		string[] splitContent = content.Split (';');

        for (int i = 0; i < splitContent.Length-1; i++)
        {   
			string[] splitValues = splitContent [i].Split ('-');

            int index = Int32.Parse(splitValues[0]);

            string itemName = splitValues[1];

			GameObject loadedItem = Instantiate (InventoryManager.Instance.itemObject);
            loadedItem.AddComponent<ItemScript>();

            if (index == 9 || index == 10)
            {
				loadedItem.GetComponent<ItemScript> ().Item = InventoryManager.Instance.ItemContainer.Weapons.Find (x => x.ItemName == itemName);
            }
            else
	        {
				loadedItem.GetComponent<ItemScript> ().Item = InventoryManager.Instance.ItemContainer.Equipment.Find (x => x.ItemName == itemName);
	        }
				
			equipmentSlots [index].AddItem (loadedItem.GetComponent<ItemScript> ());

			Destroy (loadedItem);

			CalcStats ();
        }
    }

}

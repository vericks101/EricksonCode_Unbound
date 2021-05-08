using UnityEngine;
using System.Collections;

public class Equipment : Item
{
    public int Intellect { get; set; }

    public int Agility { get; set; }

    public int Stamina { get; set; }

    public int Strength { get; set; }

    public Equipment()
    { }

    public Equipment(string itemName, string description, ItemType itemType, Quality quality, string spriteNeutral, string spriteHighlighted, int maxSize, int intellect, int agility, int stamina, int strength, int craftAmount, int layerId, Element elementa, float defense)
        : base(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, craftAmount, layerId)
    {
        this.Defense = defense;

        this.Intellect = intellect;
        this.Agility = agility;
        this.Stamina = stamina;
        this.Strength = strength;

        this.Elementa = elementa;
    }

    public override void Use(Slot slot, ItemScript item)
    {
        CharacterPanel.Instance.EquipItem(slot, item);
        Player.Instance.inventorySelect.ChangeCurrentItemText();
    }

	public override string GetTooltip(Inventory inv)
    {
        string stats = string.Empty;

        if (Strength > 0)
        {
			stats += "\n+" + Strength.ToString () + " Strength";
        }
        if (Intellect > 0)
        {
			stats += "\n+" + Intellect.ToString () + " Intellect";
        }
        if (Agility > 0)
        {
			stats += "\n+" + Agility.ToString () + " Agility";
        }
        if (Stamina > 0)
        {
			stats += "\n+" + Stamina.ToString () + " Stamina";
        }

        string itemTip = base.GetTooltip(inv);

		if (inv is VendorInventory && !(this is Weapon)) 
		{
			return string.Format("{0}" + "<size=14>{1}\n<color=white>Defense: {2}</color>\n<color=yellow>Buy Price: {3}</color></size>", itemTip, stats, Defense, BuyPrice);
		}
		else if (VendorInventory.Instance.IsOpen && !(this is Weapon))
		{
			return string.Format("{0}" + "<size=14>{1}\n<color=white>Defense: {2}</color>\n<color=yellow>Buy Price: {3}\nSell Price: {4}</color></size>", itemTip, stats, Defense, BuyPrice, SellPrice);
		}
		else
		{
			return string.Format("{0}" + "<size=14>{1}\n<color=white>Defense: {2}</color></size>", itemTip, stats, Defense);
		}
    }
}

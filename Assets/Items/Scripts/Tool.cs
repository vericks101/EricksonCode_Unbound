using UnityEngine;
using System.Collections;

public class Tool : Equipment
{
	public Tool()
	{ }

	public Tool(string itemName, string description, ItemType itemType, Quality quality, string spriteNeutral, string spriteHighlighted, int maxSize, int intellect, int agility, int stamina, int strength, float attackSpeed, int craftAmount, int layerId, Element elementa, float defense, string itemSprite)
		: base(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, intellect, agility, stamina, strength, craftAmount, layerId, elementa, defense)
	{
		this.AttackSpeed = attackSpeed;
        this.Elementa = elementa;
        this.ItemSprite = itemSprite;
	}

	public override string GetTooltip(Inventory inv)
	{
		string equipmentTip = base.GetTooltip(inv);

        //if (inv is VendorInventory)
        //{
        //    return string.Format("{0} \n <size=14>AttackSpeed: {1}\n<color=yellow>Buy Price: {2}</color></size>", equipmentTip, AttackSpeed, BuyPrice);
        //}
        //else if (VendorInventory.Instance.IsOpen)
        //{
        //    return string.Format("{0} \n <size=14>AttackSpeed: {1}\n<color=yellow>Buy Price: {2}\nSell Price: {3}</color></size>", equipmentTip, AttackSpeed, BuyPrice, SellPrice);
        //}
        //else
        //{
        return string.Format("{0}\n<size=14>AttackSpeed: {1}</size>", equipmentTip, AttackSpeed);
        //}
    }
}
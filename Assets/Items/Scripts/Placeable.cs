using UnityEngine;
using System.Collections;

public class Placeable : Item
{
    public Placeable(string itemName, string description, ItemType itemType, Quality quality, string spriteNeutral, string spriteHighlighted, int maxSize, int itemID, int width, int height, int craftAmount, int layerId)
		: base(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, craftAmount, layerId)
    { }

	public Placeable()
	{ }

	public override void Use(Slot slot, ItemScript item)
	{ }

	public override string GetTooltip (Inventory inv)
	{
		string placeableTip = base.GetTooltip (inv);

		if (inv is VendorInventory) 
		{
			return string.Format ("{0} \n<size=14><color=yellow>Buy Price: {1}</color></size>", placeableTip, BuyPrice);
		}
		else if (VendorInventory.Instance.IsOpen)
		{
			return string.Format ("{0} \n<size=14><color=yellow>Buy Price: {1}\nSell Price: {2}</color></size>", placeableTip, BuyPrice, SellPrice);
		}

		return placeableTip;
	}
}

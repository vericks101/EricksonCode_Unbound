using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Material : Item
{

    public Material(string itemName, string description, ItemType itemType, Quality quality, string spriteNeutral, string spriteHighlighted, int maxSize, int craftAmount, int layerId)
        : base(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, craftAmount, layerId)
    { }

    public Material()
    { }

	public override string GetTooltip (Inventory inv)
	{
		string materialTip = base.GetTooltip (inv);

		if (inv is VendorInventory) 
		{
			return string.Format ("{0} \n<size=14><color=yellow>Buy Price: {1}</color></size>", materialTip, BuyPrice);
		}
		else if (VendorInventory.Instance.IsOpen)
		{
			return string.Format ("{0} \n<size=14><color=yellow>Buy Price: {1}\nSell Price: {2}</color></size>", materialTip, BuyPrice, SellPrice);
		}

		return materialTip;
	}

    public override void Use(Slot slot, ItemScript item)
    { }
}

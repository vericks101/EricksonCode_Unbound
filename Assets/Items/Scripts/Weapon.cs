using UnityEngine;
using System.Collections;

public enum Element { COMMON, KINETIC, GROUND, SOLAR, ICE, WATER, LUMINANT}

public class Weapon : Equipment
{
    public Weapon()
    { }

    public Weapon(string itemName, string description, ItemType itemType, Quality quality, string spriteNeutral, string spriteHighlighted, int maxSize, int intellect, int agility, int stamina, int strength, float attackSpeed, int craftAmount, int layerId, Element elementa, float attackDamage, string itemSprite)
        : base(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, intellect, agility, stamina, strength, craftAmount, layerId, elementa, attackDamage)
    {
        this.AttackDamage = attackDamage;
        this.AttackSpeed = attackSpeed;
        this.Elementa = elementa;
        this.ItemSprite = itemSprite;
    }

    public override void Use(Slot slot, ItemScript item)
    {
        if (!GameManager.Instance.battleReady)
        {
            CharacterPanel.Instance.EquipItem(slot, item);
            InventoryManager.Instance.tooltipObject.SetActive(false);
            slot.transform.parent.GetComponent<Inventory>().ShowToolTip(slot.gameObject);
            Player.Instance.inventorySelect.ChangeCurrentItemText();
        }
    }

	public override string GetTooltip(Inventory inv)
    {
		string equipmentTip = base.GetTooltip (inv);

		if (inv is VendorInventory) 
		{
			return string.Format("{0} \n<size=14>AttackSpeed: {1} \nAttack Damage: {2} \n<color=yellow>Buy Price: {3}</color></size>", equipmentTip, AttackSpeed, AttackDamage, BuyPrice);
		}
		else if(VendorInventory.Instance.IsOpen)
		{
			return string.Format("{0} \n<size=14>AttackSpeed: {1} \nAttack Damage: {2} \n<color=yellow>Buy Price: {3}\nSell Price: {4}</color></size>", equipmentTip, AttackSpeed, AttackDamage, BuyPrice, SellPrice);
		}
		else
		{
			return string.Format("{0} \n<size=14>AttackSpeed: {1}</size> \nAttack Damage: {2}", equipmentTip, AttackSpeed, AttackDamage);
		}
    }
}

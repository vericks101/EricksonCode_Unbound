using UnityEngine;
using System.Collections;

public enum ItemType {CONSUMEABLE, MAINHAND,TWOHAND, OFFHAND, HEAD, NECK, CHEST, RING, LEGS, BRACERS, BOOTS, TRINKET, SHOULDERS, BELT, GENERIC, GENERICWEAPON, 
	MATERIAL, PLACEABLE, TOOL, PICKAXE, AXE, HAMMER};
public enum Quality {COMMON, UNCOMMON, RARE, EPIC, LEGENDARY, ARTIFACT}

public class ItemScript : MonoBehaviour 
{
    public Sprite spriteNeutral;
    public Sprite spriteHighlighted;
    public Sprite itemSprite;

    public int itemCount;

    public bool isTorched;

    private Item item;

    public Item Item
    {
        get { return item; }
        set 
        {
            item = value;
            
            if (value != null && value.SpriteHighlighted != null)
			    spriteHighlighted = Resources.Load<Sprite>(value.SpriteHighlighted);
            if (value != null && value.SpriteNeutral != null)
                spriteNeutral = Resources.Load<Sprite>(value.SpriteNeutral);
            if (value != null && value.ItemSprite != null)
                itemSprite = Resources.Load<Sprite>(value.ItemSprite);
        }
    }

    public void Use(Slot slot)
    {
        item.Use(slot, this);
    }

	public string GetTooltip(Inventory inv)
    {
        return item.GetTooltip(inv);
    }

    public Element GetElement()
    {
        return item.Elementa;
    }

    public float GetAttackDamage()
    {
        return item.AttackDamage;
    }

    public float GetAttackSpeed()
    {
        return item.AttackSpeed;
    }

    public float GetDefense()
    {
        return item.Defense;
    }
}

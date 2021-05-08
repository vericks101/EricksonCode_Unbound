using UnityEngine;
using System.Collections;

public abstract class Item
{
    public ItemType ItemType { get; set; }

    public Quality Quality { get; set; }

    public string SpriteNeutral { get; set; }

    public string SpriteHighlighted { get; set; }

    public int MaxSize { get; set; }

    public string ItemName { get; set; }

    public string Description { get; set; }

    public int BuyPrice { get; set; }

    public int SellPrice { get; set; }

    public int ItemID { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int CraftAmount { get; set; }

    public int LayerID { get; set; }

    public Element Elementa { get; set; }

    public float Defense { get; set; }

    public float AttackDamage { get; set; }

    public float AttackSpeed { get; set; }

    public string ItemSprite { get; set; }

    public Item()
    { }

	public Item(string itemName, string description, ItemType itemType, Quality quality, string spriteNeutral, string spriteHighlighted, int maxSize, int craftAmount, int layerId)
    {
        this.ItemName = itemName;
        this.Description = description;
        this.ItemType = itemType;
        this.Quality = quality;
        this.SpriteNeutral = spriteNeutral;
        this.SpriteHighlighted = spriteHighlighted;
        this.MaxSize = maxSize;
        this.CraftAmount = craftAmount;
        this.LayerID = layerId;
    }

    public abstract void Use(Slot slot, ItemScript item);

	public virtual string GetTooltip(Inventory inv)
    {
        string color = string.Empty;
        string newLine = string.Empty;

        if (Description != string.Empty)
        {
            newLine = "\n";
        }

        switch (Quality)
        {
            case Quality.COMMON:
                color = "white";
                break;
            case Quality.UNCOMMON:
                color = "lime";
                break;
            case Quality.RARE:
                color = "navy";
                break;
            case Quality.EPIC:
                color = "magenta";
                break;
            case Quality.LEGENDARY:
                color = "orange";
                break;
            case Quality.ARTIFACT:
                color = "red";
                break;
        }

		return string.Format("<color=" + color + "><size=16>{0}</size></color><size=14><i><color=grey>" + newLine + "{1}</color></i>\n{2} - {3}</size>", ItemName, Description, ItemType.ToString(), Elementa);
    }
}
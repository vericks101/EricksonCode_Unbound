using UnityEngine;
using System.Collections;
using System;
using System.Xml.Serialization;
using System.IO;

public enum Category { EQUIPMENT, WEAPON, CONSUMEABLE, MATERIAL, PLACEABLE, TOOL }

public class ItemManager : MonoBehaviour
{
    public ItemType itemType;
    public Quality quality;
    public Category categeory;
    public string spriteNeutral;
    public string spriteHighlighted;
    public string itemName;
    public string description;
    public int maxSize;
    public int intellect;
    public int agility;
    public int stamina;
    public int strength;
    public float attackSpeed;
    public int health;
    public int mana;
	public int itemID;
	public int width;
	public int height;
    public int craftAmount;
    public int layerId;
    public Element elementa;
    public float attackDamage;
    public float defense;
    public string itemSprite;

    public void CreateItem()
    {
        ItemContainer itemContainer = new ItemContainer();

		Type[] itemTypes = { typeof(Equipment), typeof(Weapon), typeof(Consumeable), typeof(Material), typeof(Placeable), typeof(Tool) };

        FileStream fs = new FileStream(Path.Combine(Application.streamingAssetsPath, "Items.xml"), FileMode.Open);
        XmlSerializer serializer = new XmlSerializer(typeof(ItemContainer), itemTypes);

        itemContainer = (ItemContainer)serializer.Deserialize(fs);
        serializer.Serialize(fs, itemContainer);

        fs.Close();

        switch (categeory)
        {
            case Category.EQUIPMENT:
                itemContainer.Equipment.Add(new Equipment(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, intellect, agility, 
				stamina, strength, craftAmount, layerId, elementa, defense));
                break;
            case Category.WEAPON:
                itemContainer.Weapons.Add(new Weapon(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, intellect, agility, 
				stamina, strength, attackSpeed, craftAmount, layerId, elementa, attackDamage, itemSprite));
                break;
            case Category.CONSUMEABLE:
                itemContainer.Consumeables.Add(new Consumeable(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, health, mana, craftAmount, layerId));
                break;
			case Category.MATERIAL:
                itemContainer.Materials.Add(new Material(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, craftAmount, layerId));
				break;
			case Category.PLACEABLE:
			itemContainer.Placeables.Add(new Placeable(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, itemID, width, height, craftAmount, layerId));
				break;
			case Category.TOOL:
			itemContainer.Tools.Add(new Tool(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize, intellect, agility, stamina,
				strength, attackSpeed, craftAmount, layerId, elementa, defense, itemSprite));
				break;
        }

        fs = new FileStream(Path.Combine(Application.streamingAssetsPath, "Items.xml"), FileMode.Create);
        serializer.Serialize(fs, itemContainer);
        
        fs.Close();
    }
}
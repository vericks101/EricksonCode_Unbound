using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemContainers { WEAPONS, EQUIPMENT, CONSUMEABLES, MATERIALS, PLACEABLES, TOOLS }

public class ItemContainer
{
	private List<Item> weapons = new List<Item> ();

	private List<Item> equipment = new List<Item> ();

	private List<Item> consumeables = new List<Item> ();

	private List<Item> materials = new List<Item> ();

	private List<Item> placeables = new List<Item> ();

	private List<Item> tools = new List<Item> ();

	public List<Item> Tools
	{
		get { return tools; }
		set { tools = value; }
	}

	public List<Item> Placeables
	{
		get { return placeables; }
		set { placeables = value; }
	}

    public List<Item> Materials
    {
        get { return materials; }
        set { materials = value; }
    }

    public List<Item> Consumeables
    {
        get { return consumeables; }
        set { consumeables = value; }
    }

    public List<Item> Weapons
    {
        get { return weapons; }
        set { weapons = value; }
    }

    public List<Item> Equipment
    {
        get { return equipment; }
        set { equipment = value; }
    }

    public ItemContainer()
    { }
}

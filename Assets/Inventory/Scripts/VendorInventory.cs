using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VendorInventory : ChestInventory
{
    public Text goldText;

	private static VendorInventory instance;

	public static VendorInventory Instance
	{
		get
		{
			if (instance == null) instance = GameObject.FindObjectOfType<VendorInventory> ();

			return instance;
		}
	}

    protected override void Start()
	{
		EmptySlots = slots;

		base.Start ();
	}

    protected void GiveItem(string itemName, ItemContainers itemContainer, int index)
	{
		GameObject tmp = Instantiate (InventoryManager.Instance.itemObject);
		tmp.AddComponent<ItemScript> ();
		ItemScript newItem = tmp.GetComponent<ItemScript> ();

        switch (itemContainer)
        {
            case ItemContainers.CONSUMEABLES:
                newItem.Item = InventoryManager.Instance.ItemContainer.Consumeables[index];
                break;
            case ItemContainers.EQUIPMENT:
                newItem.Item = InventoryManager.Instance.ItemContainer.Equipment[index];
                break;
            case ItemContainers.MATERIALS:
                newItem.Item = InventoryManager.Instance.ItemContainer.Materials[index];
                break;
            case ItemContainers.PLACEABLES:
                newItem.Item = InventoryManager.Instance.ItemContainer.Placeables[index];
                break;
            case ItemContainers.TOOLS:
                newItem.Item = InventoryManager.Instance.ItemContainer.Tools[index];
                break;
            case ItemContainers.WEAPONS:
                newItem.Item = InventoryManager.Instance.ItemContainer.Weapons[index];
                break;
        }

        if (newItem != null)
            AddItem (newItem, false);
		Destroy (tmp);
    }

    public void AddItems(VendorItem[] items)
    {
        RemoveItems();
        for (int i = 0; i < items.Length; i++)
        {
            GiveItem(items[i].item, items[i].itemContainer, items[i].index);
        }
    }

    public void RemoveItems()
    {
        foreach(var slot in allSlots)
        {
            //if (!slot.GetComponent<Slot>().IsEmpty)
                slot.GetComponent<Slot>().ClearSlot();
            EmptySlots = slots;
        }
    }

	public override void MoveItem (GameObject clicked)
	{ }
}
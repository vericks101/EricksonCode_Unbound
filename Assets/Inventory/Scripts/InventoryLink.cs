using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class VendorItem
{
    public string item;
    public ItemContainers itemContainer;
    public int index;
}

public class InventoryLink : MonoBehaviour 
{
    [SerializeField] private VendorItem[] vendorItems;
    private bool itemsAdded = false;
    private bool firstLayout = false;

    public ChestInventory linkedInventory;

    public int rows, slots;
    public List<Stack<ItemScript>> allSlots;
    private bool active = false;

	void Awake()
	{
		if (gameObject.tag == "Vendor") 
		{
			linkedInventory = GameObject.Find ("VendorInventory").GetComponent<VendorInventory> ();
		}
		else if (gameObject.tag == "Chest") 
		{
			linkedInventory = GameObject.Find ("ChestInventory").GetComponent<ChestInventory> ();
		}
	}

    void Start()
    {
        allSlots = new List<Stack<ItemScript>>(slots);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (linkedInventory.FadingOut)
            {
                linkedInventory.InstantClose = true;
                linkedInventory.MoveItemsToChest();
            }

            active = true;
            if (!firstLayout)
                linkedInventory.UpdateLayout(allSlots, rows, slots);
            firstLayout = true;
            if (!itemsAdded && linkedInventory.GetComponent<VendorInventory>() != null)
                linkedInventory.GetComponent<VendorInventory>().AddItems(vendorItems);
            itemsAdded = true;

            if (linkedInventory.GetComponent<ChestInventory>() != null)
            {
                LoadInventory();
                linkedInventory.MoveItemsFromChest();
                linkedInventory.MoveItemsToChest();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            active = false;
            itemsAdded = false;

            if (linkedInventory.GetComponent<ChestInventory>() != null)
                SaveInventory();
        }
    }

    public void SaveInventory()
    {
        if (linkedInventory.IsOpen)
            linkedInventory.MoveItemsToChest();

        CodecManager.Instance.SaveChestData(linkedInventory, allSlots, gameObject, false, null, null);
    }

    public virtual void LoadInventory()
    {
        string content = string.Empty;
        if (linkedInventory.gameObject.name != "VendorInventory")
            content = CodecManager.Instance.LoadChestData(gameObject);

        allSlots = new List<Stack<ItemScript>>();

        for (int i = 0; i < slots; i++)
        {
            allSlots.Add(new Stack<ItemScript>());
        }

        if (content != string.Empty)
        {
            string[] splitContent = content.Split(';');

            for (int x = 0; x < splitContent.Length - 1; x++)
            {
                string[] splitValues = splitContent[x].Split('-');
                int index = Int32.Parse(splitValues[0]);
                string itemName = splitValues[1];
                int amount = Int32.Parse(splitValues[2]);
                Item tmp = null;

                for (int i = 0; i < amount; i++)
                {
                    GameObject loadedItem = Instantiate(InventoryManager.Instance.itemObject);

                    if (tmp == null) tmp = InventoryManager.Instance.ItemContainer.Consumeables.Find(item => item.ItemName == itemName);
                    
                    if (tmp == null) tmp = InventoryManager.Instance.ItemContainer.Equipment.Find(item => item.ItemName == itemName);
                    
                    if (tmp == null) tmp = InventoryManager.Instance.ItemContainer.Weapons.Find(item => item.ItemName == itemName);
                    
                    if (tmp == null) tmp = InventoryManager.Instance.ItemContainer.Materials.Find(item => item.ItemName == itemName);
                    
					if (tmp == null) tmp = InventoryManager.Instance.ItemContainer.Placeables.Find(item => item.ItemName == itemName);

                    loadedItem.AddComponent<ItemScript>();
                    loadedItem.GetComponent<ItemScript>().Item = tmp;

                    allSlots[index].Push(loadedItem.GetComponent<ItemScript>());
                    Destroy(loadedItem);
                }
            }
        }

        if (active)
            linkedInventory.UpdateLayout(allSlots, rows, slots);
        //Debug.Log("loaded: " + content);
    }
}
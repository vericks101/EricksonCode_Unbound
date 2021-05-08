using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventorySelect : Inventory 
{
	public Sprite slotNeutral;
	public Sprite slotHighlighted;

	private int index = 0;
    private Text curItemText;

	protected override void Start()
	{
		base.Start ();
        IsOpen = true;

        if (GetComponent<CanvasGroup>().alpha < 1f)
            GetComponent<CanvasGroup>().alpha = 1f;

        curItemText = transform.Find("CurItemText").GetComponent<Text>();
        curItemText.text = string.Empty;

        InventoryManager.Instance.SelectedSlot = allSlots[0].GetComponent<Slot> ();

		if (InventoryManager.Instance.SelectedSlot.Items.Count > 0) 
		{
			allSlots [index].GetComponent<Image> ().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.spriteHighlighted;
		}
		else 
		{
			allSlots [index].GetComponent<Image> ().sprite = slotHighlighted;
		}

        Player.Instance.inventorySelect.ChangeCurrentItemText();

        if (MainMenuManager.characterCreated)
        {
            MainMenuManager.characterCreated = false;

            CodecManager.Instance.SaveInventories();
        }
    }

	void Update()
	{
        int keyPressed = SelectedSlotWithKey();
        if (keyPressed != -1)
        {
            if (InventoryManager.Instance.SelectedSlot.Items.Count > 0)
            {
                allSlots[index].GetComponent<Image>().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.spriteNeutral;
            }
            else
            {
                allSlots[index].GetComponent<Image>().sprite = slotNeutral;
            }

            index = keyPressed;
            InventoryManager.Instance.SelectedSlot = allSlots[index].GetComponent<Slot>();

            if (InventoryManager.Instance.SelectedSlot.Items.Count > 0)
            {
                allSlots[index].GetComponent<Image>().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.spriteHighlighted;
                ChangeCurrentItemText();
            }
            else
            {
                allSlots[index].GetComponent<Image>().sprite = slotHighlighted;
                ChangeCurrentItemText();
            }
        }

        if (InventoryManager.Instance.SelectedSlot.Items.Count > 0 && allSlots [index].GetComponent<Image> ().sprite != 
			InventoryManager.Instance.SelectedSlot.CurrentItem.spriteHighlighted) 
		{
			allSlots [index].GetComponent<Image> ().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.spriteHighlighted;
		} 
		else if (allSlots [index].GetComponent<Slot> ().Items.Count > 0 && allSlots [index].GetComponent<Image> ().sprite == slotHighlighted) 
		{
			InventoryManager.Instance.SelectedSlot = allSlots [index].GetComponent<Slot> ();
			allSlots [index].GetComponent<Image> ().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.spriteHighlighted;
		}
		else if (InventoryManager.Instance.SelectedSlot.Items.Count == 0)
		{
			allSlots [index].GetComponent<Image> ().sprite = slotHighlighted;
		}

		var wheelDir = Input.GetAxis("Mouse ScrollWheel");
		if (wheelDir < 0f)
		{
			if (index >= 0 && index < allSlots.Count - 1) 
			{
				if (InventoryManager.Instance.SelectedSlot.Items.Count > 0) 
				{
					allSlots [index].GetComponent<Image> ().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.spriteNeutral;
				}
				else 
				{
					allSlots [index].GetComponent<Image> ().sprite = slotNeutral;
				}

				index++;

				InventoryManager.Instance.SelectedSlot = allSlots [index].GetComponent<Slot> ();
				if (InventoryManager.Instance.SelectedSlot.Items.Count > 0) 
				{
					allSlots [index].GetComponent<Image> ().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.spriteHighlighted;
                    ChangeCurrentItemText();
                }
				else 
				{
					allSlots [index].GetComponent<Image> ().sprite = slotHighlighted;
                    ChangeCurrentItemText();
                }
			}
		}
		else if (wheelDir > 0f)
		{
			if (index > 0 && index < allSlots.Count) 
			{
				if (InventoryManager.Instance.SelectedSlot.Items.Count > 0) 
				{
					allSlots [index].GetComponent<Image> ().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.spriteNeutral;
				}
				else 
				{
					allSlots [index].GetComponent<Image> ().sprite = slotNeutral;
				}

				index--;

				InventoryManager.Instance.SelectedSlot = allSlots [index].GetComponent<Slot> ();
				if (InventoryManager.Instance.SelectedSlot.Items.Count > 0) 
				{
					allSlots [index].GetComponent<Image> ().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.spriteHighlighted;
                    ChangeCurrentItemText();
                }
				else 
				{
					allSlots [index].GetComponent<Image> ().sprite = slotHighlighted;
                    ChangeCurrentItemText();
                }
			}
		}
	}

    public void ChangeCurrentItemText()
    {
        if (InventoryManager.Instance.SelectedSlot.Items.Count > 0)
        {
            curItemText.text = InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemName;
        }
        else
        {
            curItemText.text = string.Empty;
        }
    }

    private int SelectedSlotWithKey()
    {
        if (!ChatManager.Instance.chatBoxActive)
        {
            if (Input.GetKeyDown("1"))
            {
                return 0;
            }
            else if (Input.GetKeyDown("2"))
            {
                return 1;
            }
            else if (Input.GetKeyDown("3"))
            {
                return 2;
            }
            else if (Input.GetKeyDown("4"))
            {
                return 3;
            }
            else if (Input.GetKeyDown("5"))
            {
                return 4;
            }
            else if (Input.GetKeyDown("6"))
            {
                return 5;
            }
            else if (Input.GetKeyDown("7"))
            {
                return 6;
            }
            else if (Input.GetKeyDown("8"))
            {
                return 7;
            }
            else if (Input.GetKeyDown("9"))
            {
                return 8;
            }
            else if (Input.GetKeyDown("0"))
            {
                return 9;
            }

            return -1;
        }
        return index;
    }

    private void GiveItem(string itemName, int amount)
    {
        itemName = itemName.Replace("_", " ");
        Item tmp = null;

        for (int i = 0; i < amount; i++)
        {
            GameObject loadedItem = Instantiate(InventoryManager.Instance.itemObject);

            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Consumeables.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Equipment.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Weapons.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Materials.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Placeables.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Tools.Find(item => item.ItemName == itemName);
            }

            if (tmp != null)
            {
                loadedItem.AddComponent<ItemScript>();
                loadedItem.GetComponent<ItemScript>().Item = tmp;
                QuestManager.Instance.itemCollected = loadedItem.GetComponent<ItemScript>().Item.ItemName;
                if (!Player.Instance.inventorySelect.AddItem(loadedItem.GetComponent<ItemScript>(), false))
                    Player.Instance.inventory.AddItem(loadedItem.GetComponent<ItemScript>(), false);
            }
            Destroy(loadedItem);
        }
    }
}
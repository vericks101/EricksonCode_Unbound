using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatCommands : MonoBehaviour
{
    public void CheckCommand(string content, string[] contentArray)
    {
        if (MainMenuManager.developmentMode)
        {
            switch (contentArray[0])
            {
                case "/help":
                    try
                    {
                        ChatManager.Instance.AddChatLine("Commands: give");
                    }
                    catch (FormatException)
                    {
                        ChatManager.Instance.AddChatLine("One or more characters weren't entered correctly.");
                    }
                    break;
                case "/give":
                    try
                    {
                        if (contentArray.Length >= 3)
                            GiveItem(contentArray[1], Int32.Parse(contentArray[2]));
                        else
                        {
                            ChatManager.Instance.AddChatLine("Command wasn't entered correctly.");
                            ChatManager.Instance.AddChatLine("Try: give [Item_Name] [Amount]");
                        }
                    }
                    catch (FormatException)
                    {
                        ChatManager.Instance.AddChatLine("One or more characters weren't entered correctly.");
                    }
                    break;
                default:
                    ChatManager.Instance.AddChatLine("Command doesn't exist.");
                    break;
            }
        }
        else
            ChatManager.Instance.AddChatLine("Commands aren't enabled.");
    }

    private void GiveItem(string itemName, int amount)
    {
        itemName = itemName.Replace("_", " ");
        Item tmp = null;
        bool firstLoop = true;

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
                tmp = InventoryManager.Instance.ItemContainer.Tools.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Placeables.Find(item => item.ItemName == itemName);
            }

            if (tmp != null)
            {
                loadedItem.AddComponent<ItemScript>();
                loadedItem.GetComponent<ItemScript>().Item = tmp;
                QuestManager.Instance.itemCollected = loadedItem.GetComponent<ItemScript>().Item.ItemName;
                if (!Player.Instance.inventorySelect.AddItem(loadedItem.GetComponent<ItemScript>(), false))
                    Player.Instance.inventory.AddItem(loadedItem.GetComponent<ItemScript>(), false);
                if (firstLoop)
                    ChatManager.Instance.AddChatLine(amount + " " + itemName + "(s) added to your inventory.");
                firstLoop = false;
            }
            else
            {
                ChatManager.Instance.AddChatLine(itemName + " isn't a valid item.");
                Destroy(loadedItem);
                Player.Instance.inventorySelect.ChangeCurrentItemText();
                break;
            }
            Destroy(loadedItem);
            Player.Instance.inventorySelect.ChangeCurrentItemText();
        }
    }
}
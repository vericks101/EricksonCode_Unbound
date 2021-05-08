using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCrafting : CraftingBench 
{
    public static Dictionary<string, Item> playerCraftItems = new Dictionary<string, Item>();

    public override void CreateLayout()
    {
        base.CreateLayout();

        RectTransform btnRect = craftBtn.GetComponent<RectTransform>();
        RectTransform slotRect = previewSlot.GetComponent<RectTransform>();

        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth + slotPaddingLeft * 2 + slotSize);

        btnRect.localPosition += new Vector3(-slotSize, slotPaddingTop * 3 + (slotSize * 3));
        slotRect.localPosition += new Vector3(-slotPaddingLeft, slotPaddingTop * 3 + (slotSize * 3));
    }

    public override void CreateBlueprints()
	{
        playerCraftItems.Add("Log-", InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Wood"));
        playerCraftItems.Add("Dark Log-", InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Wood"));
        playerCraftItems.Add("Wood-Wood-Wood-Wood-", InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Crafting Table"));
        playerCraftItems.Add("Cotton-EMPTY-Wood-", InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Torch"));
        playerCraftItems.Add("Cobblestone-EMPTY-Cobblestone-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Cobble Piece"));
        playerCraftItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-", InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Iron Block"));
        playerCraftItems.Add("Iron Bar-EMPTY-EMPTY-Iron Bar-", InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Iron Wall"));
    }

    public override void CraftItem()
    {
        Recipe rec = new Recipe();
        string output = string.Empty;
        bool firstSlot = true;
        int itemCount = 0;
        int maxItems = 0;

        foreach (GameObject slot in allSlots)
        {
            Slot tmp = slot.GetComponent<Slot>();

            if (!tmp.IsEmpty)
                maxItems++;
        }

        foreach (GameObject slot in allSlots)
        {
            Slot tmp = slot.GetComponent<Slot>();

            if (tmp.IsEmpty && !firstSlot && maxItems > itemCount)
                output += "EMPTY-";
            else if (!tmp.IsEmpty)
            {
                output += tmp.CurrentItem.Item.ItemName + "-";
                firstSlot = false;
                itemCount++;
            }
        }
        rec.recipe = output;

        if (playerCraftItems.ContainsKey(rec.recipe))
        {
            GameObject tmpObj = Instantiate(InventoryManager.Instance.itemObject);
            tmpObj.AddComponent<ItemScript>();

            ItemScript craftedItem = tmpObj.GetComponent<ItemScript>();
            Item tmpItem;

            playerCraftItems.TryGetValue(rec.recipe, out tmpItem);

            if (tmpItem != null)
            {
                if (!CraftingPreviewManager.craftingList.ContainsKey(tmpItem.ItemName))
                {
                    int insertIndex = rec.recipe.IndexOf("EMPTY-");
                    if (insertIndex != -1)
                        rec.recipe = rec.recipe.Insert(insertIndex, "EMPTY-EMPTY-EMPTY-");
                    else
                        rec.recipe = rec.recipe.Insert((int)(Mathf.Ceil(rec.recipe.Length / 2)), "EMPTY-EMPTY-EMPTY-");
                    CraftingPreviewManager.Instance.AddRecipe(tmpItem.ItemName, rec.recipe);
                    CraftingBench.remainingRecipes.Remove(rec.recipe);
                }

                bool firstLoop = true;
                for (int i = 0; i < tmpItem.CraftAmount; i++)
                {
                    craftedItem.Item = tmpItem;

                    if (Player.Instance.inventorySelect.AddItem(craftedItem, true) && firstLoop)
                    {
                        foreach (GameObject slot in allSlots)
                        {
                            firstLoop = false;
                            slot.GetComponent<Slot>().RemoveItem();
                        }
                    }
                    else if (firstLoop && Player.Instance.inventory.AddItem(craftedItem, true))
                    {
                        foreach (GameObject slot in allSlots)
                        {
                            firstLoop = false;
                            slot.GetComponent<Slot>().RemoveItem();
                        }
                    }

                    Destroy(tmpObj);
                }
            }
        }

        UpdatePreview();
    }

    public override void UpdatePreview()
    {
        Recipe rec = new Recipe();
        string output = string.Empty;
        bool firstSlot = true;
        int itemCount = 0;
        int maxItems = 0;

        previewSlot.GetComponent<Slot>().ClearSlot();

        foreach (GameObject slot in allSlots)
        {
            Slot tmp = slot.GetComponent<Slot>();

            if (!tmp.IsEmpty)
                maxItems++;
        }

        foreach (GameObject slot in allSlots)
        {
            Slot tmp = slot.GetComponent<Slot>();

            if (tmp.IsEmpty && !firstSlot && maxItems > itemCount)
            {
                output += "EMPTY-";
            }
            else if (!tmp.IsEmpty)
            {
                output += tmp.CurrentItem.Item.ItemName + "-";
                firstSlot = false;
                itemCount++;
            }
        }
        rec.recipe = output;

        if (playerCraftItems.ContainsKey(rec.recipe))
        {
            GameObject tmpObj = Instantiate(InventoryManager.Instance.itemObject);
            tmpObj.AddComponent<ItemScript>();

            ItemScript craftedItem = tmpObj.GetComponent<ItemScript>();
            Item tmpItem;

            playerCraftItems.TryGetValue(rec.recipe, out tmpItem);

            if (tmpItem != null)
            {
                for (int i = 0; i < tmpItem.CraftAmount; i++)
                {
                    craftedItem.Item = tmpItem;
                    previewSlot.GetComponent<Slot>().AddItem(craftedItem);

                    Destroy(tmpObj);
                }
            }
        }
    }
}
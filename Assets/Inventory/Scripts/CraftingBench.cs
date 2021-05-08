using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets._2D;

public class Recipe
{
    public string recipe;
}

public class CraftingBench : Inventory
{
	[SerializeField] protected GameObject prefabButton;
    protected GameObject craftBtn;
    protected GameObject mmcBtn;
	public static Dictionary<string, Item> craftingItems = new Dictionary<string, Item> ();
    public static Dictionary<string, Item> remainingRecipes = new Dictionary<string, Item>();
    protected GameObject previewSlot;
    protected GameObject mmcSlot;

	private static CraftingBench instance;
    public static CraftingBench Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("CraftingBench").GetComponent<CraftingBench>();

            return instance;
        }
    }

    public override void CreateLayout()
    {
        base.CreateLayout();

        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHight + slotSize + slotPaddingTop * 2);

        craftBtn = Instantiate(prefabButton);

        RectTransform btnRect = craftBtn.GetComponent<RectTransform>();

        craftBtn.name = "CraftButton";
        craftBtn.transform.SetParent(this.transform.parent);

        btnRect.localPosition = inventoryRect.localPosition + new Vector3(slotPaddingLeft, -slotPaddingTop * 5 - (slotSize * 5));
        btnRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((slotSize * 2) + (slotPaddingLeft * 2)) * InventoryManager.Instance.canvas.scaleFactor);
        btnRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * InventoryManager.Instance.canvas.scaleFactor);

        craftBtn.transform.SetParent(transform);
        craftBtn.GetComponent<Button>().onClick.AddListener(CraftItem);

        previewSlot = Instantiate(InventoryManager.Instance.slotPrefab);

        RectTransform slotRect = previewSlot.GetComponent<RectTransform>();

        previewSlot.name = "PreviewSlot";
        previewSlot.transform.SetParent(this.transform.parent);

        slotRect.localPosition = inventoryRect.localPosition + new Vector3((slotPaddingLeft * 3) + (slotSize * 2), -slotPaddingTop * 5 - (slotSize * 5));
        slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize * InventoryManager.Instance.canvas.scaleFactor);
        slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * InventoryManager.Instance.canvas.scaleFactor);

        previewSlot.transform.SetParent(this.transform);
        previewSlot.GetComponent<Slot>().clickAble = false;
        previewSlot.GetComponent<CanvasGroup>().interactable = false;
    }

    public virtual void CreateBlueprints()
    {
        craftingItems.Add("Log-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Wood"));
        craftingItems.Add("Dark Log-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Wood"));
        craftingItems.Add("Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-Wood-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Crafting Table"));
        craftingItems.Add("Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Iron Bar-Wood-Wood-Wood-Wood-Iron Bar-Wood-Wood-Wood-Wood-Wood-Wood-Wood-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Chest"));
        craftingItems.Add("Cobblestone-Cobblestone-Cobblestone-Cobblestone-Cobblestone-Cobblestone-Cobblestone-EMPTY-Cobblestone-Cobblestone-Cobblestone-EMPTY-EMPTY-EMPTY-Cobblestone-Cobblestone-Torch-Torch-Torch-Cobblestone-Cobblestone-Cobblestone-Cobblestone-Cobblestone-Cobblestone-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Smelting Bench"));
        craftingItems.Add("Cotton-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Torch"));

        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-EMPTY-Copper Bar-Copper Bar-EMPTY-EMPTY-EMPTY-Copper Bar-Copper Bar-EMPTY-EMPTY-EMPTY-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Nanohelmet"));
        craftingItems.Add("Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-EMPTY-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Nanoshoulders"));
        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-Copper Bar-Copper Bar-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Nanochestplate"));
        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Nanoleggings"));
        craftingItems.Add("Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Nanoboots"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Nanohelmet"));
        craftingItems.Add("Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Nanoshoulders"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-Iron Bar-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Nanochestplate"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Nanoleggings"));
        craftingItems.Add("Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Nanoboots"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-EMPTY-Gold Bar-Gold Bar-EMPTY-EMPTY-EMPTY-Gold Bar-Gold Bar-EMPTY-EMPTY-EMPTY-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Nanohelmet"));
        craftingItems.Add("Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-EMPTY-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Nanoshoulders"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-Gold Bar-Gold Bar-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Nanochestplate"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Nanoleggings"));
        craftingItems.Add("Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Nanoboots"));
        craftingItems.Add("EMPTY-EMPTY-Copper Bar-EMPTY-EMPTY-EMPTY-EMPTY-Copper Bar-EMPTY-EMPTY-EMPTY-Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Copper Nanosaber"));
        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Wood-Copper Bar-Copper Bar-Copper Bar-EMPTY-Wood-EMPTY-Copper Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Copper Pickaxe"));
        craftingItems.Add("Copper Bar-EMPTY-EMPTY-EMPTY-EMPTY-Copper Bar-Copper Bar-Wood-EMPTY-EMPTY-Copper Bar-Copper Bar-Wood-EMPTY-EMPTY-Copper Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Copper Axe"));
        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Wood-Copper Bar-Copper Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Copper Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Iron Bar-EMPTY-EMPTY-EMPTY-EMPTY-Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Iron Nanosaber"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Wood-Iron Bar-Iron Bar-Iron Bar-EMPTY-Wood-EMPTY-Iron Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Iron Pickaxe"));
        craftingItems.Add("Iron Bar-EMPTY-EMPTY-EMPTY-EMPTY-Iron Bar-Iron Bar-Wood-EMPTY-EMPTY-Iron Bar-Iron Bar-Wood-EMPTY-EMPTY-Iron Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Iron Axe"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Wood-Iron Bar-Iron Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Iron Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Gold Bar-EMPTY-EMPTY-EMPTY-EMPTY-Gold Bar-EMPTY-EMPTY-EMPTY-Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Gold Nanosaber"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Wood-Gold Bar-Gold Bar-Gold Bar-EMPTY-Wood-EMPTY-Gold Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Gold Pickaxe"));
        craftingItems.Add("Gold Bar-EMPTY-EMPTY-EMPTY-EMPTY-Gold Bar-Gold Bar-Wood-EMPTY-EMPTY-Gold Bar-Gold Bar-Wood-EMPTY-EMPTY-Gold Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Gold Axe"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Wood-Gold Bar-Gold Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Gold Hammer"));
        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-EMPTY-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Copper Nanoshield"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Iron Nanoshield"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-EMPTY-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Gold Nanoshield"));
        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-Copper Bar-EMPTY-Copper Bar-EMPTY-EMPTY-Copper Bar-EMPTY-Copper Bar-EMPTY-EMPTY-Copper Bar-Copper Bar-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Neckpiece"));
        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Nanobelt"));
        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-Copper Bar-EMPTY-Copper Bar-EMPTY-EMPTY-Copper Bar-Copper Bar-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Wristpiece"));
        craftingItems.Add("Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Nanobracers"));
        craftingItems.Add("Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-Copper Bar-EMPTY-Copper Bar-Copper Bar-Copper Bar-EMPTY-EMPTY-EMPTY-Copper Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Copper Anklepieces"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-Iron Bar-EMPTY-Iron Bar-EMPTY-EMPTY-Iron Bar-EMPTY-Iron Bar-EMPTY-EMPTY-Iron Bar-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Neckpiece"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Nanobelt"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-Iron Bar-EMPTY-Iron Bar-EMPTY-EMPTY-Iron Bar-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Wristpiece"));
        craftingItems.Add("Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Nanobracers"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Iron Anklepieces"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-Gold Bar-EMPTY-Gold Bar-EMPTY-EMPTY-Gold Bar-EMPTY-Gold Bar-EMPTY-EMPTY-Gold Bar-Gold Bar-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Neckpiece"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Nanobelt"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-Gold Bar-EMPTY-Gold Bar-EMPTY-EMPTY-Gold Bar-Gold Bar-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Wristpiece"));
        craftingItems.Add("Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Nanobracers"));
        craftingItems.Add("Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-Gold Bar-EMPTY-Gold Bar-Gold Bar-Gold Bar-EMPTY-EMPTY-EMPTY-Gold Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Gold Anklepieces"));
        craftingItems.Add("EMPTY-EMPTY-Stone-EMPTY-EMPTY-EMPTY-Stone-Cobble Piece-Stone-EMPTY-Stone-Cobble Piece-Stone-Cobble Piece-Stone-EMPTY-Stone-Cobble Piece-Stone-EMPTY-EMPTY-EMPTY-Stone-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ground Of Silvis"));
        craftingItems.Add("EMPTY-EMPTY-Stone-EMPTY-EMPTY-EMPTY-Stone-Copper Bar-Stone-EMPTY-Stone-Copper Bar-Iron Bar-Copper Bar-Stone-EMPTY-Stone-Copper Bar-Stone-EMPTY-EMPTY-EMPTY-Stone-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ores Of Silvis"));
        craftingItems.Add("EMPTY-EMPTY-Stone-EMPTY-EMPTY-EMPTY-Stone-Brown Mushroom-Stone-EMPTY-Stone-Red Flower-Brush-Yellow Flower-Stone-EMPTY-Stone-Cotton-Stone-EMPTY-EMPTY-EMPTY-Stone-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Nature Of Silvis"));
        craftingItems.Add("EMPTY-EMPTY-Stone-EMPTY-EMPTY-EMPTY-Stone-Ground Of Silvis-Stone-EMPTY-Stone-Gold Bar-Ores Of Silvis-Gold Bar-Stone-EMPTY-Stone-Nature Of Silvis-Stone-EMPTY-EMPTY-EMPTY-Stone-", 
            InventoryManager.Instance.ItemContainer.Consumeables.Find(x => x.ItemName == "Essence Of Green"));

        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-EMPTY-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Nanohelmet"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-EMPTY-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Nanoshoulders"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Nanochestplate"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Nanoleggings"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Nanoboots"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-EMPTY-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Nanohelmet"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-EMPTY-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Nanoshoulders"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Nanochestplate"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Nanoleggings"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Nanoboots"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-EMPTY-Titanium Bar-Titanium Bar-EMPTY-EMPTY-EMPTY-Titanium Bar-Titanium Bar-EMPTY-EMPTY-EMPTY-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Nanohelmet"));
        craftingItems.Add("Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-EMPTY-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Nanoshoulders"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Nanochestplate"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Nanoleggings"));
        craftingItems.Add("Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Nanoboots"));
        craftingItems.Add("EMPTY-EMPTY-Zirconin Bar-EMPTY-EMPTY-EMPTY-EMPTY-Zirconin Bar-EMPTY-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Zirconin Nanosaber"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Wood-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Wood-EMPTY-Zirconin Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Zirconin Pickaxe"));
        craftingItems.Add("Zirconin Bar-EMPTY-EMPTY-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-Wood-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-Wood-EMPTY-EMPTY-Zirconin Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Zirconin Axe"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Wood-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Zirconin Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Tungsten Bar-EMPTY-EMPTY-EMPTY-EMPTY-Tungsten Bar-EMPTY-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Tungsten Nanosaber"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Wood-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Wood-EMPTY-Tungsten Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Tungsten Pickaxe"));
        craftingItems.Add("Tungsten Bar-EMPTY-EMPTY-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-Wood-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-Wood-EMPTY-EMPTY-Tungsten Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Tungsten Axe"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Wood-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Tungsten Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Titanium Bar-EMPTY-EMPTY-EMPTY-EMPTY-Titanium Bar-EMPTY-EMPTY-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Titanium Nanosaber"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Wood-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Wood-EMPTY-Titanium Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Titanium Pickaxe"));
        craftingItems.Add("Titanium Bar-EMPTY-EMPTY-EMPTY-EMPTY-Titanium Bar-Titanium Bar-Wood-EMPTY-EMPTY-Titanium Bar-Titanium Bar-Wood-EMPTY-EMPTY-Titanium Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Titanium Axe"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Wood-Titanium Bar-Titanium Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Titanium Hammer"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-EMPTY-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Zirconin Nanoshield"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-EMPTY-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Tungsten Nanoshield"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-EMPTY-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Titanium Nanoshield"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-Zirconin Bar-EMPTY-Zirconin Bar-EMPTY-EMPTY-Zirconin Bar-EMPTY-Zirconin Bar-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Neckpiece"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Nanobelt"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-Zirconin Bar-EMPTY-Zirconin Bar-EMPTY-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Wristpiece"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Nanobracers"));
        craftingItems.Add("Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-Zirconin Bar-Zirconin Bar-Zirconin Bar-EMPTY-EMPTY-EMPTY-Zirconin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Zirconin Anklepieces"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-Tungsten Bar-EMPTY-Tungsten Bar-EMPTY-EMPTY-Tungsten Bar-EMPTY-Tungsten Bar-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Neckpiece"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Nanobelt"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-Tungsten Bar-EMPTY-Tungsten Bar-EMPTY-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Wristpiece"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Nanobracers"));
        craftingItems.Add("Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-Tungsten Bar-Tungsten Bar-Tungsten Bar-EMPTY-EMPTY-EMPTY-Tungsten Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tungsten Anklepieces"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-Titanium Bar-EMPTY-Titanium Bar-EMPTY-EMPTY-Titanium Bar-EMPTY-Titanium Bar-EMPTY-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Neckpiece"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Nanobelt"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-Titanium Bar-EMPTY-Titanium Bar-EMPTY-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Wristpiece"));
        craftingItems.Add("Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Nanobracers"));
        craftingItems.Add("Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-Titanium Bar-Titanium Bar-Titanium Bar-EMPTY-EMPTY-EMPTY-Titanium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Titanium Anklepieces"));
        craftingItems.Add("EMPTY-EMPTY-Sandstone-EMPTY-EMPTY-EMPTY-Sandstone-Sand-Sandstone-EMPTY-Sandstone-Sand-Sandstone-Sand-Sandstone-EMPTY-Sandstone-Sand-Sandstone-EMPTY-EMPTY-EMPTY-Sandstone-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ground Of Eugmus"));
        craftingItems.Add("EMPTY-EMPTY-Sandstone-EMPTY-EMPTY-EMPTY-Sandstone-Zirconin Bar-Sandstone-EMPTY-Sandstone-Zirconin Bar-Tungsten Bar-Zirconin Bar-Stone-EMPTY-Sandstone-Zirconin Bar-Sandstone-EMPTY-EMPTY-EMPTY-Sandstone-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ores Of Eugmus"));
        craftingItems.Add("EMPTY-EMPTY-Sandstone-EMPTY-EMPTY-EMPTY-Sandstone-Desert Bush-Sandstone-EMPTY-Sandstone-Saguaro Cactus-Brush-Opuntia Cactus-Sandstone-EMPTY-Sandstone-Desert Brush-Sandstone-EMPTY-EMPTY-EMPTY-Sandstone-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Nature Of Eugmus"));
        craftingItems.Add("EMPTY-EMPTY-Sandstone-EMPTY-EMPTY-EMPTY-Sandstone-Ground Of Eugmus-Sandstone-EMPTY-Sandstone-Titanium Bar-Ores Of Eugmus-Titanium Bar-Sandstone-EMPTY-Sandstone-Nature Of Eugmus-Sandstone-EMPTY-EMPTY-EMPTY-Sandstone-",
            InventoryManager.Instance.ItemContainer.Consumeables.Find(x => x.ItemName == "Essence Of Ground"));

        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-EMPTY-Lead Bar-Lead Bar-EMPTY-EMPTY-EMPTY-Lead Bar-Lead Bar-EMPTY-EMPTY-EMPTY-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Nanohelmet"));
        craftingItems.Add("Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-EMPTY-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Nanoshoulders"));
        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-Lead Bar-Lead Bar-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Nanochestplate"));
        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Nanoleggings"));
        craftingItems.Add("Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Nanoboots"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-EMPTY-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Nanohelmet"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-EMPTY-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Nanoshoulders"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Nanochestplate"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Nanoleggings"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Nanoboots"));
        craftingItems.Add("Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-EMPTY-EMPTY-EMPTY-Diamond-Diamond-EMPTY-EMPTY-EMPTY-Diamond-Diamond-EMPTY-EMPTY-EMPTY-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Nanohelmet"));
        craftingItems.Add("Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-EMPTY-EMPTY-EMPTY-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Nanoshoulders"));
        craftingItems.Add("Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-EMPTY-EMPTY-Diamond-Diamond-Diamond-EMPTY-EMPTY-Diamond-Diamond-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Nanochestplate"));
        craftingItems.Add("Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Nanoleggings"));
        craftingItems.Add("Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Nanoboots"));
        craftingItems.Add("EMPTY-EMPTY-Lead Bar-EMPTY-EMPTY-EMPTY-EMPTY-Lead Bar-EMPTY-EMPTY-EMPTY-Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Lead Nanosaber"));
        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Wood-Lead Bar-Lead Bar-Lead Bar-EMPTY-Wood-EMPTY-Lead Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Lead Pickaxe"));
        craftingItems.Add("Lead Bar-EMPTY-EMPTY-EMPTY-EMPTY-Lead Bar-Lead Bar-Wood-EMPTY-EMPTY-Lead Bar-Lead Bar-Wood-EMPTY-EMPTY-Lead Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Lead Axe"));
        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Wood-Lead Bar-Lead Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Lead Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Bauxum Bar-EMPTY-EMPTY-EMPTY-EMPTY-Bauxum Bar-EMPTY-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Bauxum Nanosaber"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Wood-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Wood-EMPTY-Bauxum Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Bauxum Pickaxe"));
        craftingItems.Add("Bauxum Bar-EMPTY-EMPTY-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-Wood-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-Wood-EMPTY-EMPTY-Bauxum Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Bauxum Axe"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Wood-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Bauxum Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Diamond-EMPTY-EMPTY-EMPTY-EMPTY-Diamond-EMPTY-EMPTY-EMPTY-Diamond-Diamond-Diamond-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Diamond Nanosaber"));
        craftingItems.Add("Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Wood-Diamond-Diamond-Diamond-EMPTY-Wood-EMPTY-Diamond-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood--",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Diamond Pickaxe"));
        craftingItems.Add("Diamond-EMPTY-EMPTY-EMPTY-EMPTY-Diamond-Diamond-Wood-EMPTY-EMPTY-Diamond-Diamond-Wood-EMPTY-EMPTY-Diamond-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Diamond Axe"));
        craftingItems.Add("Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Wood-Diamond-Diamond-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Diamond Hammer"));
        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-EMPTY-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Lead Nanoshield"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-EMPTY-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Bauxum Nanoshield"));
        craftingItems.Add("Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-EMPTY-EMPTY-Diamond-Diamond-Diamond-EMPTY-EMPTY-EMPTY-Diamond-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Diamond Nanoshield"));
        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-Lead Bar-EMPTY-Lead Bar-EMPTY-EMPTY-Lead Bar-EMPTY-Lead Bar-EMPTY-EMPTY-Lead Bar-Lead Bar-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Neckpiece"));
        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Nanobelt"));
        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-Lead Bar-EMPTY-Lead Bar-EMPTY-EMPTY-Lead Bar-Lead Bar-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Wristpiece"));
        craftingItems.Add("Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Nanobracers"));
        craftingItems.Add("Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-Lead Bar-EMPTY-Lead Bar-Lead Bar-Lead Bar-EMPTY-EMPTY-EMPTY-Lead Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Lead Anklepieces"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-Bauxum Bar-EMPTY-Bauxum Bar-EMPTY-EMPTY-Bauxum Bar-EMPTY-Bauxum Bar-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Neckpiece"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Nanobelt"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-Bauxum Bar-EMPTY-Bauxum Bar-EMPTY-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Wristpiece"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Nanobracers"));
        craftingItems.Add("Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-Bauxum Bar-Bauxum Bar-Bauxum Bar-EMPTY-EMPTY-EMPTY-Bauxum Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Bauxum Anklepieces"));
        craftingItems.Add("Diamond-Diamond-Diamond-EMPTY-EMPTY-Diamond-EMPTY-Diamond-EMPTY-EMPTY-Diamond-EMPTY-Diamond-EMPTY-EMPTY-Diamond-Diamond-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Neckpiece"));
        craftingItems.Add("Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Nanobelt"));
        craftingItems.Add("Diamond-Diamond-Diamond-EMPTY-EMPTY-Diamond-EMPTY-Diamond-EMPTY-EMPTY-Diamond-Diamond-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Wristpiece"));
        craftingItems.Add("Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Nanobracers"));
        craftingItems.Add("Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-Diamond-EMPTY-Diamond-Diamond-Diamond-EMPTY-EMPTY-EMPTY-Diamond-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Diamond Anklepieces"));
        craftingItems.Add("EMPTY-EMPTY-Ash Block-EMPTY-EMPTY-EMPTY-Ash Block-Ash Block-Ash Block-EMPTY-Ash Block-Ash Block-Ash Block-Ash Block-Ash Block-EMPTY-Ash Block-Ash Block-Ash Block-EMPTY-EMPTY-EMPTY-Ash Block-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ground Of Aestrus"));
        craftingItems.Add("EMPTY-EMPTY-Ash Block-EMPTY-EMPTY-EMPTY-Ash Block-Lead Bar-Ash Block-EMPTY-Ash Block-Lead Bar-Bauxum Bar-Lead Bar-Ash Block-EMPTY-Ash Block-Lead Bar-Ash Block-EMPTY-EMPTY-EMPTY-Ash Block-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ores Of Aestrus"));
        craftingItems.Add("EMPTY-EMPTY-Ash Block-EMPTY-EMPTY-EMPTY-Ash Block-Fire Flower-Ash Block-EMPTY-Ash Block-Fire Flower-EMPTY-Fire Flower-Ash Block-EMPTY-Ash Block-Fire Flower-Ash Block-EMPTY-EMPTY-EMPTY-Ash Block-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Nature Of Aestrus"));
        craftingItems.Add("EMPTY-EMPTY-Ash Block-EMPTY-EMPTY-EMPTY-Ash Block-Ground Of Aestrus-Ash Block-EMPTY-Ash Block-Diamond-Ores Of Aestrus-Diamond-Ash Block-EMPTY-Ash Block-Nature Of Aestrus-Ash Block-EMPTY-EMPTY-EMPTY-Ash Block-",
            InventoryManager.Instance.ItemContainer.Consumeables.Find(x => x.ItemName == "Essence Of Fire"));

        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-EMPTY-Tin Bar-Tin Bar-EMPTY-EMPTY-EMPTY-Tin Bar-Tin Bar-EMPTY-EMPTY-EMPTY-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Nanohelmet"));
        craftingItems.Add("Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-EMPTY-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Nanoshoulders"));
        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-Tin Bar-Tin Bar-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Nanochestplate"));
        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Nanoleggings"));
        craftingItems.Add("Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Nanoboots"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-EMPTY-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Nanohelmet"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-EMPTY-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Nanoshoulders"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Nanochestplate"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Nanoleggings"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Nanoboots"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-EMPTY-Onyx Bar-Onyx Bar-EMPTY-EMPTY-EMPTY-Onyx Bar-Onyx Bar-EMPTY-EMPTY-EMPTY-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Nanohelmet"));
        craftingItems.Add("Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-EMPTY-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Nanoshoulders"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Nanochestplate"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Nanoleggings"));
        craftingItems.Add("Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Nanoboots"));
        craftingItems.Add("EMPTY-EMPTY-Tin Bar-EMPTY-EMPTY-EMPTY-EMPTY-Tin Bar-EMPTY-EMPTY-EMPTY-Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Tin Nanosaber"));
        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Wood-Tin Bar-Tin Bar-Tin Bar-EMPTY-Wood-EMPTY-Tin Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Tin Pickaxe"));
        craftingItems.Add("Tin Bar-EMPTY-EMPTY-EMPTY-EMPTY-Tin Bar-Tin Bar-Wood-EMPTY-EMPTY-Tin Bar-Tin Bar-Wood-EMPTY-EMPTY-Tin Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Tin Axe"));
        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Wood-Tin Bar-Tin Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Tin Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Thyrium Bar-EMPTY-EMPTY-EMPTY-EMPTY-Thyrium Bar-EMPTY-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Thyrium Nanosaber"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Wood-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Wood-EMPTY-Thyrium Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Thyrium Pickaxe"));
        craftingItems.Add("Thyrium Bar-EMPTY-EMPTY-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-Wood-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-Wood-EMPTY-EMPTY-Thyrium Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Thyrium Axe"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Wood-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Thyrium Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Onyx Bar-EMPTY-EMPTY-EMPTY-EMPTY-Onyx Bar-EMPTY-EMPTY-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Onyx Nanosaber"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Wood-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Wood-EMPTY-Onyx Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Onyx Pickaxe"));
        craftingItems.Add("Onyx Bar-EMPTY-EMPTY-EMPTY-EMPTY-Onyx Bar-Onyx Bar-Wood-EMPTY-EMPTY-Onyx Bar-Onyx Bar-Wood-EMPTY-EMPTY-Onyx Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Onyx Axe"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Wood-Onyx Bar-Onyx Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Onyx Hammer"));
        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-EMPTY-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Tin Nanoshield"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-EMPTY-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Thyrium Nanoshield"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-EMPTY-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Onyx Nanoshield"));
        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-Tin Bar-EMPTY-Tin Bar-EMPTY-EMPTY-Tin Bar-EMPTY-Tin Bar-EMPTY-EMPTY-Tin Bar-Tin Bar-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Neckpiece"));
        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Nanobelt"));
        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-Tin Bar-EMPTY-Tin Bar-EMPTY-EMPTY-Tin Bar-Tin Bar-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Wristpiece"));
        craftingItems.Add("Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Nanobracers"));
        craftingItems.Add("Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-Tin Bar-EMPTY-Tin Bar-Tin Bar-Tin Bar-EMPTY-EMPTY-EMPTY-Tin Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Tin Anklepieces"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-Thyrium Bar-EMPTY-Thyrium Bar-EMPTY-EMPTY-Thyrium Bar-EMPTY-Thyrium Bar-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Neckpiece"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Nanobelt"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-Thyrium Bar-EMPTY-Thyrium Bar-EMPTY-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Wristpiece"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Nanobracers"));
        craftingItems.Add("Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-Thyrium Bar-Thyrium Bar-Thyrium Bar-EMPTY-EMPTY-EMPTY-Thyrium Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Thyrium Anklepieces"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-Onyx Bar-EMPTY-Onyx Bar-EMPTY-EMPTY-Onyx Bar-EMPTY-Onyx Bar-EMPTY-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Neckpiece"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Nanobelt"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-Onyx Bar-EMPTY-Onyx Bar-EMPTY-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Wristpiece"));
        craftingItems.Add("Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Nanobracers"));
        craftingItems.Add("Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-Onyx Bar-Onyx Bar-Onyx Bar-EMPTY-EMPTY-EMPTY-Onyx Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Onyx Anklepieces"));
        craftingItems.Add("EMPTY-EMPTY-Snow Block-EMPTY-EMPTY-EMPTY-Snow Block-Slush Block-Snow Block-EMPTY-Snow Block-Slush Block-Snow Block-Slush Block-Snow Block-EMPTY-Snow Block-Slush Block-Snow Block-EMPTY-EMPTY-EMPTY-Snow Block-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ground Of Sontra"));
        craftingItems.Add("EMPTY-EMPTY-Snow Block-EMPTY-EMPTY-EMPTY-Snow Block-Tin Bar-Snow Block-EMPTY-Snow Block-Tin Bar-Thyrium Bar-Tin Bar-Snow Block-EMPTY-Snow Block-Tin Bar-Snow Block-EMPTY-EMPTY-EMPTY-Snow Block-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ores Of Sontra"));
        craftingItems.Add("EMPTY-EMPTY-Snow Block-EMPTY-EMPTY-EMPTY-Snow Block-Crocuses-Snow Block-EMPTY-Snow Block-Sarcodes-Brush-Snowdrops-Snow Block-EMPTY-Snow Block-Tulips-Snow Block-EMPTY-EMPTY-EMPTY-Snow Block-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Nature Of Sontra"));
        craftingItems.Add("EMPTY-EMPTY-Snow Block-EMPTY-EMPTY-EMPTY-Snow Block-Ground Of Sontra-Snow Block-EMPTY-Snow Block-Onyx Bar-Ores Of Sontra-Onyx Bar-Snow Block-EMPTY-Snow Block-Nature Of Sontra-Snow Block-EMPTY-EMPTY-EMPTY-Snow Block-",
            InventoryManager.Instance.ItemContainer.Consumeables.Find(x => x.ItemName == "Essence Of Ice"));

        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-EMPTY-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Nanohelmet"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-EMPTY-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Nanoshoulders"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Nanochestplate"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Nanoleggings"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Nanoboots"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-EMPTY-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Nanohelmet"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-EMPTY-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Nanoshoulders"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Nanochestplate"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Nanoleggings"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Nanoboots"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-EMPTY-Manganese Bar-Manganese Bar-EMPTY-EMPTY-EMPTY-Manganese Bar-Manganese Bar-EMPTY-EMPTY-EMPTY-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Nanohelmet"));
        craftingItems.Add("Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-EMPTY-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Nanoshoulders"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Nanochestplate"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Nanoleggings"));
        craftingItems.Add("Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Nanoboots"));
        craftingItems.Add("EMPTY-EMPTY-Cobalt Bar-EMPTY-EMPTY-EMPTY-EMPTY-Cobalt Bar-EMPTY-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Cobalt Nanosaber"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Wood-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Wood-EMPTY-Cobalt Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Cobalt Pickaxe"));
        craftingItems.Add("Cobalt Bar-EMPTY-EMPTY-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-Wood-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-Wood-EMPTY-EMPTY-Cobalt Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Cobalt Axe"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Wood-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Cobalt Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Sphalerite Bar-EMPTY-EMPTY-EMPTY-EMPTY-Sphalerite Bar-EMPTY-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Sphalerite Nanosaber"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Wood-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Wood-EMPTY-Sphalerite Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Sphalerite Pickaxe"));
        craftingItems.Add("Sphalerite Bar-EMPTY-EMPTY-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-Wood-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-Wood-EMPTY-EMPTY-Sphalerite Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Sphalerite Axe"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Wood-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Sphalerite Hammer"));
        craftingItems.Add("EMPTY-EMPTY-Manganese Bar-EMPTY-EMPTY-EMPTY-EMPTY-Manganese Bar-EMPTY-EMPTY-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Manganese Nanosaber"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Wood-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Wood-EMPTY-Manganese Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Manganese Pickaxe"));
        craftingItems.Add("Manganese Bar-EMPTY-EMPTY-EMPTY-EMPTY-Manganese Bar-Manganese Bar-Wood-EMPTY-EMPTY-Manganese Bar-Manganese Bar-Wood-EMPTY-EMPTY-Manganese Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Manganese Axe"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Wood-Manganese Bar-Manganese Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Manganese Hammer"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-EMPTY-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Cobalt Nanoshield"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-EMPTY-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Sphalerite Nanoshield"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-EMPTY-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Manganese Nanoshield"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-Cobalt Bar-EMPTY-Cobalt Bar-EMPTY-EMPTY-Cobalt Bar-EMPTY-Cobalt Bar-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Neckpiece"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Nanobelt"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-Cobalt Bar-EMPTY-Cobalt Bar-EMPTY-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Wristpiece"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Nanobracers"));
        craftingItems.Add("Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-Cobalt Bar-Cobalt Bar-Cobalt Bar-EMPTY-EMPTY-EMPTY-Cobalt Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Cobalt Anklepieces"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-Sphalerite Bar-EMPTY-Sphalerite Bar-EMPTY-EMPTY-Sphalerite Bar-EMPTY-Sphalerite Bar-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Neckpiece"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Nanobelt"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-Sphalerite Bar-EMPTY-Sphalerite Bar-EMPTY-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Wristpiece"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Nanobracers"));
        craftingItems.Add("Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-Sphalerite Bar-Sphalerite Bar-Sphalerite Bar-EMPTY-EMPTY-EMPTY-Sphalerite Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Sphalerite Anklepieces"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-Manganese Bar-EMPTY-Manganese Bar-EMPTY-EMPTY-Manganese Bar-EMPTY-Manganese Bar-EMPTY-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Neckpiece"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Nanobelt"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-Manganese Bar-EMPTY-Manganese Bar-EMPTY-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Wristpiece"));
        craftingItems.Add("Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Nanobracers"));
        craftingItems.Add("Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-Manganese Bar-Manganese Bar-Manganese Bar-EMPTY-EMPTY-EMPTY-Manganese Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Manganese Anklepieces"));
        craftingItems.Add("EMPTY-EMPTY-Sand Block-EMPTY-EMPTY-EMPTY-Sand Block-Sandstone-Sand Block-EMPTY-Sand Block-Sandstone-Sand Block-Sandstone-Sand Block-EMPTY-Sand Block-Sandstone-Sand Block-EMPTY-EMPTY-EMPTY-Sand Block-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ground Of Palgar"));
        craftingItems.Add("EMPTY-EMPTY-Sand Block-EMPTY-EMPTY-EMPTY-Sand Block-Cobalt Bar-Sand Block-EMPTY-Sand Block-Cobalt Bar-Sphalerite Bar-Cobalt Bar-Sand Block-EMPTY-Sand Block-Cobalt Bar-Sand Block-EMPTY-EMPTY-EMPTY-Sand Block-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Ores Of Palgar"));
        craftingItems.Add("EMPTY-EMPTY-Sand Block-EMPTY-EMPTY-EMPTY-Sand Block-Crocuses-Sand Block-EMPTY-Sand Block-Coral-Kelp-Red Algae-Sand Block-EMPTY-Sand Block-Seaweed-Sand Block-EMPTY-EMPTY-EMPTY-Sand Block-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Nature Of Palgar"));
        craftingItems.Add("EMPTY-EMPTY-Sand Block-EMPTY-EMPTY-EMPTY-Sand Block-Ground Of Palgar-Sand Block-EMPTY-Sand Block-Manganese Bar-Ores Of Palgar-Manganese Bar-Sand Block-EMPTY-Sand Block-Nature Of Palgar-Sand Block-EMPTY-EMPTY-EMPTY-Sand Block-",
            InventoryManager.Instance.ItemContainer.Consumeables.Find(x => x.ItemName == "Essence Of Water"));

        craftingItems.Add("Stone-Stone-Stone-Stone-Stone-Stone-Gold Bar-Stone-Titanium Bar-Stone-Stone-Stone-Diamond-Stone-Stone-Stone-Onyx Bar-Stone-Manganese Bar-Stone-Stone-Stone-Stone-Stone-Stone-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Luminance Ore"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-EMPTY-Luminance Bar-Luminance Bar-EMPTY-EMPTY-EMPTY-Luminance Bar-Luminance Bar-EMPTY-EMPTY-EMPTY-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Nanohelmet"));
        craftingItems.Add("Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-EMPTY-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Nanoshoulders"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Nanochestplate"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Nanoleggings"));
        craftingItems.Add("Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Nanoboots"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-Luminance Bar-EMPTY-Luminance Bar-EMPTY-EMPTY-Luminance Bar-EMPTY-Luminance Bar-EMPTY-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Neckpiece"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Nanobelt"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-Luminance Bar-EMPTY-Luminance Bar-EMPTY-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Wristpiece"));
        craftingItems.Add("Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Nanobracers"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-EMPTY-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Luminance Anklepieces"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-EMPTY-Luminance Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Luminance Nanoshield"));
        craftingItems.Add("EMPTY-EMPTY-Luminance Bar-EMPTY-EMPTY-EMPTY-EMPTY-Luminance Bar-EMPTY-EMPTY-EMPTY-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Luminance Nanosaber"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Wood-Luminance Bar-Luminance Bar-Luminance Bar-EMPTY-Wood-EMPTY-Luminance Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Luminance Pickaxe"));
        craftingItems.Add("Luminance Bar-EMPTY-EMPTY-EMPTY-EMPTY-Luminance Bar-Luminance Bar-Wood-EMPTY-EMPTY-Luminance Bar-Luminance Bar-Wood-EMPTY-EMPTY-Luminance Bar-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Luminance Axe"));
        craftingItems.Add("Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Luminance Bar-Wood-Luminance Bar-Luminance Bar-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Luminance Hammer"));
        craftingItems.Add("Green Energy Cell-Ground Energy Cell-Warm Energy Cell-Cold Energy Cell-Wet Energy Cell-",
            InventoryManager.Instance.ItemContainer.Consumeables.Find(x => x.ItemName == "Essence of Light"));

        craftingItems.Add("Iron Bar-Wood-Wood-Wood-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Resting Chamber"));

        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Gold Bar-Titanium Bar-Diamond-Onyx Bar-Manganese Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Assault Rifle"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Gold Bar-Manganese Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Handgun"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Gold Bar-Titanium Bar-Diamond-Onyx Bar-Manganese Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-EMPTY-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Rocket Launcher"));
        craftingItems.Add("Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Gold Bar-Titanium Bar-Diamond-Iron Bar-Iron Bar-Onyx Bar-Iron Bar-Manganese Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-Iron Bar-EMPTY-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == "Jetpack"));

        craftingItems.Add("EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == "Wood Nanosaber"));
        craftingItems.Add("Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-EMPTY-Wood-EMPTY-Wood-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Wood Pickaxe"));
        craftingItems.Add("Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-Wood-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Wood Axe"));
        craftingItems.Add("Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-Wood-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-EMPTY-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Tools.Find(x => x.ItemName == "Wood Hammer"));

        craftingItems.Add("Iron Bar-Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Iron Block"));
        craftingItems.Add("Iron Bar-EMPTY-EMPTY-EMPTY-EMPTY-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Iron Wall"));

        craftingItems.Add("EMPTY-Glass-EMPTY-Glass-EMPTY-EMPTY-Glass-EMPTY-Glass-EMPTY-Glass-EMPTY-EMPTY-EMPTY-Glass-Glass-Brown Mushroom-Brown Mushroom-Brown Mushroom-Glass-EMPTY-Glass-Glass-Glass-",
            InventoryManager.Instance.ItemContainer.Consumeables.Find(x => x.ItemName == "Health Brew"));
        craftingItems.Add("EMPTY-Glass-EMPTY-Glass-EMPTY-EMPTY-Glass-EMPTY-Glass-EMPTY-Glass-EMPTY-EMPTY-EMPTY-Glass-Glass-Red Flower-Brown Mushroom-Yellow Flower-Glass-EMPTY-Glass-Glass-Glass-",
            InventoryManager.Instance.ItemContainer.Consumeables.Find(x => x.ItemName == "Fuel Brew"));

        craftingItems.Add("Wood-Wood-Wood-EMPTY-EMPTY-Wood-Iron Bar-Wood-EMPTY-EMPTY-Wood-Iron Bar-Wood-EMPTY-EMPTY-Wood-Iron Bar-Wood-EMPTY-EMPTY-Wood-Iron Bar-Wood-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Wooden Door"));

        craftingItems.Add("Wood-Wood-Wood-Wood-Wood-Wood-Wood-EMPTY-Wood-Wood-Wood-Wood-EMPTY-Wood-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-Wood-EMPTY-EMPTY-EMPTY-Wood-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Wooden Platform"));

        craftingItems.Add("Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-Iron Bar-EMPTY-EMPTY-EMPTY-Iron Bar-EMPTY-Iron Bar-Iron Bar-Iron Bar-",
            InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Bucket"));

        craftingItems.Add("Cobblestone-EMPTY-EMPTY-EMPTY-EMPTY-Cobblestone-",
            InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Cobble Piece"));

        foreach (KeyValuePair<string, Item> craftingItem in CraftingBench.craftingItems)
        {
            remainingRecipes.Add(craftingItem.Key, craftingItem.Value);
        }
    }

    public virtual void CraftItem()
    {
        Recipe rec = new Recipe();
        string output = string.Empty;
        int itemCount = 0;
        int maxItems = 0;
        bool craftable = false;
        //bool notFirstItem = false;

        foreach (GameObject slot in allSlots)
        {
            Slot tmp = slot.GetComponent<Slot>();

            if (!tmp.IsEmpty)
                maxItems++;
        }

            foreach (GameObject slot in allSlots)
        {
            Slot tmp = slot.GetComponent<Slot>();

            if (tmp.IsEmpty /*&& notFirstItem*/ && maxItems > itemCount)
                output += "EMPTY-";
            else if (!tmp.IsEmpty)
            {
                //notFirstItem = true;
                output += tmp.CurrentItem.Item.ItemName + "-";
                itemCount++;
            }
        }
        rec.recipe = output;

        if (craftingItems.ContainsKey(rec.recipe))
        {
            GameObject tmpObj = Instantiate(InventoryManager.Instance.itemObject);
            tmpObj.AddComponent<ItemScript>();

            ItemScript craftedItem = tmpObj.GetComponent<ItemScript>();
            Item tmpItem;

            craftingItems.TryGetValue(rec.recipe, out tmpItem);

            if (tmpItem != null)
            {
                craftable = true;
                if (!CraftingPreviewManager.craftingList.ContainsKey(tmpItem.ItemName))
                {
                    CraftingPreviewManager.Instance.AddRecipe(tmpItem.ItemName, rec.recipe);
                    CraftingBench.remainingRecipes.Remove(rec.recipe);
                }

                bool firstLoop = true;
                for (int i = 0; i < tmpItem.CraftAmount; i++)
                {
                    craftedItem.Item = tmpItem;

                    if (Player.Instance.inventory.AddItem(craftedItem, true) && firstLoop)
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

        if (!craftable)
        {
            output = string.Empty;
            itemCount = 0;
            maxItems = 0;
            bool notFirstItem = false;

            foreach (GameObject slot in allSlots)
            {
                Slot tmp = slot.GetComponent<Slot>();

                if (!tmp.IsEmpty)
                    maxItems++;
            }

            foreach (GameObject slot in allSlots)
            {
                Slot tmp = slot.GetComponent<Slot>();

                if (tmp.IsEmpty && notFirstItem && maxItems > itemCount)
                    output += "EMPTY-";
                else if (!tmp.IsEmpty)
                {
                    notFirstItem = true;
                    output += tmp.CurrentItem.Item.ItemName + "-";
                    itemCount++;
                }
            }
            rec.recipe = output;

            if (craftingItems.ContainsKey(rec.recipe))
            {
                GameObject tmpObj = Instantiate(InventoryManager.Instance.itemObject);
                tmpObj.AddComponent<ItemScript>();

                ItemScript craftedItem = tmpObj.GetComponent<ItemScript>();
                Item tmpItem;

                craftingItems.TryGetValue(rec.recipe, out tmpItem);

                if (tmpItem != null)
                {
                    if (!CraftingPreviewManager.craftingList.ContainsKey(tmpItem.ItemName))
                    {
                        CraftingPreviewManager.Instance.AddRecipe(tmpItem.ItemName, rec.recipe);
                        CraftingBench.remainingRecipes.Remove(rec.recipe);
                    }

                    bool firstLoop = true;
                    for (int i = 0; i < tmpItem.CraftAmount; i++)
                    {
                        craftedItem.Item = tmpItem;

                        if (Player.Instance.inventory.AddItem(craftedItem, true) && firstLoop)
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
        }

        UpdatePreview();
    }

    public override void MoveItem(GameObject clicked)
    {
        base.MoveItem(clicked);

        UpdatePreview();
    }

    public virtual void UpdatePreview()
    {
        Recipe rec = new Recipe();
        string output = string.Empty;
        int itemCount = 0;
        int maxItems = 0;
        bool craftable = false;
        //bool notFirstItem = false;

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

            if (tmp.IsEmpty /*&& notFirstItem*/ && maxItems > itemCount)
            {
                output += "EMPTY-";
            }
            else if (!tmp.IsEmpty)
            {
                //notFirstItem = true;
                output += tmp.CurrentItem.Item.ItemName + "-";
                itemCount++;
            }
        }
        rec.recipe = output;

        if (craftingItems.ContainsKey(rec.recipe))
        {
            GameObject tmpObj = Instantiate(InventoryManager.Instance.itemObject);
            tmpObj.AddComponent<ItemScript>();

            ItemScript craftedItem = tmpObj.GetComponent<ItemScript>();
            Item tmpItem;

            craftingItems.TryGetValue(rec.recipe, out tmpItem);

            if (tmpItem != null)
            {
                craftable = true;
                for (int i = 0; i < tmpItem.CraftAmount; i++)
                {
                    craftedItem.Item = tmpItem;
                    previewSlot.GetComponent<Slot>().AddItem(craftedItem);

                    Destroy(tmpObj);
                }
            }
        }

        if (!craftable)
        {
            output = string.Empty;
            itemCount = 0;
            maxItems = 0;
            bool notFirstItem = false;

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

                if (tmp.IsEmpty && notFirstItem && maxItems > itemCount)
                {
                    output += "EMPTY-";
                }
                else if (!tmp.IsEmpty)
                {
                    notFirstItem = true;
                    output += tmp.CurrentItem.Item.ItemName + "-";
                    itemCount++;
                }
            }
            rec.recipe = output;

            if (craftingItems.ContainsKey(rec.recipe))
            {
                GameObject tmpObj = Instantiate(InventoryManager.Instance.itemObject);
                tmpObj.AddComponent<ItemScript>();

                ItemScript craftedItem = tmpObj.GetComponent<ItemScript>();
                Item tmpItem;

                craftingItems.TryGetValue(rec.recipe, out tmpItem);

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

    public override void LoadInventory()
    {
        base.LoadInventory();

        UpdatePreview();
    }

    public override void Open(bool forceOpen = false)
    {
        base.Open(false);

        foreach (GameObject slot in allSlots)
        {
            Slot tmpSlot = slot.GetComponent<Slot>();

            int count = tmpSlot.Items.Count;
            bool addedToSel = false;
            for (int i = 0; i < count; i++)
            {
                ItemScript tmpItem = tmpSlot.RemoveItem();

                if (Player.Instance.inventorySelect.AddItem(tmpItem, true))
                    addedToSel = true;
                else
                {
                    Vector3 throwVec;
                    if (Player.Instance.GetComponent<PlatformerCharacter2D>().m_FacingRight)
                        throwVec = new Vector3(Player.Instance.transform.position.x + 3f, Player.Instance.transform.position.y + 1f, Player.Instance.transform.position.z);
                    else
                        throwVec = new Vector3(Player.Instance.transform.position.x - 3f, Player.Instance.transform.position.y + 1f, Player.Instance.transform.position.z);

                    GameObject tmpDrp = (GameObject)GameObject.Instantiate(InventoryManager.Instance.dropItem, throwVec, Quaternion.identity);

                    tmpDrp.AddComponent<ItemScript>();
                    tmpDrp.GetComponent<ItemScript>().Item = tmpItem.Item;
                    tmpDrp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tmpItem.Item.ItemSprite);
                }

                if (!addedToSel && !Player.Instance.inventory.AddItem(tmpItem, true))
                {
                    Vector3 throwVec;
                    if (Player.Instance.GetComponent<PlatformerCharacter2D>().m_FacingRight)
                        throwVec = new Vector3(Player.Instance.transform.position.x + 3f, Player.Instance.transform.position.y + 1f, Player.Instance.transform.position.z);
                    else
                        throwVec = new Vector3(Player.Instance.transform.position.x - 3f, Player.Instance.transform.position.y + 1f, Player.Instance.transform.position.z);

                    GameObject tmpDrp = (GameObject)GameObject.Instantiate(InventoryManager.Instance.dropItem, throwVec, Quaternion.identity);
                    tmpDrp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tmpItem.Item.ItemSprite);

                    tmpDrp.AddComponent<ItemScript>();
                    tmpDrp.GetComponent<ItemScript>().Item = tmpItem.Item;
                }
            }
        }

        UpdatePreview();
    }
}

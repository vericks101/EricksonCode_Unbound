using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;

public class SmeltingBench : Inventory
{
    [SerializeField] protected GameObject prefabButton;
    protected GameObject craftBtn;
    private static Dictionary<string, Item> smeltingItems = new Dictionary<string, Item>();
    protected GameObject previewSlot;

    private static SmeltingBench instance;

    public static SmeltingBench Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SmeltingBench").GetComponent<SmeltingBench>();

            return instance;
        }
    }

    public override void CreateLayout()
    {
        base.CreateLayout();

        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHight + slotSize + slotPaddingTop * 2);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth + slotPaddingLeft * 2 + slotSize * 2);

        craftBtn = Instantiate(prefabButton);

        RectTransform btnRect = craftBtn.GetComponent<RectTransform>();

        craftBtn.name = "CraftButton";
        craftBtn.transform.SetParent(this.transform.parent);

        btnRect.localPosition = inventoryRect.localPosition + new Vector3(slotPaddingLeft, -slotPaddingTop * 2 - (slotSize * 1));
        btnRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((slotSize * 2) + (slotPaddingLeft * 2)) * InventoryManager.Instance.canvas.scaleFactor);
        btnRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * InventoryManager.Instance.canvas.scaleFactor);

        craftBtn.transform.SetParent(transform);
        craftBtn.GetComponent<Button>().onClick.AddListener(CraftItem);

        previewSlot = Instantiate(InventoryManager.Instance.slotPrefab);

        RectTransform slotRect = previewSlot.GetComponent<RectTransform>();

        previewSlot.name = "PreviewSlot";
        previewSlot.transform.SetParent(this.transform.parent);

        slotRect.localPosition = inventoryRect.localPosition + new Vector3((slotPaddingLeft * 2) + (slotSize * 2), -slotPaddingTop * 2 - (slotSize * 1));
        slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize * InventoryManager.Instance.canvas.scaleFactor);
        slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * InventoryManager.Instance.canvas.scaleFactor);

        previewSlot.transform.SetParent(this.transform);
        previewSlot.GetComponent<Slot>().clickAble = false;
        previewSlot.GetComponent<CanvasGroup>().interactable = false;
    }

    public virtual void CreateBlueprints()
    {
        smeltingItems.Add("Cobblestone-", InventoryManager.Instance.ItemContainer.Placeables.Find(x => x.ItemName == "Stone"));

        smeltingItems.Add("Copper Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Copper Bar"));
        smeltingItems.Add("Iron Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Iron Bar"));
        smeltingItems.Add("Gold Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Gold Bar"));

        smeltingItems.Add("Titanium Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Titanium Bar"));
        smeltingItems.Add("Tungsten Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Tungsten Bar"));
        smeltingItems.Add("Zirconin Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Zirconin Bar"));

        smeltingItems.Add("Lead Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Lead Bar"));
        smeltingItems.Add("Bauxum Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Bauxum Bar"));

        smeltingItems.Add("Thyrium Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Thyrium Bar"));
        smeltingItems.Add("Tin Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Tin Bar"));
        smeltingItems.Add("Onyx Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Onyx Bar"));

        smeltingItems.Add("Cobalt Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Cobalt Bar"));
        smeltingItems.Add("Sphalerite Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Sphalerite Bar"));
        smeltingItems.Add("Manganese Ore-", InventoryManager.Instance.ItemContainer.Materials.Find(x => x.ItemName == "Manganese Bar"));
    }

    public virtual void CraftItem()
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

        if (smeltingItems.ContainsKey(rec.recipe))
        {
            GameObject tmpObj = Instantiate(InventoryManager.Instance.itemObject);
            tmpObj.AddComponent<ItemScript>();

            ItemScript craftedItem = tmpObj.GetComponent<ItemScript>();
            Item tmpItem;

            smeltingItems.TryGetValue(rec.recipe, out tmpItem);

            if (tmpItem != null)
            {
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

        if (smeltingItems.ContainsKey(rec.recipe))
        {
            GameObject tmpObj = Instantiate(InventoryManager.Instance.itemObject);
            tmpObj.AddComponent<ItemScript>();

            ItemScript craftedItem = tmpObj.GetComponent<ItemScript>();
            Item tmpItem;

            smeltingItems.TryGetValue(rec.recipe, out tmpItem);

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

                    tmpDrp.AddComponent<ItemScript>();
                    tmpDrp.GetComponent<ItemScript>().Item = tmpItem.Item;
                    tmpDrp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tmpItem.Item.ItemSprite);
                }
            }
        }
    }
}
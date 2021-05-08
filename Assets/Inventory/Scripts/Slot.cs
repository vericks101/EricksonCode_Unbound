using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public Text stackTxt;
    public Image spriteImage;

    public Sprite slotEmpty;
    public Sprite slotHighlight;
    public Sprite itemSprite;
    public Sprite defaultSprite;

	public ItemType canContain;

	public bool clickAble = true;
    private CanvasGroup canvasGroup;

	private Stack<ItemScript> items;

    public Stack<ItemScript> Items
    {
        get { return items; }
        set { items = value; }
    }

    public bool IsEmpty
    {
        get { return items.Count == 0; }
    }

    public bool IsAvailable
    {
        get { return CurrentItem.Item.MaxSize > items.Count; }
    }

    public ItemScript CurrentItem
    {
        get
        {
            if (items.Count > 0)
                return items.Peek();
            else
                return null;
        }
    }

    void Awake()
    {
        items = new Stack<ItemScript>();
    }

    void Start()
    {
        RectTransform slotRect = GetComponent<RectTransform>();
        RectTransform txtRect = stackTxt.GetComponent<RectTransform>();
        RectTransform spriteRect = spriteImage.GetComponent<RectTransform>();
        int txtScleFactor = (int)(slotRect.sizeDelta.x * 0.60);

        stackTxt.resizeTextMaxSize = txtScleFactor;
        stackTxt.resizeTextMinSize = txtScleFactor;
        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);
        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotRect.sizeDelta.y);

        spriteRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);
        spriteRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotRect.sizeDelta.y);

        if (transform.parent != null)
        {
            canvasGroup = transform.parent.GetComponent<CanvasGroup>();

			EventTrigger trigger = GetComponentInParent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener ((eventData) => { Player.Instance.inventory.ShowToolTip (gameObject); });

			if (trigger != null)
                trigger.triggers.Add (entry);
        }
    }

    public void AddItem(ItemScript item)
    {
        if (transform.parent.GetComponent<VendorInventory>() == null && item != null && clickAble)
            CombatTextManager.Instance.CreateText(Player.Instance.transform.position, item.Item.ItemName, Color.white, false, true);
        if (IsEmpty && transform.parent.GetComponent<Inventory>() != null)
            transform.parent.GetComponent<Inventory>().EmptySlots--;
        items.Push(item);
        if (clickAble)
            QuestManager.Instance.itemCollected = item.Item.ItemName;

        if (items.Count > 1)
            stackTxt.text = items.Count.ToString();
        ChangeSprite(item.spriteNeutral, item.spriteHighlighted, item.itemSprite);
    }

    public void AddItems(Stack<ItemScript> items)
    {
        if (items.Peek().Item != null)
        {
            this.items = new Stack<ItemScript>(items);
            QuestManager.Instance.itemCollected = items.Peek().Item.ItemName;
            stackTxt.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

            ChangeSprite(CurrentItem.spriteNeutral, CurrentItem.spriteHighlighted, CurrentItem.itemSprite);
        }
    }

    private void ChangeSprite(Sprite neutral, Sprite highlight, Sprite sprite)
    {
        //GetComponent<Image>().sprite = neutral;

        //SpriteState st = new SpriteState();
        //st.highlightedSprite = highlight;
        //st.pressedSprite = neutral;
        if (sprite != null)
            spriteImage.sprite = sprite;
        else
            spriteImage.sprite = defaultSprite;
        //GetComponent<Button>().spriteState = st;
    }

    private void UseItem()
    {
		if (!IsEmpty) 
		{
			if (transform.parent.GetComponent<Inventory> () is VendorInventory) 
			{
                if (CurrentItem.Item.BuyPrice <= Player.Instance.Gold && Player.Instance.inventory.AddItem(CurrentItem, true))
                {
                    Player.Instance.Gold -= CurrentItem.Item.BuyPrice;
                    AudioManager.instance.PlaySound("Sell0");
                }
			}
			else if (VendorInventory.Instance.IsOpen)
			{
				Player.Instance.Gold += CurrentItem.Item.SellPrice;
                MenuManager.Instance.overallIncome += CurrentItem.Item.SellPrice;
                AudioManager.instance.PlaySound("Sell0");
                RemoveItem ();
			}
			else if (clickAble)
			{
				items.Peek().Use(this);

				stackTxt.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

				if (IsEmpty)
				{
					ChangeSprite(slotEmpty, slotHighlight, itemSprite);
					transform.parent.GetComponent<Inventory>().EmptySlots++;
				}
			}
		}
    }

    public void ClearSlot()
    {
        items.Clear();
        ChangeSprite(slotEmpty, slotHighlight, itemSprite);
        stackTxt.text = string.Empty;

        if (transform.parent != null && transform.parent.GetComponent<Inventory>() != null)
            transform.parent.GetComponent<Inventory>().EmptySlots++;
    }

    public Stack<ItemScript> RemoveItems(int amount)
    {
        Stack<ItemScript> tmp = new Stack<ItemScript>();

        for (int i = 0; i < amount; i++)
        {
            tmp.Push(items.Pop());
        }

        stackTxt.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

        return tmp;
    }

    public ItemScript RemoveItem()
    {
        if (!IsEmpty)
        {
            ItemScript tmp = items.Pop();

            stackTxt.text = items.Count > 1 ? items.Count.ToString() : string.Empty;
            if (IsEmpty) 
				ClearSlot();

            return tmp;
        }

        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !GameObject.Find("Hover") && canvasGroup != null && canvasGroup.alpha > 0)
        {
            UseItem();
        }
        else if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftShift) && !IsEmpty && !GameObject.Find("Hover") &&
            transform.parent.GetComponent<Inventory>().IsOpen)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(InventoryManager.Instance.canvas.transform as RectTransform, Input.mousePosition,
                InventoryManager.Instance.canvas.worldCamera, out position);

            if (!InventoryManager.Instance.selectStackSize.activeInHierarchy)
            {
                InventoryManager.Instance.selectStackSize.SetActive(true);
                InventoryManager.Instance.selectStackSize.transform.position = InventoryManager.Instance.canvas.transform.TransformPoint(position);
                InventoryManager.Instance.SetStackInfo(items.Count);
            }
            Player.Instance.inventory.ChangeStackText(1);
        }
    }

    public static void SwapItems(Slot from, Slot to)
    {
        if (to != null && from != null)
        {
            bool calcStats = from.transform.parent == CharacterPanel.Instance.transform || to.transform.parent == CharacterPanel.Instance.transform;

            if (CanSwap(from, to))
            {
                Stack<ItemScript> tmpTo = new Stack<ItemScript>(to.Items);

                to.AddItems(from.Items);

                if (tmpTo.Count == 0)
                {
                    to.transform.parent.GetComponent<Inventory>().EmptySlots--;
                    from.ClearSlot();
                }
                else
                {
                    from.AddItems(tmpTo);
                }
            }

            if (calcStats) 
				CharacterPanel.Instance.CalcStats();
        }
    }

    private static bool CanSwap(Slot from, Slot to)
    {
        ItemType fromType = from.CurrentItem.Item.ItemType;

        if (to.canContain == from.canContain) 
			return true;

        if (fromType != ItemType.OFFHAND && to.canContain == fromType) 
			return true;
        
        if (to.canContain == ItemType.GENERIC && (to.IsEmpty || to.CurrentItem.Item.ItemType == fromType)) 
			return true;
        
		if (fromType == ItemType.MAINHAND && to.canContain == ItemType.GENERICWEAPON) 
			return true;
        
        if (fromType == ItemType.TWOHAND && to.canContain == ItemType.GENERICWEAPON && CharacterPanel.Instance.OffHandSlot.IsEmpty) 
			return true;
        
        if (fromType == ItemType.OFFHAND && (to.IsEmpty || to.CurrentItem.Item.ItemType == ItemType.OFFHAND) && (CharacterPanel.Instance.WeaponSlot.IsEmpty || 
			CharacterPanel.Instance.WeaponSlot.CurrentItem.Item.ItemType != ItemType.TWOHAND)) 
			return true;
        
        return false;
    }

    public void OnSlotClick()
    {
        AudioManager.instance.PlaySound("Click_Select");
    }
}
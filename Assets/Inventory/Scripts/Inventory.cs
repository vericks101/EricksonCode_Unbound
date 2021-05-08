using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets._2D;
using System.Collections;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
	public static bool mouseInside = false;

    public int rows;
    public int slots;

    public float slotPaddingLeft;
    public float slotPaddingTop;
	public float slotSize;

	public CanvasGroup canvasGroup;
	public float fadeTime;

    private bool fadingIn;
    private bool fadingOut;

	private bool isOpen;
	private bool instantClose = false;

	protected float hoverYOffset;
	protected float inventoryWidth, inventoryHight;
	public RectTransform inventoryRect;

	protected static GameObject playerRef;
	private InventorySelect inventorySelect;

	public List<GameObject> allSlots;
	private int emptySlots;

    [SerializeField] private LayerMask hitMask;

    public bool FadingOut
    {
        get { return fadingOut; }
    }

    public bool InstantClose 
	{
		get { return instantClose; }
		set { instantClose = value; }
	}

    public bool IsOpen
    {
        get { return isOpen; }
        set { isOpen = value; }
    }

    public int EmptySlots
    {
        get { return emptySlots; }
        set { emptySlots = value; }
    }

    protected virtual void Start()
    {
		if (inventorySelect == null)
            inventorySelect = FindObjectOfType<InventorySelect> ();

        isOpen = false;

        playerRef = GameObject.Find("Player");

		CreateLayout ();

        InventoryManager.Instance.MovingSlot = GameObject.Find("MovingSlot").GetComponent<Slot>();
		InventoryManager.Instance.SelectedSlot = GameObject.Find("SelectedSlot").GetComponent<Slot>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1) && InventoryManager.Instance.selectStackSize.activeInHierarchy)
            SplitStack();

        if (Input.GetMouseButtonUp(1))
        {
            if (!mouseInside && InventoryManager.Instance.From != null)
            {
                InventoryManager.Instance.From.GetComponent<Image>().color = Color.white;

                Vector3 throwVec;
                if (Player.Instance.GetComponent<PlatformerCharacter2D>().m_FacingRight)
                    throwVec = new Vector3(Player.Instance.transform.position.x + 3f, Player.Instance.transform.position.y + 1f, Player.Instance.transform.position.z);
                else
                    throwVec = new Vector3(Player.Instance.transform.position.x - 3f, Player.Instance.transform.position.y + 1f, Player.Instance.transform.position.z);

                GameObject tmpDrp = (GameObject)GameObject.Instantiate(InventoryManager.Instance.dropItem, throwVec, Quaternion.identity);

                tmpDrp.AddComponent<ItemScript>();
                tmpDrp.AddComponent<BoxCollider2D>();
                tmpDrp.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                Item poppedItem = InventoryManager.Instance.From.Items.Pop().Item;
                tmpDrp.GetComponent<ItemScript>().Item = poppedItem;
                tmpDrp.GetComponent<ItemScript>().itemCount = InventoryManager.Instance.From.Items.Count + 1;
                tmpDrp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(poppedItem.ItemSprite);

                InventoryManager.Instance.From.ClearSlot();

                if (InventoryManager.Instance.From.transform.parent == CharacterPanel.Instance.transform)
                    CharacterPanel.Instance.CalcStats();
                if (CharacterPanel.Instance.armorSlots[2].Items.Count <= 0 || (CharacterPanel.Instance.armorSlots[2].Items.Count > 0 && CharacterPanel.Instance.armorSlots[2].CurrentItem.Item.ItemName != "Jetpack"))
                    Player.Instance.jetpack.SetActive(false);

                Destroy(GameObject.Find("Hover"));

                InventoryManager.Instance.To = null;
                InventoryManager.Instance.From = null;

                if (CharacterPanel.Instance.OffHandSlot.IsEmpty)
                    Player.Instance.shield.SetActive(false);

                Player.Instance.inventorySelect.ChangeCurrentItemText();
            }
            else if (!InventoryManager.Instance.eventSystem.IsPointerOverGameObject(-1) && !InventoryManager.Instance.MovingSlot.IsEmpty)
            {
                Vector3 throwVec;
                if (Player.Instance.GetComponent<PlatformerCharacter2D>().m_FacingRight)
                    throwVec = new Vector3(Player.Instance.transform.position.x + 3f, Player.Instance.transform.position.y + 1f, Player.Instance.transform.position.z);
                else
                    throwVec = new Vector3(Player.Instance.transform.position.x - 3f, Player.Instance.transform.position.y + 1f, Player.Instance.transform.position.z);

                GameObject tmpDrp = (GameObject)GameObject.Instantiate(InventoryManager.Instance.dropItem, throwVec, Quaternion.identity);

                tmpDrp.AddComponent<ItemScript>();
                tmpDrp.AddComponent<BoxCollider2D>();
                tmpDrp.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                Item poppedItem = InventoryManager.Instance.From.Items.Pop().Item;
                tmpDrp.GetComponent<ItemScript>().Item = poppedItem;
                tmpDrp.GetComponent<ItemScript>().itemCount = InventoryManager.Instance.From.Items.Count + 1;
                tmpDrp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(poppedItem.ItemSprite);

                InventoryManager.Instance.MovingSlot.ClearSlot();

                Destroy(GameObject.Find("Hover"));

                if (CharacterPanel.Instance.OffHandSlot.IsEmpty)
                    Player.Instance.shield.SetActive(false);

                Player.Instance.inventorySelect.ChangeCurrentItemText();
            }
        }

        if (InventoryManager.Instance.HoverObject != null)
        {
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(InventoryManager.Instance.canvas.transform as RectTransform, Input.mousePosition, 
				InventoryManager.Instance.canvas.worldCamera, out position);
            position.Set(position.x, position.y - hoverYOffset);

            InventoryManager.Instance.HoverObject.transform.position = InventoryManager.Instance.canvas.transform.TransformPoint(position);
        }
    }

    public void OnDrag()
    {
        if (isOpen)
            MoveInventory ();
    }

    public void PointerExit()
    {
        mouseInside = false;
    }

    public void PointerEnter()
    {
        if (canvasGroup.alpha > 0)
            mouseInside = true;
    }

    public virtual void Open(bool forceOpen)
    {
        if (forceOpen)
        {
            StartCoroutine("FadeIn");
            canvasGroup.blocksRaycasts = true;
            isOpen = true;
        }

        if (canvasGroup.alpha > 0 && !forceOpen)
        {
            canvasGroup.blocksRaycasts = false;
            StartCoroutine("FadeOut");
            PutItemBack();
            HideToolTip();
            isOpen = false;
        }
        else if (!forceOpen)
        {
            StartCoroutine("FadeIn");
            canvasGroup.blocksRaycasts = true;
            isOpen = true;
        }
    }

    public virtual void ShowToolTip(GameObject slot)
    {
        Slot tmpSlot = slot.GetComponent<Slot>();

        if ((slot.transform.parent.name == "CharPanel" || slot.transform.parent.name == "CraftingPanel" || slot.GetComponentInParent<Inventory>().isOpen) && !tmpSlot.IsEmpty && InventoryManager.Instance.HoverObject == null 
			&& !InventoryManager.Instance.selectStackSize.activeSelf)
        {
            InventoryManager.Instance.visualTextObject.text = tmpSlot.CurrentItem.GetTooltip(this);
            InventoryManager.Instance.sizeTextObject.text = InventoryManager.Instance.visualTextObject.text;

            InventoryManager.Instance.tooltipObject.SetActive(true);

            float xPos = slot.transform.position.x + slotPaddingLeft;
            float yPos;
            /*if (slot.transform.parent != null && slot.transform.parent.name == "CraftingPanel")
                yPos = slot.transform.position.y - (slot.GetComponent<RectTransform>().sizeDelta.y / 2) - slotPaddingTop;
            else */if (slot.transform.parent != null && slot.transform.parent.name == "CharPanel")
            {
                xPos = slot.transform.position.x;
                yPos = slot.transform.position.y;
            }
            else
                yPos = slot.transform.position.y - slot.GetComponent<RectTransform>().sizeDelta.y - slotPaddingTop;

            InventoryManager.Instance.tooltipObject.transform.position = new Vector2(xPos, yPos);
        }
    }

    public void HideToolTip()
    {
        InventoryManager.Instance.tooltipObject.SetActive(false);
    }

    public virtual void SaveInventory()
    {
        string content = string.Empty;

        for (int i = 0; i < allSlots.Count; i++)
        {
            Slot tmp = allSlots[i].GetComponent<Slot>();
			if (!tmp.IsEmpty)
                content += i + "-" + tmp.CurrentItem.Item.ItemName.ToString () + "-" + tmp.Items.Count.ToString () + ";";
        }

        PlayerPrefs.SetString(gameObject.name + "content", content);

        PlayerPrefs.SetInt(gameObject.name + "slots", slots);
        PlayerPrefs.SetInt(gameObject.name + "rows", rows);

        PlayerPrefs.SetFloat(gameObject.name + "slotPaddingLeft", slotPaddingLeft);
        PlayerPrefs.SetFloat(gameObject.name + "slotPaddingTop", slotPaddingTop);
        PlayerPrefs.SetFloat(gameObject.name + "slotSize", slotSize);
        PlayerPrefs.SetFloat(gameObject.name + "xPos", inventoryRect.position.x);
        PlayerPrefs.SetFloat(gameObject.name + "yPos", inventoryRect.position.y);

        PlayerPrefs.Save();
    }

    public virtual void LoadInventory()
    {
        string content = PlayerPrefs.GetString(gameObject.name + "content");

        if (content != string.Empty)
        {
            slots = PlayerPrefs.GetInt(gameObject.name + "slots");
            rows = PlayerPrefs.GetInt(gameObject.name + "rows");

            slotPaddingLeft = PlayerPrefs.GetFloat(gameObject.name + "slotPaddingLeft");
            slotPaddingTop = PlayerPrefs.GetFloat(gameObject.name + "slotPaddingTop");
            slotSize = PlayerPrefs.GetFloat(gameObject.name + "slotSize");

            inventoryRect.position = new Vector3(PlayerPrefs.GetFloat(gameObject.name + "xPos"), PlayerPrefs.GetFloat(gameObject.name + "yPos"), 
				inventoryRect.position.z);

            CreateLayout();

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

                    if (tmp == null)
                        tmp = InventoryManager.Instance.ItemContainer.Consumeables.Find(item => item.ItemName == itemName);

                    if (tmp == null)
                        tmp = InventoryManager.Instance.ItemContainer.Equipment.Find(item => item.ItemName == itemName);

                    if (tmp == null)
                        tmp = InventoryManager.Instance.ItemContainer.Weapons.Find(item => item.ItemName == itemName);

                    if (tmp == null)
                        tmp = InventoryManager.Instance.ItemContainer.Materials.Find(item => item.ItemName == itemName);

					if (tmp == null)
                        tmp = InventoryManager.Instance.ItemContainer.Placeables.Find(item => item.ItemName == itemName);

                    if (tmp == null)
                        tmp = InventoryManager.Instance.ItemContainer.Tools.Find(item => item.ItemName == itemName);

                    loadedItem.AddComponent<ItemScript>();
                    loadedItem.GetComponent<ItemScript>().Item = tmp;

                    allSlots[index].GetComponent<Slot>().AddItem(loadedItem.GetComponent<ItemScript>());

                    Destroy(loadedItem);
                }
            }
        }
    }

    public virtual void CreateLayout()
    {
        if (allSlots != null)
        {
            foreach (GameObject go in allSlots)
            {
                Destroy(go);
            }
        }

        allSlots = new List<GameObject>();
        hoverYOffset = slotSize * 0.01f;
        emptySlots = slots;

        inventoryWidth = (slots / rows) * (slotSize + slotPaddingLeft);
        inventoryHight = rows * (slotSize + slotPaddingTop);
        inventoryRect = GetComponent<RectTransform>();
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth + slotPaddingLeft);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHight + slotPaddingTop);

        int columns = slots / rows;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject newSlot = (GameObject)Instantiate(InventoryManager.Instance.slotPrefab);
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();

                newSlot.name = "Slot";
                newSlot.transform.SetParent(this.transform.parent);

                slotRect.localPosition = inventoryRect.localPosition + new Vector3(slotPaddingLeft * (x + 1) + (slotSize * x), -slotPaddingTop * (y + 1) - 
					(slotSize * y));
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize * InventoryManager.Instance.canvas.scaleFactor);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * InventoryManager.Instance.canvas.scaleFactor);
                newSlot.transform.SetParent(this.transform);

                allSlots.Add(newSlot);
                newSlot.GetComponent<Button>().onClick.AddListener ( delegate{ MoveItem(newSlot); } );
            }
        }
    }

    public bool AddItem(ItemScript item, bool questableItem)
    {
        if (item.Item.MaxSize == 1)
        {
            if (GetType() != typeof(VendorInventory))
                CombatTextManager.Instance.CreateText(Player.Instance.transform.position, item.Item.ItemName, Color.white, false, true);
            return PlaceEmpty(item);
        }
        else
        {
            foreach (GameObject slot in allSlots)
            {
				Slot tmp = slot.GetComponent<Slot> ();

                if (!tmp.IsEmpty)
                {
                    if (tmp.CurrentItem.Item.ItemName == item.Item.ItemName && tmp.IsAvailable)
                    {
                        if (!InventoryManager.Instance.MovingSlot.IsEmpty && InventoryManager.Instance.Clicked.GetComponent<Slot>() == tmp.GetComponent<Slot>())
                        {
                            continue;
                        }
                        else
                        {
							tmp.AddItem(item);
                            if (questableItem)
                                QuestManager.Instance.itemCollected = item.Item.ItemName;
                            return true;
                        }
                    }
                }
            }

			if (emptySlots > 0)
                return PlaceEmpty(item);
        }

        return false;
    }

    private void MoveInventory()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(InventoryManager.Instance.canvas.transform as RectTransform, 
			new Vector3(Input.mousePosition.x - (inventoryRect.sizeDelta.x / 2 * InventoryManager.Instance.canvas.scaleFactor), 
				Input.mousePosition.y + (inventoryRect.sizeDelta.y / 2 * InventoryManager.Instance.canvas.scaleFactor)), 
			InventoryManager.Instance.canvas.worldCamera, out mousePos);
        transform.position = InventoryManager.Instance.canvas.transform.TransformPoint(mousePos);
    }

    private bool PlaceEmpty(ItemScript item)
    {
        if (emptySlots > 0)
        {
            foreach (GameObject slot in allSlots)
            {
                Slot tmp = slot.GetComponent<Slot>();
                if (tmp.IsEmpty)
                {
                    tmp.AddItem(item);
                    return true;
                }
            }
        }

        return false;
    }

    public virtual void MoveItem(GameObject clicked)
	{
        if (isOpen)
        {
            CanvasGroup cg = clicked.transform.parent.GetComponent<CanvasGroup>();
            if (cg != null && cg.alpha > 0 || clicked.transform.parent.parent.GetComponent<CanvasGroup>().alpha > 0)
            {
                InventoryManager.Instance.Clicked = clicked;

                if (!InventoryManager.Instance.MovingSlot.IsEmpty)
                {
                    Slot tmp = clicked.GetComponent<Slot>();
                    if (tmp.IsEmpty)
                    {
                        tmp.AddItems(InventoryManager.Instance.MovingSlot.Items);
                        InventoryManager.Instance.MovingSlot.Items.Clear();
                        Destroy(GameObject.Find("Hover"));
                    }
                    else if (!tmp.IsEmpty && InventoryManager.Instance.MovingSlot.CurrentItem.Item.ItemName == tmp.CurrentItem.Item.ItemName && tmp.IsAvailable)
                    {
                        MergeStacks(InventoryManager.Instance.MovingSlot, tmp);
                    }
                    Player.Instance.inventorySelect.ChangeCurrentItemText();
                }
                else if (InventoryManager.Instance.From == null && clicked.transform.parent.GetComponent<Inventory>().isOpen && !Input.GetKey(KeyCode.LeftShift))
                {
                    if (!clicked.GetComponent<Slot>().IsEmpty && !GameObject.Find("Hover"))
                    {
                        InventoryManager.Instance.From = clicked.GetComponent<Slot>();
                        InventoryManager.Instance.From.GetComponent<Image>().color = Color.gray;

                        CreateHoverIcon(InventoryManager.Instance.From.Items.Peek().Item.ItemSprite);
                    }
                    Player.Instance.inventorySelect.ChangeCurrentItemText();
                }
                else if (InventoryManager.Instance.To == null && !Input.GetKey(KeyCode.LeftShift))
                {
                    InventoryManager.Instance.To = clicked.GetComponent<Slot>();
                    Destroy(GameObject.Find("Hover"));
                    Player.Instance.inventorySelect.ChangeCurrentItemText();
                }
                if (InventoryManager.Instance.To != null && InventoryManager.Instance.From != null)
                {
                    if (!InventoryManager.Instance.To.IsEmpty && InventoryManager.Instance.From.CurrentItem.Item.ItemName == 
						InventoryManager.Instance.To.CurrentItem.Item.ItemName && InventoryManager.Instance.To.IsAvailable)
                    {
                        MergeStacks(InventoryManager.Instance.From, InventoryManager.Instance.To);
                    }
                    else
                    {
                        Slot.SwapItems(InventoryManager.Instance.From, InventoryManager.Instance.To);
                    }

                    InventoryManager.Instance.From.GetComponent<Image>().color = Color.white;
                    InventoryManager.Instance.To = null;
                    InventoryManager.Instance.From = null;

                    Destroy(GameObject.Find("Hover"));
                    Player.Instance.inventorySelect.ChangeCurrentItemText();
                }
            }

            if (CraftingBench.Instance.isOpen)
                CraftingBench.Instance.UpdatePreview();
        }
    }

    private void CreateHoverIcon(String hoverSpriteName)
    {
        InventoryManager.Instance.HoverObject = (GameObject)Instantiate(InventoryManager.Instance.iconPrefab);
        InventoryManager.Instance.HoverObject.GetComponent<Image>().sprite = InventoryManager.Instance.Clicked.GetComponent<Image>().sprite;
        if (hoverSpriteName != null)
            InventoryManager.Instance.HoverObject.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(hoverSpriteName);
        else
            InventoryManager.Instance.HoverObject.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("TempNormall");
        InventoryManager.Instance.HoverObject.name = "Hover";

        RectTransform hoverTransform = InventoryManager.Instance.HoverObject.GetComponent<RectTransform>();
        RectTransform clickedTransform = InventoryManager.Instance.Clicked.GetComponent<RectTransform>();
        hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clickedTransform.sizeDelta.x);
        hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clickedTransform.sizeDelta.y);
        InventoryManager.Instance.HoverObject.transform.SetParent(GameObject.Find("Canvas").transform, true);
        InventoryManager.Instance.HoverObject.transform.localScale = InventoryManager.Instance.Clicked.gameObject.transform.localScale;
        InventoryManager.Instance.HoverObject.transform.GetChild(0).GetComponent<Text>().text = InventoryManager.Instance.MovingSlot.Items.Count > 1 ? InventoryManager.Instance.MovingSlot.Items.Count.ToString() : string.Empty;
    }

    private void PutItemBack()
    {
		if (InventoryManager.Instance.From != null)
        {
            Destroy(GameObject.Find("Hover"));

            InventoryManager.Instance.From.GetComponent<Image>().color = Color.white;
            InventoryManager.Instance.From = null;
        }
        else if (!InventoryManager.Instance.MovingSlot.IsEmpty)
        {
            Destroy(GameObject.Find("Hover"));

            foreach (ItemScript item in InventoryManager.Instance.MovingSlot.Items)
            {
                InventoryManager.Instance.Clicked.GetComponent<Slot>().AddItem(item);
            }

            InventoryManager.Instance.MovingSlot.ClearSlot();
        }

        InventoryManager.Instance.selectStackSize.SetActive(false);
    }

    public void SplitStack()
    {
        InventoryManager.Instance.selectStackSize.SetActive(false);

        if (InventoryManager.Instance.SplitAmount == InventoryManager.Instance.MaxStackCount)
        {
            MoveItem(InventoryManager.Instance.Clicked);
        }
        else if (InventoryManager.Instance.SplitAmount > 0)
        {
            InventoryManager.Instance.MovingSlot.Items = InventoryManager.Instance.Clicked.GetComponent<Slot>().RemoveItems(InventoryManager.Instance.SplitAmount);

            CreateHoverIcon(InventoryManager.Instance.Clicked.GetComponent<Slot>().Items.Peek().Item.ItemSprite);
        }
    }

    public void ChangeStackText(int i)
    {
        InventoryManager.Instance.SplitAmount += i;
        if (InventoryManager.Instance.SplitAmount < 0) InventoryManager.Instance.SplitAmount = 0;
        if (InventoryManager.Instance.SplitAmount > InventoryManager.Instance.MaxStackCount) InventoryManager.Instance.SplitAmount = 
			InventoryManager.Instance.MaxStackCount;

        InventoryManager.Instance.stackText.text = InventoryManager.Instance.SplitAmount.ToString();
    }

    public void MergeStacks(Slot source, Slot destination)
    {
        int max = destination.CurrentItem.Item.MaxSize - destination.Items.Count;
        int count = source.Items.Count < max ? source.Items.Count : max;

		for (int i = 0; i < count; i++)
        {
            destination.AddItem(source.RemoveItem());
            InventoryManager.Instance.HoverObject.transform.GetChild(0).GetComponent<Text>().text = InventoryManager.Instance.MovingSlot.Items.Count.ToString();
        }

        if (source.Items.Count == 0)
        {
            source.ClearSlot();
            Destroy(GameObject.Find("Hover"));
        }
    }

    protected virtual IEnumerator FadeOut()
    {
        if (!fadingOut)
        {
            fadingOut = true;
            fadingIn = false;
            StopCoroutine("FadeIn");

            float startAlpha = canvasGroup.alpha;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;

            while (progress < 1.0)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, progress);
                progress += rate * Time.deltaTime;

                if (instantClose) break;

                yield return null;
            }

            canvasGroup.alpha = 0;

            fadingOut = false;
            instantClose = false;
        }
    }

    private IEnumerator FadeIn()
    {
        if (!fadingIn)
        {
            fadingOut = false;
            fadingIn = true;
            StopCoroutine("FadeOut");

            float startAlpha = canvasGroup.alpha;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;

            while (progress < 1.0)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, progress);
                progress += rate * Time.deltaTime;

                yield return null;
            }

            canvasGroup.alpha = 1;
            fadingIn = false;
        }
    }
}
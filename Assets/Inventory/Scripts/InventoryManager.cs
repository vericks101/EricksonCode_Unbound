using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public GameObject iconPrefab;
    public GameObject itemObject;
    public GameObject dropItem;
    public GameObject tooltipObject;
    public Text sizeTextObject;
    public Text visualTextObject;
	public Text stackText;
	public GameObject selectStackSize;

    public Canvas canvas;
	public EventSystem eventSystem;

	public PlayerCrafting playerCrafting;

	private GameObject hoverObject;

	private ItemContainer itemContainer = new ItemContainer();

    private Slot from;
    private Slot to;

    [SerializeField] private Slot movingSlot;
	private Slot selectedSlot;
    private GameObject clicked;

    private int splitAmount;
    private int maxStackCount;

    private static bool createdBPs = false;

    private static InventoryManager instance;

    public static InventoryManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<InventoryManager>();

            return instance;
        }
    }

    public ItemContainer ItemContainer
    {
        get { return itemContainer; }
        set { itemContainer = value; }
    }

    public Slot From
    {
        get { return from; }
        set { from = value; }
    }

    public Slot To
    {
        get { return to; }
        set { to = value; }
    }

    public GameObject Clicked
    {
        get { return clicked; }
        set { clicked = value; }
    }

    public int SplitAmount
    {
        get { return splitAmount; }
        set { splitAmount = value; }
    }

    public int MaxStackCount
    {
        get { return maxStackCount; }
        set { maxStackCount = value; }
    }

    public Slot MovingSlot
    {
        get { return movingSlot; }
        set { movingSlot = value; }
    }

	public Slot SelectedSlot
	{
		get { return selectedSlot; }
		set { selectedSlot = value; }
	}

    public GameObject HoverObject
    {
        get { return hoverObject; }
        set { hoverObject = value; }
    }

    public void Awake()
    {
        XmlDocument doc = new XmlDocument();
        TextAsset myXmlAsset = Resources.Load<TextAsset>("Items");

        doc.LoadXml(myXmlAsset.text);

		Type[] itemTypes = { typeof(Equipment), typeof(Weapon), typeof(Consumeable), typeof(Material), typeof(Placeable), typeof(Tool) };

        XmlSerializer serializer = new XmlSerializer(typeof(ItemContainer), itemTypes);
        TextReader textReader = new StringReader(doc.InnerXml);

        itemContainer = (ItemContainer)serializer.Deserialize(textReader);

        textReader.Close();
        if (!createdBPs)
        {
            createdBPs = true;
        }
    }

    public void Start()
    {
        CraftingBench.Instance.CreateBlueprints();
        playerCrafting.CreateBlueprints();
        SmeltingBench.Instance.CreateBlueprints();

        //Item tmp = null;
        //foreach (KeyValuePair<string, Item> item in CraftingBench.craftingItems)
        //{
        //    tmp = item.Value;
        //    if (tmp != null)
        //        CraftingPreviewManager.Instance.AddRecipe(tmp.ItemName, item.Key);
        //}
    }

    public void SetStackInfo(int maxStackCount)
    {
        selectStackSize.SetActive(true);
        tooltipObject.SetActive(false);

        splitAmount = 0;
        this.maxStackCount = maxStackCount;
        stackText.text = splitAmount.ToString();
    }

    public void Save()
	{
        GameObject[] inventories = GameObject.FindGameObjectsWithTag("Inventory");
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");

        foreach (GameObject inventory in inventories)
        {
            inventory.GetComponent<Inventory>().SaveInventory();
        }

        foreach (GameObject chest in chests)
        {
			chest.GetComponent<InventoryLink>().SaveInventory();
        }
    }

    public void Load()
    {
        GameObject[] inventories = GameObject.FindGameObjectsWithTag("Inventory");
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");

        foreach (GameObject inventory in inventories)
        {
            inventory.GetComponent<Inventory>().LoadInventory();
        }

        foreach (GameObject chest in chests)
        {
            chest.GetComponent<InventoryLink>().LoadInventory();
        }
    }
}
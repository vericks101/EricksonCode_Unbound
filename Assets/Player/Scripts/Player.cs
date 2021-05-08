using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets._2D;
using System.Collections;

[System.Serializable]
public class PlayerAbility
{
    public Ability ability;
    public GameObject objHolder;
}

public class Player : MonoBehaviour
{
    public GameObject[] playerLightableObjects;

    public PlayerAbility[] playerAbilities;

	public bool devMode;
	[SerializeField] private float flySpeed;

	public int baseIntellect;
	public int baseAgility;
	public int baseStrength;
	public int baseStamina;
    public int baseDefense;
	public float healthRegenCD;
	private float healthRegenTimer;
	private int intellect;
	private int agility;
	private int strength;
	private int stamina;
    private int defense;
	public int gold;

	public Inventory inventory;
	public InventorySelect inventorySelect;
	public Inventory charPanel;
	public Inventory crafting;
	public Inventory chest;
	public Text helperText;

	public Text strengthText;
    public Text staminaText;
    public Text intellectText;
    public Text agilityText;
    public Text defenseText;
    public CanvasGroup strengthUI;
    public CanvasGroup staminaUI;
    public CanvasGroup intellectUI;
    public CanvasGroup agilityUI;
    public CanvasGroup defenseUI;

    public GameObject moneyUI;

    public Text playerNameText;

	public Stat health;
	public Stat mana;
    public Stat experience;

	[HideInInspector] public float invincTimer = 0f;
	[SerializeField] private float invincCooldown = 0f;

	public Transform spawnPoint;
	[HideInInspector] public bool isDead = false;

	[SerializeField] private Animator swordAnimator;
    public GameObject shield;
    public GameObject jetpack;

    public bool isTorched = false;

    public float regenCDChangeTimer;
    public float baseHealthRegen;
    public float manaCDTimer;
    public float baseManaRegen;

    public float criticalStrikeChance;
    public float damageIncreaseFactor;
    public float swiftnessChance;

    public GameObject playerGraphics;

    public bool inBed = false;

    public SpriteRenderer hairImage;
    public SpriteRenderer headImage;
    public SpriteRenderer bodyImage;
    public SpriteRenderer feetImage;

    public Image hairPanel;
    public Image headPanel;
    public Image bodyPanel;
    public Image feetPanel;

    public GameObject[] playerMasks;

    [SerializeField] private GameObject bloodPrefab;

	public static Player Instance;

	public int Gold
	{
		get { return gold; }
		set 
		{
			moneyUI.GetComponentInChildren<Text>().text = "Gold:" + value;
            VendorInventory.Instance.goldText.text = "Gold:" + value;
            if (RecipeGiver.Instance != null)
                RecipeGiver.Instance.goldText.text = "Gold:" + value;
            gold = value;
		}
	}

	private void Awake()
	{
        gold = 0;

        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        health.Initialize ();
		mana.Initialize ();
        experience.Initialize();

        playerNameText = GameObject.Find("NameText").GetComponent<Text>();

        playerGraphics = transform.Find("Graphics").gameObject;
    }

    private void Start()
    {
		SetStats (baseAgility, baseStrength, baseStamina, baseIntellect, baseDefense);

		if (inventorySelect != null && !inventorySelect.IsOpen)
			inventorySelect.Open (false);

        CodecManager.Instance.LoadCharacterTreeData();
        CodecManager.Instance.LoadMenuData();

        if (CodecManager.playerName != null)
        {
            Player.Instance.playerNameText.text = CodecManager.playerName;
            Player.Instance.hairImage.color = CodecManager.hairColor;
            Player.Instance.headImage.color = CodecManager.headColor;
            Player.Instance.bodyImage.color = CodecManager.bodyColor;
            Player.Instance.feetImage.color = CodecManager.feetColor;

            Player.Instance.hairPanel.color = CodecManager.hairColor;
            Player.Instance.headPanel.color = CodecManager.headColor;
            Player.Instance.bodyPanel.color = CodecManager.bodyColor;
            Player.Instance.feetPanel.color = CodecManager.feetColor;
        }
    }

    private void Update()
    {
        if (Time.time > regenCDChangeTimer)
        {
            healthRegenCD = baseHealthRegen;
        }

        if (devMode)
        {
            transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * flySpeed,
                Input.GetAxis("Vertical") * Time.deltaTime * flySpeed, 0f);
        }

		if (health.CurrentVal < health.MaxVal && health.CurrentVal > 0f && Time.time > healthRegenTimer) 
		{
			healthRegenTimer = Time.time + healthRegenCD;
			health.CurrentVal += 1f;
		}
        if (mana.CurrentVal < mana.MaxVal && Time.time > manaCDTimer)
        {
            manaCDTimer = Time.time + baseManaRegen;
            mana.CurrentVal += 1f;
        }

        if (!isDead)
		{
			if (gameObject.layer == 15 && Time.time > invincTimer)
                SetLayerRecursively (gameObject, 8);

            if (Input.GetKeyDown(KeyCode.E) && !ChatManager.Instance.chatBoxActive && !Player.Instance.inBed)
            {
                if (chest != null && inventorySelect.gameObject.activeInHierarchy)
                {
                    if (chest.canvasGroup.alpha == 0f || chest.canvasGroup.alpha == 1f)
                    {
                        if (inventory.IsOpen && crafting.IsOpen && !chest.IsOpen)
                        {
                            chest.Open(false);
                            AudioManager.instance.PlaySound("Click_Select");
                        }
                        else
                        {
                            inventory.Open(false);
                            //charPanel.Open();
                            crafting.Open(false);
                            chest.Open(false);
                            AudioManager.instance.PlaySound("Click_Select");
                        }
                    }
                }
                else if ((chest == null || chest.canvasGroup.alpha == 0f) && inventorySelect.gameObject.activeInHierarchy)
                {
                    inventory.Open(false);
                    //charPanel.Open ();
                    crafting.Open(false);
                    AudioManager.instance.PlaySound("Click_Select");
                }
            }

			if (!inventorySelect.gameObject.activeInHierarchy && inventory.IsOpen) 
				inventory.Open (false);
			//if (!inventorySelect.gameObject.activeInHierarchy && charPanel.IsOpen) 
				//charPanel.Open ();
			if (!inventorySelect.gameObject.activeInHierarchy && crafting.IsOpen) 
				crafting.Open (false);
			if (!inventorySelect.gameObject.activeInHierarchy && chest != null && chest.IsOpen) 
				chest.Open (false);
		}
    }

	public void SetLayerRecursively(GameObject obj, int newLayer)
	{
        if (obj.name != "Sword" && obj.name != "Tool" && obj.layer != 16)
		    obj.layer = newLayer;

		foreach (Transform child in obj.transform)
		{
			if (child.name != "Sword" && child.name != "Tool" && obj.layer != 16) 
			{
				SetLayerRecursively (child.gameObject, newLayer);
			}
		}
	}

	public void DamagePlayer(float damage, float knockPow, Vector2 hitNormal, float shakeAmt, float shakeTime)
	{
        if (strength > 0f)
            damage = Mathf.Ceil((damage * CharacterPanel.Instance.armorDefense) + (1f / (float)strength));
        else
            damage = Mathf.Ceil(damage * CharacterPanel.Instance.armorDefense);

        if (!devMode) 
		{
			health.CurrentVal -= damage;
			invincTimer = Time.time + invincCooldown;
            if (!inBed && health.CurrentVal > 0f)
                transform.Find("Graphics").GetComponent<Animator>().SetTrigger("Ghosted");

			CombatTextManager.Instance.CreateText (transform.position, damage.ToString (), Color.red, false, false);
            if (!Player.Instance.inBed)
            {
                KnockbackManager.Instance.ApplyKnockback(GetComponent<Rigidbody2D>(), knockPow, -hitNormal);
                CameraShake.Instance.Shake(shakeAmt, shakeTime);
            }
            GameObject tmpBlood = Instantiate(bloodPrefab, transform.position, transform.rotation);
            Destroy(tmpBlood, 2f);
            AudioManager.instance.PlaySound("Hit_Hurt");
		}

		if (health.CurrentVal <= 0f) 
		{
            MenuManager.Instance.overallDeaths++;

            int goldLoss = (int)Mathf.Ceil(GameManager.Instance.deathPenalty * Gold);
            Gold -= goldLoss;
            if (Gold < 0)
                Gold = 0;

			isDead = true;

            StartCoroutine (GameManager.Instance.RespawnPlayer (gameObject, SpawnpointManager.Instance.transform.position, goldLoss));
		}
	}

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Item")
        {
			int randomItem;
            int randomType = UnityEngine.Random.Range(0, 5);

			GameObject tmp = Instantiate(InventoryManager.Instance.itemObject);
            tmp.AddComponent<ItemScript>();

            ItemScript newEquipment = tmp.GetComponent<ItemScript>();

            switch (randomType)
            {
                case 0:
                    randomItem = UnityEngine.Random.Range(0, InventoryManager.Instance.ItemContainer.Consumeables.Count);
                    newEquipment.Item = InventoryManager.Instance.ItemContainer.Consumeables[randomItem];
                    break;

                case 1:
                    randomItem = UnityEngine.Random.Range(0, InventoryManager.Instance.ItemContainer.Weapons.Count);
                    newEquipment.Item = InventoryManager.Instance.ItemContainer.Weapons[randomItem];
                    break;

                case 2:
                    randomItem = UnityEngine.Random.Range(0, InventoryManager.Instance.ItemContainer.Equipment.Count);
                    newEquipment.Item = InventoryManager.Instance.ItemContainer.Equipment[randomItem];
                    break;

				case 3:
					randomItem = UnityEngine.Random.Range(0, InventoryManager.Instance.ItemContainer.Materials.Count);
					newEquipment.Item = InventoryManager.Instance.ItemContainer.Materials[randomItem];
					break;

				case 4:
					randomItem = UnityEngine.Random.Range(0, InventoryManager.Instance.ItemContainer.Placeables.Count);
					newEquipment.Item = InventoryManager.Instance.ItemContainer.Placeables[randomItem];
					break;
            }

            inventory.AddItem(newEquipment, true);
            Destroy(tmp);
        }

		if (other.tag == "Chest" || other.tag == "Vendor")
        {
            //helperText.gameObject.SetActive(true);
            chest = other.GetComponent<InventoryLink>().linkedInventory;
        }

        if (other.tag == "CraftingBench")
        {
            //helperText.gameObject.SetActive(true);
            chest = other.GetComponent<CraftingBenchScript>().craftingBench;
        }

        if (other.tag == "SmeltingBench")
        {
            //helperText.gameObject.SetActive(true);
            chest = other.GetComponent<SmeltingBenchScript>().smeltingBench;
        }

        if (other.tag == "Material")
        {
            for (int i = 0; i < 5; i++)
            {
                for (int x = 0; x < 3; x++)
                {
                    GameObject tmp = Instantiate(InventoryManager.Instance.itemObject);
                    tmp.AddComponent<ItemScript>();

                    ItemScript newMaterial = tmp.GetComponent<ItemScript>();
                    newMaterial.Item = InventoryManager.Instance.ItemContainer.Materials[x];

                    inventory.AddItem(newMaterial, true);
                    Destroy(tmp);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
		if (other.gameObject.tag == "Chest" || other.gameObject.tag == "CraftingBench" || other.gameObject.tag == "SmeltingBench" || 
            other.gameObject.tag == "Vendor")
        {
            //helperText.gameObject.SetActive(false);
         
            if (chest != null && chest.IsOpen)
                chest.Open(false);
            chest = null;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "DroppedItem")
        {
            if (collision.gameObject.GetComponent<DelayPickup>().canPickup)
            {
                GameObject tmp = Instantiate(InventoryManager.Instance.itemObject);
                tmp.AddComponent<ItemScript>();

                ItemScript newDrop = tmp.GetComponent<ItemScript>();
                var type = collision.gameObject.GetComponent<SpriteManager>().ItemType;
                switch (type)
                {
                    case ItemType.PLACEABLE:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Placeables[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                    case ItemType.CONSUMEABLE:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Consumeables[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                    case ItemType.MATERIAL:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Materials[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                    case ItemType.GENERICWEAPON:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Weapons[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                    case ItemType.BELT:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Equipment[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                }

                if (!inventorySelect.AddItem(newDrop, true))
                    inventory.AddItem(newDrop, true);

                Destroy(tmp);
                Destroy(collision.gameObject);
            }
        }

        if (collision.gameObject.tag == "Item")
        {
            for (int i = 0; i < collision.gameObject.GetComponent<ItemScript>().itemCount; i++)
            {
                inventory.AddItem(collision.gameObject.GetComponent<ItemScript>(), true);
            }
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.gameObject.tag == "DroppedItem")
		{
            if (collision.gameObject.GetComponent<DelayPickup>().canPickup)
            {
                GameObject tmp = Instantiate(InventoryManager.Instance.itemObject);
                tmp.AddComponent<ItemScript>();

                ItemScript newDrop = tmp.GetComponent<ItemScript>();
                var type = collision.gameObject.GetComponent<SpriteManager>().ItemType;
                switch (type)
                {
                    case ItemType.PLACEABLE:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Placeables[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                    case ItemType.CONSUMEABLE:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Consumeables[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                    case ItemType.MATERIAL:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Materials[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                    case ItemType.GENERICWEAPON:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Weapons[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                    case ItemType.BELT:
                        newDrop.Item = InventoryManager.Instance.ItemContainer.Equipment[collision.gameObject.GetComponent<SpriteManager>().ListIndex];
                        break;
                }

                if (!inventorySelect.AddItem(newDrop, true))
                    inventory.AddItem(newDrop, true);

                Destroy(tmp);
                Destroy(collision.gameObject);
            }
		}

        if (collision.gameObject.tag == "Item")
        {
            for (int i = 0; i < collision.gameObject.GetComponent<ItemScript>().itemCount; i++)
            {
                inventory.AddItem(collision.gameObject.GetComponent<ItemScript>(), true);
            }
            Destroy(collision.gameObject);
        }
    }

    public void SetStats(int agility, int strength, int stamina, int intellect, int defense)
    {
        if (this.agility > 0f)
            GetComponent<PlatformerCharacter2D>().m_MaxSpeed -= (1f / (float)this.agility);
        if (this.stamina > 0f)
            mana.MaxVal -= (1f / (float)this.stamina);
        if (this.intellect > 0f)
            manaCDTimer -= (1f / (float)this.intellect);

        this.agility = agility + baseAgility;
        this.strength = strength + baseStrength;
        this.stamina = stamina + baseStamina;
        this.intellect = intellect + baseIntellect;
        this.defense = defense;

        if (this.agility > 0f)
            GetComponent<PlatformerCharacter2D>().m_MaxSpeed += (1f / (float)this.agility);
        if (this.stamina > 0f)
            mana.MaxVal += (1f / (float)this.stamina);
        if (this.intellect > 0f)
            manaCDTimer += (1f / (float)this.intellect);

        strengthText.text = string.Format("STRENGTH: {0}", this.strength);
        staminaText.text = string.Format("STAMINA: {0}", this.stamina);
        intellectText.text = string.Format("INTELLECT: {0}", this.intellect);
        agilityText.text = string.Format("AGILITY: {0}", this.agility);
        defenseText.text = string.Format("DEFENSE: {0}", this.defense);
    }

    public void OnStrengthEnter()
    {
        strengthUI.alpha = 1f;
        strengthUI.interactable = true;
        strengthUI.blocksRaycasts = true;
    }

    public void OnStrengthExit()
    {
        strengthUI.alpha = 0f;
        strengthUI.interactable = false;
        strengthUI.blocksRaycasts = false;
    }

    public void OnStaminaEnter()
    {
        staminaUI.alpha = 1f;
        staminaUI.interactable = true;
        staminaUI.blocksRaycasts = true;
    }

    public void OnStaminaExit()
    {
        staminaUI.alpha = 0f;
        staminaUI.interactable = false;
        staminaUI.blocksRaycasts = false;
    }

    public void OnIntellectEnter()
    {
        intellectUI.alpha = 1f;
        intellectUI.interactable = true;
        intellectUI.blocksRaycasts = true;
    }

    public void OnIntellectExit()
    {
        intellectUI.alpha = 0f;
        intellectUI.interactable = false;
        intellectUI.blocksRaycasts = false;
    }

    public void OnAgilityEnter()
    {
        agilityUI.alpha = 1f;
        agilityUI.interactable = true;
        agilityUI.blocksRaycasts = true;
    }

    public void OnAgilityExit()
    {
        agilityUI.alpha = 0f;
        agilityUI.interactable = false;
        agilityUI.blocksRaycasts = false;
    }

    public void OnDefenseEnter()
    {
        defenseUI.alpha = 1f;
        defenseUI.interactable = true;
        defenseUI.blocksRaycasts = true;
    }

    public void OnDefenseExit()
    {
        defenseUI.alpha = 0f;
        defenseUI.interactable = false;
        defenseUI.blocksRaycasts = false;
    }
}

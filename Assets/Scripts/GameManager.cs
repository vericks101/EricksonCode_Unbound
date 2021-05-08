using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityStandardAssets._2D;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class ArmItem
{
	public string name;
	public GameObject item;
}

public class GameManager : MonoBehaviour
{
    public ArmItem[] armItems;

    [SerializeField] private GameObject charPanel;
    public GameObject invSelect;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Text goldLossText;
    [SerializeField] private GameObject noticePanel;

    private GameObject cameraObj;
    public bool restrictX;
    public bool restrictY;
    public GameObject terrainMap;
    [SerializeField] private float offsetBorderAllowed;

    public float liquidDmgCooldown;
    [HideInInspector] public float liquidCooldownTimer;
    public float inWaterGravity;
    public float normalGavity;

    private bool firstFrame = true;

    public GameObject spawnPoint;

    public Font defaultFont;

    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;

    public static bool canUseWeapons = true;

    public float greenSpreadCD;
    [HideInInspector] public float timeTillSpread;

    public float eventCD;

    private float fadeTime = 0f;
    private bool canFade = false;

    public float deathPenalty;

    public bool battleReady = false;
    public GameObject battleReadyObj;
    public GameObject battleNotReadyObj;
    public string battleReadyString;
    public string battleNotReadyString;
    public Text battleReadyText;

    public static GameManager Instance;

    void Awake()
    {
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

        cameraObj = GameObject.Find("Camera");
        terrainMap = GameObject.Find("Terrain");
    }

    void OnLevelWasLoaded()
    {
        terrainMap = GameObject.Find("Terrain");
        spawnPoint = GameObject.Find("SpawnPoint");
        for (int i = 0; i < armItems.Length; i++)
        {
            if (armItems[i].item.GetComponent<SpriteRenderer>() != null && armItems[i].item.layer == 16)
                armItems[i].item.GetComponent<SpriteRenderer>().enabled = false;
        }

        restrictX = false;
        restrictY = false;
    }

    void Start()
    {
        DialogueManager.Instance.dialogueBox.SetActive(false);

        Player.Instance.transform.position = spawnPoint.transform.position;

        var textComponents = Component.FindObjectsOfType<Text>();
        foreach (var component in textComponents)
        {
            component.font = defaultFont;
        }
    }

    void Update()
    {
        if (firstFrame)
        {
            CodecManager.Instance.LoadInventories();
            firstFrame = false;
        }

        if (terrainMap != null && TerrainManager.Instance.enableBorders)
        {
            if (Player.Instance.transform.position.x >= terrainMap.GetComponent<TerrainMap>().mapWidth - offsetBorderAllowed
                || Player.Instance.transform.position.x <= offsetBorderAllowed)
            {
                cameraObj.GetComponent<Camera2DFollow>().target = null;
                restrictX = true;
            }
            if (Player.Instance.transform.position.y >= terrainMap.GetComponent<TerrainMap>().mapHeight - offsetBorderAllowed
                || Player.Instance.transform.position.y <= offsetBorderAllowed)
            {
                cameraObj.GetComponent<Camera2DFollow>().target = null;
                restrictY = true;
            }


            if (cameraObj.GetComponent<Camera2DFollow>().target == null || restrictX || restrictY)
            {
                if ((Player.Instance.transform.position.x <= terrainMap.GetComponent<TerrainMap>().mapWidth - offsetBorderAllowed && Player.Instance.transform.position.x >= terrainMap.GetComponent<TerrainMap>().mapWidth - (offsetBorderAllowed * 2))
                || (Player.Instance.transform.position.x >= offsetBorderAllowed && Player.Instance.transform.position.x <= terrainMap.GetComponent<TerrainMap>().mapWidth - (offsetBorderAllowed * 2)))
                {
                    cameraObj.GetComponent<Camera2DFollow>().target = Player.Instance.transform;
                    restrictX = false;
                }
                if ((Player.Instance.transform.position.y <= terrainMap.GetComponent<TerrainMap>().mapHeight - offsetBorderAllowed && Player.Instance.transform.position.y >= terrainMap.GetComponent<TerrainMap>().mapHeight - (offsetBorderAllowed * 2))
                || (Player.Instance.transform.position.y >= offsetBorderAllowed && Player.Instance.transform.position.y <= terrainMap.GetComponent<TerrainMap>().mapHeight - (offsetBorderAllowed * 2)))
                {
                    cameraObj.GetComponent<Camera2DFollow>().target = Player.Instance.transform;
                    restrictY = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !Player.Instance.isDead && !ChatManager.Instance.chatBoxActive && Player.Instance.shield.activeInHierarchy && !armItems[0].item.GetComponent<SpriteRenderer>().enabled)
            SwitchSelect(true);
        else if (Input.GetKeyDown(KeyCode.Tab) && !Player.Instance.isDead && !ChatManager.Instance.chatBoxActive && !armItems[0].item.GetComponent<SpriteRenderer>().enabled)
            SwitchSelect(false);

		if (charPanel.transform.Find ("MainHand").GetComponent<Slot> ().Items.Count <= 0 && !invSelect.activeInHierarchy) 
		{
			foreach (ArmItem ai in armItems)
			{
				ai.item.SetActive (false);
			}
		}

        if (Input.GetMouseButtonDown(1) && !Player.Instance.inBed)
        {
            if (!Player.Instance.shield.activeInHierarchy)
                SwitchHand(true);
            else
                SwitchHand(false);
        }

        if (Time.time > fadeTime && canFade)
        {
            LoadingScreenManager.LoadScene(1);
            canFade = false;
        }
    }

	public IEnumerator RespawnPlayer(GameObject player, Vector3 spawnPoint, int goldLoss)
	{
        goldLossText.text = "dropped " + goldLoss + " gold";
		deathScreen.GetComponent<CanvasGroup> ().alpha = 1f;
		player.GetComponent<Platformer2DUserControl> ().enabled = false;
        //player.transform.Find("Graphics").GetComponent<SpriteRenderer>().enabled = false;
        //player.transform.Find("Arm").GetComponent<SpriteRenderer>().enabled = false;
        Player.Instance.GetComponent<BoxCollider2D>().enabled = false;
        player.GetComponent<CircleCollider2D>().enabled = false;
        //player.transform.Find("Graphics").gameObject.SetActive(false);
        //player.transform.Find("Arm").gameObject.SetActive(false);
        player.GetComponent<Rigidbody2D> ().drag = 100f;
		cameraObj.GetComponent<Camera2DFollow> ().enabled = false;
        MenuManager.Instance.aiObject.SetActive(false);
        //MenuManager.Instance.playerReturnButton.SetActive(false);

        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer.enabled = false;
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer.enabled = false;
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer.enabled = false;
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer.enabled = false;
        }

        yield return new WaitForSeconds (3f);

        if (!GameManager.Instance.invSelect.activeInHierarchy && Player.Instance.shield.activeInHierarchy)
            GameManager.Instance.SwitchSelect(true);
        else if (!GameManager.Instance.invSelect.activeInHierarchy)
            GameManager.Instance.SwitchSelect(false);

        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            CodecManager.Instance.SaveInventories();
            CodecManager.Instance.SaveTerrain();
            CodecManager.Instance.SavePlayerData();
            CodecManager.Instance.SavePreviewData();
            CodecManager.Instance.SaveCharacterTreeData();
            CodecManager.Instance.SaveMenuData();
            CodecManager.Instance.SaveQuestData();

            Player.Instance.GetComponent<FallDamageManager>().enabled = false;

            StartCoroutine(CurrentSceneManager.Instance.FadeOut());
            fadeTime = GameManager.Instance.GetComponent<FadingManager>().BeginFade(1) + Time.time;
            canFade = true;

            if (Player.Instance.chest != null && Player.Instance.chest.IsOpen)
                Player.Instance.chest.Open(false);
            Player.Instance.chest = null;

            if (CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent != null)
                CurrentSceneManager.Instance.GetComponent<EventManager>().EndEvent(false);
            SpawningManager.Instance.KillMobs();
        }

        deathScreen.GetComponent<CanvasGroup>().alpha = 0f;

        if (SceneManager.GetActiveScene().buildIndex != 1)
            yield return new WaitForSeconds(3f);

        player.GetComponent<Platformer2DUserControl>().enabled = true;
        //player.transform.Find("Graphics").GetComponent<SpriteRenderer>().enabled = true;
        //player.transform.Find("Arm").GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<BoxCollider2D>().enabled = true;
        player.GetComponent<CircleCollider2D>().enabled = true;
        //player.transform.Find("Graphics").gameObject.SetActive(true);
        //player.transform.Find("Arm").gameObject.SetActive(true);
        player.GetComponent<Rigidbody2D>().drag = 0f;
        cameraObj.GetComponent<Camera2DFollow>().enabled = true;

        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer != null)
            {
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer.enabled = true;
                if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer.gameObject.tag == "PlayerMasked")
                    GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer != null)
            {
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer.enabled = true;
                if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer.gameObject.tag == "PlayerMasked")
                    GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer != null)
            {
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer.enabled = true;
                if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer.gameObject.tag == "PlayerMasked")
                    GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer != null)
            {
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer.enabled = true;
                if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer.gameObject.tag == "PlayerMasked")
                    GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer.color = new Color(0f, 0f, 0f, 0f);
            }
        }

        Player.Instance.isDead = false;
        Player.Instance.health.CurrentVal = Player.Instance.health.MaxVal;
        Player.Instance.mana.CurrentVal = Player.Instance.mana.MaxVal;
        Player.Instance.transform.position = new Vector3(this.spawnPoint.transform.position.x, this.spawnPoint.transform.position.y, this.spawnPoint.transform.position.z);
        Player.Instance.GetComponent<Rigidbody2D>().gravityScale = normalGavity;

        CodecManager.Instance.SavePlayerData();
        CodecManager.Instance.SavePreviewData();
    }

	public void AcceptNotice()
	{
		noticePanel.SetActive (false);
	}

	public void SwitchSelect(bool tabWithShield)
	{
		if ((!battleReady || Player.Instance.shield.activeInHierarchy) && !tabWithShield) 
		{
            battleReady = true;
            battleNotReadyObj.GetComponent<Image>().color = new Color(battleNotReadyObj.GetComponent<Image>().color.r, battleNotReadyObj.GetComponent<Image>().color.b,
                battleNotReadyObj.GetComponent<Image>().color.g, 0.25f);
            battleReadyObj.GetComponent<Image>().color = new Color(battleReadyObj.GetComponent<Image>().color.r, battleReadyObj.GetComponent<Image>().color.b,
                battleReadyObj.GetComponent<Image>().color.g, 1f);
            battleReadyText.text = battleReadyString;
			//invSelect.SetActive (false);
            //InventoryManager.Instance.tooltipObject.SetActive(false);
            
            if (charPanel.transform.Find ("MainHand").GetComponent<Slot> ().Items.Count > 0) 
			{
				var itemName = charPanel.transform.Find ("MainHand").GetComponent<Slot> ().CurrentItem.Item.ItemName;
				if (itemName.Contains("Nanosaber")) 
					armItems [0].item.SetActive (true);
				else if (itemName == "Handgun") 
					armItems [1].item.SetActive (true);
				else if (itemName == "Assault Rifle") 
					armItems [2].item.SetActive (true);
                else if (itemName == "Rocket Launcher")
                    armItems[3].item.SetActive(true);
            }
		}
		else 
		{
            //invSelect.SetActive (true);
            battleReady = false;
            battleNotReadyObj.GetComponent<Image>().color = new Color(battleNotReadyObj.GetComponent<Image>().color.r, battleNotReadyObj.GetComponent<Image>().color.b,
                battleNotReadyObj.GetComponent<Image>().color.g, 1f);
            battleReadyObj.GetComponent<Image>().color = new Color(battleReadyObj.GetComponent<Image>().color.r, battleReadyObj.GetComponent<Image>().color.b,
                battleReadyObj.GetComponent<Image>().color.g, 0.25f);
            battleReadyText.text = battleNotReadyString;

            foreach (ArmItem ai in armItems)
            {
                ai.item.SetActive(false);
            }
            Player.Instance.shield.SetActive(false);
		}
	}

    void SwitchHand(bool offHand)
    {
        if (offHand)
        {
            if (charPanel.transform.Find("OffHand").GetComponent<Slot>().Items.Count > 0)
            {
                Player.Instance.shield.SetActive(true);
                Player.Instance.GetComponentInChildren<Shield>().shieldDef = Player.Instance.GetComponentInChildren<Shield>().baseShieldDef / 
                    charPanel.transform.Find("OffHand").GetComponent<Slot>().CurrentItem.GetDefense();
            }
        }
        else
        {
            SwitchSelect(false);
            Player.Instance.shield.SetActive(false);
        }
    }

    public void OnMouseOver()
    {
        AudioManager.instance.PlaySound("Hover_Over");
    }

    public void OnMouseClick()
    {
        AudioManager.instance.PlaySound("Select_Pickup");
    }

    public void OnBattleReadyEnter()
    {
        battleReadyText.transform.parent.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void OnBattleReadyExit()
    {
        battleReadyText.transform.parent.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
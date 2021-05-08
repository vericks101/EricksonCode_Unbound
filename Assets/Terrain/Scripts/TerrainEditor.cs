using UnityEngine;
using System.Collections;

public enum MapType { FRONT, BRUSH, BACK }

public class TerrainEditor : BaseGenerator
{
	public float digDistance = 3f;
	public float placeCooldown = 0.15f;
	public float hitCooldown = 1f;
	public int hitDamage = 50;
	public float breakForce = 50f;
	private float hitTime = 0f;
	private float placeTime = 0f;
	private float breakXCone = 100f;
	private LayerMask layermask;
	private bool cantPlace = false;

    [SerializeField] private LayerMask hitMask;
	[SerializeField] private LayerMask defaultMask;
	[SerializeField] private LayerMask pickaxeMask;
	[SerializeField] private LayerMask axeMask;
	[SerializeField] private LayerMask hammerMask;

    private ToolManager toolTM;

    public GameObject dropItem;

	private Transform player;

    [SerializeField] private float liquidUpdatePeriod;

    [SerializeField] private GameObject hitParticles;

	void Awake()
	{
		if (player == null)
            player = GameObject.Find("Player").GetComponent<Transform>();

        toolTM = FindObjectOfType<ToolManager>();
	}

	void FixedUpdate ()
	{
        if (toolTM == null)
            toolTM = FindObjectOfType<ToolManager>();

		if(Input.GetMouseButton (0) && Time.time >= hitTime && Player.Instance.inventorySelect.gameObject.activeInHierarchy && !Player.Instance.inBed 
            && DestinationManager.Instance.GetComponent<CanvasGroup>().alpha <= 0f && MenuManager.Instance.GetComponent<CanvasGroup>().alpha <= 0f)
		{
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			clickPosition = new Vector3 (Mathf.RoundToInt(clickPosition.x), Mathf.RoundToInt(clickPosition.y), 0);

			float distance = Vector2.Distance(clickPosition, player.position);
			if(distance < digDistance && InventoryManager.Instance.SelectedSlot.Items.Count > 0)
			{
                if (InventoryManager.Instance.SelectedSlot.CurrentItem.GetAttackSpeed() != 0 && (InventoryManager.Instance.SelectedSlot.CurrentItem.GetElement() == CurrentSceneManager.Instance.planetElement
                    || InventoryManager.Instance.SelectedSlot.CurrentItem.GetElement() == Element.LUMINANT || InventoryManager.Instance.SelectedSlot.CurrentItem.GetElement() == Element.COMMON))
                    hitCooldown = InventoryManager.Instance.SelectedSlot.CurrentItem.GetAttackSpeed();
                else
                    hitCooldown = 1f;

                if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemType.ToString() == "PICKAXE")
                {
                    layermask = pickaxeMask;
                    toolTM.SwingTool();
                }
                else if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemType.ToString() == "AXE")
                {
                    layermask = axeMask;
                    toolTM.SwingTool();
                }
                else if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemType.ToString() == "HAMMER")
                {
                    layermask = hammerMask;
                    toolTM.SwingTool();
                }
                else
                    layermask = defaultMask;

				RaycastHit2D hit = Physics2D.Linecast (clickPosition, clickPosition, layermask);
				if (hit.collider != null && hit.collider != player.GetComponent<BoxCollider2D>() && hit.collider != player.GetComponent<CircleCollider2D>())
				{
                    if (hit.transform.gameObject.GetComponent<SpriteManager>() != null)
                    {
                        GameObject tmpParticles = Instantiate(hitParticles, clickPosition, transform.rotation);
                        Destroy(tmpParticles, 2f);
                        AudioManager.instance.PlaySound("ToolHit");
                        hit.transform.gameObject.GetComponent<SpriteManager>().DamageSprite(hitDamage);
                        //hit.transform.gameObject.GetComponent<Animator>().SetTrigger("BlockHit");
                        if (hit.transform.gameObject.GetComponent<SpriteManager>().Health <= 0)
                        {
                            bool itemFound = false;
                            if (hit.transform.gameObject.GetComponent<InventoryLink>() != null)
                            {
                                for (int i = 0; i < hit.transform.gameObject.GetComponent<InventoryLink>().allSlots.Count; i++)
                                {
                                    if (hit.transform.gameObject.GetComponent<InventoryLink>().allSlots[i] != null && hit.transform.gameObject.GetComponent<InventoryLink>().allSlots[i].Count > 0)
                                    {
                                        itemFound = true;
                                        break;
                                    }
                                }
                            }
                            if (!itemFound)
                                DestructTile(hit, clickPosition);
                        }
                        hitTime = Time.time + 1 / hitCooldown;
                    }
				}
			}
		}

		if (Input.GetMouseButton (1) && Player.Instance.inventorySelect.gameObject.activeInHierarchy 
            && InventoryManager.Instance.SelectedSlot.Items.Count > 0 && InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemType == ItemType.PLACEABLE && !Player.Instance.inBed
            && DestinationManager.Instance.GetComponent<CanvasGroup>().alpha <= 0f && MenuManager.Instance.GetComponent<CanvasGroup>().alpha <= 0f)
		{
			TerrainRenderer terrainRenderer = GetComponent<TerrainRenderer>();

			Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			clickPosition = new Vector3 (Mathf.RoundToInt(clickPosition.x), Mathf.RoundToInt(clickPosition.y), 0);

			float distance = Vector2.Distance(clickPosition, player.position);
			if (distance < digDistance)
			{
				RaycastHit2D hit = Physics2D.Linecast (clickPosition, clickPosition, hitMask);

                if (hit.collider != null && hit.collider.gameObject.layer == 10 && InventoryManager.Instance.SelectedSlot.Items.Count > 0 && InventoryManager.Instance.SelectedSlot.Items.Peek().Item.ItemID == 71)
                {
                    InventoryManager.Instance.SelectedSlot.RemoveItem();

                    if (InventoryManager.Instance.SelectedSlot.Items.Count <= 0)
                        InventoryManager.Instance.tooltipObject.SetActive(false);
                    Player.Instance.inventorySelect.ChangeCurrentItemText();

                    GameObject liquidObj = (GameObject)terrainRenderer.renderedChunks["x: " + (int)clickPosition.x + " y: " + (int)clickPosition.y];

                    if (liquidObj.GetComponent<SpriteManager>().ItemID == 7)
                        GiveItem("Water", 1);
                    else if (liquidObj.GetComponent<SpriteManager>().ItemID == 24)
                        GiveItem("Lava", 1);

                    CurrentMap[(int)clickPosition.x, (int)clickPosition.y] = 0;
                    terrainRenderer.renderedChunks.Remove("x: " + (int)clickPosition.x + " y: " + (int)clickPosition.y);
                    ObjectPooler.current.pooledObjects.Remove(liquidObj);
                    Destroy(liquidObj);

                    StartCoroutine(SettleLiquids((int)clickPosition.x, (int)clickPosition.y));

                    return;
                }

                if ((hit.collider == null || hit.collider.gameObject.layer == 10) && hit.collider != player.GetComponent<BoxCollider2D>() && hit.collider != player.GetComponent<CircleCollider2D>()
                    && Time.time >= placeTime && InventoryManager.Instance.SelectedSlot.Items.Count > 0 && InventoryManager.Instance.SelectedSlot.Items.Peek().Item.ItemID != 71)
                {
                    if ((CurrentMap[(int)clickPosition.x, (int)clickPosition.y] == 0 || CurrentMap[(int)clickPosition.x, (int)clickPosition.y] == 7 || CurrentMap[(int)clickPosition.x, (int)clickPosition.y] == 24) 
                        && CurrentBrushMap[(int)clickPosition.x, (int)clickPosition.y] == 0)
                    {
                        if ((InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 12 && CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y] == 0)
                            || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID == 70)
                        {
                            if (((CurrentMap[(int)clickPosition.x, (int)clickPosition.y - 1] != 0 || CurrentMap[(int)clickPosition.x,
                                (int)clickPosition.y + 1] != 0 || CurrentMap[(int)clickPosition.x - 1, (int)clickPosition.y] != 0 ||
                                CurrentMap[(int)clickPosition.x + 1, (int)clickPosition.y] != 0)) ||
                                ((CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y - 1] != 0 ||
                                CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y + 1] != 0 ||
                                CurrentBackMap[(int)clickPosition.x - 1, (int)clickPosition.y] != 0 ||
                                CurrentBackMap[(int)clickPosition.x + 1, (int)clickPosition.y] != 0 ||
                                CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y] != 0)))
                            {
                                cantPlace = false;

                                if (InventoryManager.Instance.SelectedSlot.Items.Count > 0)
                                {
                                    if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Width != 0 || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Height != 0)
                                    {
                                        for (int x = 0; x < InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Width; x++)
                                        {
                                            for (int y = 0; y < InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Height; y++)
                                            {
                                                if ((CurrentMap[(int)clickPosition.x + x, (int)clickPosition.y - 1] == 0 || CurrentMap[(int)clickPosition.x + x,
                                                    (int)clickPosition.y + y] != 0) || CurrentBrushMap[(int)clickPosition.x + x, (int)clickPosition.y + y] != 0)
                                                    cantPlace = true;
                                            }
                                        }

                                        if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID == 70)
                                        {
                                            for (int x = 0; x < InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Width; x++)
                                            {
                                                for (int y = 0; y < InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Height; y++)
                                                {
                                                    if ((CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y] != 0 && CurrentBackMap[(int)clickPosition.x + 1, (int)clickPosition.y] != 0))
                                                        cantPlace = false;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!cantPlace)
                                {
                                    placeTime = Time.time + placeCooldown;

                                    if (InventoryManager.Instance.SelectedSlot.Items.Count > 0)
                                    {
                                        try
                                        {
                                            if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 0 || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 10)
                                                CurrentMap[(int)clickPosition.x, (int)clickPosition.y] = InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID;
                                            else if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 9 || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 18 || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 19)
                                            {
                                                CurrentBrushMap[(int)clickPosition.x, (int)clickPosition.y] = InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID;
                                            }
                                            else
                                                CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y] = InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID;

                                            AudioManager.instance.PlaySound("ToolHit");
                                            terrainRenderer.SingleRender((int)clickPosition.x, (int)clickPosition.y, InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID, InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID);
                                            InventoryManager.Instance.SelectedSlot.RemoveItem();
                                            StartCoroutine(SettleLiquids((int)clickPosition.x, (int)clickPosition.y));
                                        }
                                        catch (System.NullReferenceException)
                                        { }
                                    }
                                }
                            }
                        }
                        if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item != null && InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID != 12)
                        {
                            if (((CurrentMap[(int)clickPosition.x, (int)clickPosition.y - 1] != 0 || CurrentMap[(int)clickPosition.x,
                                (int)clickPosition.y + 1] != 0 || CurrentMap[(int)clickPosition.x - 1, (int)clickPosition.y] != 0 ||
                                CurrentMap[(int)clickPosition.x + 1, (int)clickPosition.y] != 0)) ||
                                ((CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y] != 0)))
                            {
                                cantPlace = false;

                                if (InventoryManager.Instance.SelectedSlot.Items.Count > 0)
                                {
                                    if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Width != 0 || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Height != 0)
                                    {
                                        for (int x = 0; x < InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Width; x++)
                                        {
                                            for (int y = 0; y < InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Height; y++)
                                            {
                                                if ((CurrentMap[(int)clickPosition.x + x, (int)clickPosition.y - 1] == 0 || CurrentMap[(int)clickPosition.x + x,
                                                    (int)clickPosition.y + y] != 0) || CurrentBrushMap[(int)clickPosition.x + x, (int)clickPosition.y + y] != 0)
                                                    cantPlace = true;
                                            }
                                        }
                                    }

                                    if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 19 && CurrentMap[(int)clickPosition.x, (int)clickPosition.y - 1] == 0
                                        && InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID != 12)
                                        cantPlace = true;
                                }

                                if (!cantPlace)
                                {
                                    placeTime = Time.time + placeCooldown;

                                    if (InventoryManager.Instance.SelectedSlot.Items.Count > 0)
                                    {
                                        try
                                        {
                                            if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 0 || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 10
                                                || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID == 69 || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID == 70)
                                                CurrentMap[(int)clickPosition.x, (int)clickPosition.y] = InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID;
                                            else if (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 9 || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 18 || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID == 19)
                                            {
                                                CurrentBrushMap[(int)clickPosition.x, (int)clickPosition.y] = InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID;
                                            }
                                            else
                                                CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y] = InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID;

                                            AudioManager.instance.PlaySound("ToolHit");
                                            if (terrainRenderer.renderedChunks.ContainsKey("x: " + (int)clickPosition.x + " y: " + (int)clickPosition.y))
                                            {
                                                GameObject liquidObj = (GameObject)terrainRenderer.renderedChunks["x: " + (int)clickPosition.x + " y: " + (int)clickPosition.y];
                                                terrainRenderer.renderedChunks.Remove("x: " + (int)clickPosition.x + " y: " + (int)clickPosition.y);
                                                ObjectPooler.current.pooledObjects.Remove(liquidObj);
                                                Destroy(liquidObj);
                                            }
                                            terrainRenderer.SingleRender((int)clickPosition.x, (int)clickPosition.y, InventoryManager.Instance.SelectedSlot.CurrentItem.Item.LayerID, InventoryManager.Instance.SelectedSlot.CurrentItem.Item.ItemID);

                                            if (InventoryManager.Instance.SelectedSlot.Items.Count > 0 && (InventoryManager.Instance.SelectedSlot.Items.Peek().Item.ItemID == 7 ||
                                                InventoryManager.Instance.SelectedSlot.Items.Peek().Item.ItemID == 24))
                                                GiveItem("Bucket", 1);

                                            InventoryManager.Instance.SelectedSlot.RemoveItem();
                                            StartCoroutine(SettleLiquids((int)clickPosition.x, (int)clickPosition.y));
                                        }
                                        catch (System.NullReferenceException)
                                        { }
                                    }
                                }
                            }
                        }
                    }
                }
			}
		}
	}

	IEnumerator SettleLiquids(int x, int y)
	{
        yield return new WaitForSeconds(liquidUpdatePeriod);

        TerrainRenderer terrainRenderer = GetComponent<TerrainRenderer>();

		if (CurrentMap [x + 1, y] == 7 || CurrentMap [x - 1, y] == 7 || CurrentMap [x, y + 1] == 7 || CurrentMap[x, y] == 7) 
		{
			CurrentMap [x, y] = 7;
			terrainRenderer.SingleRender (x, y, 10, 7);

            if ((CurrentMap[x + 1, y] == 0 && CurrentMap[x + 1, y - 1] != 0) 
                || (CurrentMap[x + 1, y] == 0 && CurrentMap[x, y - 1] != 0 && CurrentMap[x, y - 1] != 7))
                StartCoroutine(SettleLiquids(x + 1, y));
            if (CurrentMap[x - 1, y] == 0 && CurrentMap[x - 1, y - 1] != 0 
                || (CurrentMap[x - 1, y] == 0 && CurrentMap[x, y - 1] != 0 && CurrentMap[x, y - 1] != 7))
                StartCoroutine(SettleLiquids(x - 1, y));
            if (CurrentMap[x, y - 1] == 0)
                StartCoroutine(SettleLiquids(x, y - 1));
        }
        if (CurrentMap[x + 1, y] == 24 || CurrentMap[x - 1, y] == 24 || CurrentMap[x, y + 1] == 24 || CurrentMap[x, y] == 24)
        {
            CurrentMap[x, y] = 24;
            terrainRenderer.SingleRender(x, y, 10, 24);

            if ((CurrentMap[x + 1, y] == 0 && CurrentMap[x + 1, y - 1] != 0)
                || (CurrentMap[x + 1, y] == 0 && CurrentMap[x, y - 1] != 0 && CurrentMap[x, y - 1] != 24))
                StartCoroutine(SettleLiquids(x + 1, y));
            if (CurrentMap[x - 1, y] == 0 && CurrentMap[x - 1, y - 1] != 0
                || (CurrentMap[x - 1, y] == 0 && CurrentMap[x, y - 1] != 0 && CurrentMap[x, y - 1] != 24))
                StartCoroutine(SettleLiquids(x - 1, y));
            if (CurrentMap[x, y - 1] == 0)
                StartCoroutine(SettleLiquids(x, y - 1));
        }
    }

    public void DestructTile(RaycastHit2D hit, Vector3 clickPosition)
    {
        GameObject tmpNeighbor = new GameObject();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(hit.collider.gameObject.transform.position, .5f);
        for (int j = 0; j < hitColliders.Length; j++)
        {
            if ((int)hitColliders[j].transform.position.y == (int)hit.collider.gameObject.transform.position.y + 1f && hitColliders[j].GetComponent<TerrainLighting>() == null
                && hitColliders[j].gameObject.layer != 12)
                tmpNeighbor = hitColliders[j].gameObject;
        }

        GameObject go = (GameObject)Instantiate(dropItem, hit.transform.gameObject.transform.position, Quaternion.identity);
        go.GetComponent<SpriteManager>().ItemName = hit.transform.gameObject.GetComponent<SpriteManager>().ItemName;
        go.GetComponent<SpriteManager>().ItemID = hit.transform.gameObject.GetComponent<SpriteManager>().ItemID;
        go.GetComponent<SpriteManager>().ListIndex = hit.transform.gameObject.GetComponent<SpriteManager>().ListIndex;
        go.GetComponent<SpriteManager>().ItemType = hit.transform.gameObject.GetComponent<SpriteManager>().ItemType;

        SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = hit.transform.gameObject.GetComponent<SpriteRenderer>().sprite;

        go.AddComponent<BoxCollider2D>();
        go.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
        go.GetComponent<BoxCollider2D>().size = new Vector2(0.9f, 0.9f);
        float randomXDir = Random.Range(-breakXCone, breakXCone);
        go.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomXDir, breakForce));

        //GameObject tmpLeftNeighbor = new GameObject();
        if (hit.transform.localScale.x > 1f)
        {
            if (CurrentBrushMap[(int)clickPosition.x - 1, (int)clickPosition.y] == hit.transform.GetComponent<SpriteManager>().ItemID)
                CurrentBrushMap[(int)clickPosition.x - 1, (int)clickPosition.y] = 0;
            if (CurrentMap[(int)clickPosition.x - 1, (int)clickPosition.y] == hit.transform.GetComponent<SpriteManager>().ItemID)
                CurrentMap[(int)clickPosition.x - 1, (int)clickPosition.y] = 0;
        }
        if (hit.transform.localScale.y > 1f)
        {
            if (CurrentMap[(int)clickPosition.x, (int)clickPosition.y - 1] == hit.transform.GetComponent<SpriteManager>().ItemID)
                CurrentMap[(int)clickPosition.x, (int)clickPosition.y - 1] = 0;
            if (CurrentMap[(int)clickPosition.x, (int)clickPosition.y - 2] == hit.transform.GetComponent<SpriteManager>().ItemID)
                CurrentMap[(int)clickPosition.x, (int)clickPosition.y - 2] = 0;
        }

        if (hit.transform.GetComponent<SpriteManager>().LayerID == 0)
            CurrentMap[(int)clickPosition.x, (int)clickPosition.y] = 0;
        else if (hit.transform.gameObject.layer == 9 || hit.transform.gameObject.layer == 10 || hit.transform.gameObject.layer == 18 || hit.transform.gameObject.layer == 19)
            CurrentBrushMap[(int)clickPosition.x, (int)clickPosition.y] = 0;
        else
            CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y] = 0;

        if (hit.collider.gameObject.GetComponent<SpriteManager>().ItemID == 12)
            hit.collider.gameObject.GetComponentInChildren<TerrainLighting>().Destruct();

        if (!GetComponent<TerrainRenderer>().RenderedChunks.ContainsKey("back x: " + (int)clickPosition.x + " back y: " + (int)clickPosition.y))
            GetComponent<TerrainRenderer>().SingleRender((int)clickPosition.x, (int)clickPosition.y, 12, hit.transform.GetComponent<SpriteManager>().ItemID);

        gameObject.GetComponent<TerrainRenderer>().RenderedChunks.Remove(hit.transform.gameObject.name);
        GetComponent<TerrainUnloader>().Unload(hit.transform.gameObject);

        StartCoroutine(SettleLiquids((int)hit.transform.position.x, (int)hit.transform.position.y));

        if (CurrentBrushMap[(int)clickPosition.x, (int)clickPosition.y + 1] != 0 && tmpNeighbor.tag != "Chest")
            DestructNeighbor(tmpNeighbor, new Vector3(clickPosition.x, clickPosition.y + 1f, clickPosition.z));
    }

    public void DestructNeighbor(GameObject neighborObj, Vector3 clickPosition)
    {
        GameObject go = (GameObject)Instantiate(dropItem, neighborObj.transform.gameObject.transform.position, Quaternion.identity);
        go.GetComponent<SpriteManager>().ItemName = neighborObj.transform.gameObject.GetComponent<SpriteManager>().ItemName;
        go.GetComponent<SpriteManager>().ItemID = neighborObj.transform.gameObject.GetComponent<SpriteManager>().ItemID;
        go.GetComponent<SpriteManager>().ListIndex = neighborObj.transform.gameObject.GetComponent<SpriteManager>().ListIndex;
        go.GetComponent<SpriteManager>().ItemType = neighborObj.transform.gameObject.GetComponent<SpriteManager>().ItemType;

        SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = neighborObj.transform.gameObject.GetComponent<SpriteRenderer>().sprite;

        go.AddComponent<BoxCollider2D>();
        go.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
        go.GetComponent<BoxCollider2D>().size = new Vector2(0.9f, 0.9f);
        float randomXDir = Random.Range(-breakXCone, breakXCone);
        go.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomXDir, breakForce));

        if (neighborObj.transform.GetComponent<SpriteManager>().LayerID == 0)
            CurrentMap[(int)clickPosition.x, (int)clickPosition.y] = 0;
        else if (neighborObj.transform.gameObject.layer == 9 || neighborObj.transform.gameObject.layer == 10 || neighborObj.transform.gameObject.layer == 18 || neighborObj.transform.gameObject.layer == 19)
            CurrentBrushMap[(int)clickPosition.x, (int)clickPosition.y] = 0;
        else
            CurrentBackMap[(int)clickPosition.x, (int)clickPosition.y] = 0;

        if (neighborObj.gameObject.GetComponent<SpriteManager>().ItemID == 12)
            neighborObj.gameObject.GetComponentInChildren<TerrainLighting>().Destruct();

        if (!GetComponent<TerrainRenderer>().RenderedChunks.ContainsKey("back x: " + (int)clickPosition.x + " back y: " + (int)clickPosition.y))
            GetComponent<TerrainRenderer>().SingleRender((int)clickPosition.x, (int)clickPosition.y, 12, neighborObj.transform.GetComponent<SpriteManager>().ItemID);

        gameObject.GetComponent<TerrainRenderer>().RenderedChunks.Remove(neighborObj.transform.gameObject.name);
        GetComponent<TerrainUnloader>().Unload(neighborObj.transform.gameObject);

        StartCoroutine(SettleLiquids((int)neighborObj.transform.position.x, (int)neighborObj.transform.position.y));
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
            }
            Destroy(loadedItem);
            Player.Instance.inventorySelect.ChangeCurrentItemText();
        }
    }
}
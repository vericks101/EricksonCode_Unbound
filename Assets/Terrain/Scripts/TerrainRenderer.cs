using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum renderType { FRONT, BACK, BRUSH }

public class TerrainRenderer : MonoBehaviour 
{
	public string tilesetName;
	public List<TileSet> tileSets;
	public Hashtable renderedChunks;
    public Hashtable toBeDeleted;

    public GameObject prefabDefault;
	public GameObject chestPrefab;
	public GameObject craftingPrefab;
    public GameObject smeltingPrefab;
    public GameObject bedPrefab;
	public GameObject lightPrefab;
    public GameObject brushLightPrefab;
	public GameObject torchPrefab;
    public GameObject lavaLightPrefab;
    public GameObject woodenDoorPrefab;
    public GameObject woodenPlatformPrefab;

	public Transform player;
	public int renderDistance = 10;
	public int renderMove = 10;
	Vector2 startPos;
	float unloadDistance;
	int xMove;
	int yMove;

    private renderType type;

    private TerrainUnloader terrainUnloader;
    [SerializeField] private TODManager todManager;

    public int chestID;
    public int torchID;
    public int craftingTableID;
    public int smeltingTableID;
    public int bedID;
    public int saplingID;
    public int woodenDoorID;
    public int woodenPlatformID;

    public float timeTillSpread = 0f;

    public int[,] TerrainMap
	{
		get
		{
			TerrainMap terrainMap = GetComponent<TerrainMap> ();

			return terrainMap.Map;
		}
	}

	public int[,] BackMap
	{
		get
		{
			TerrainMap backMap = GetComponent<TerrainMap> ();

			return backMap.BackMap;
		}
	}

	public int[,] BrushMap
	{
		get 
		{
			TerrainMap brushMap = GetComponent<TerrainMap> ();

			return brushMap.BrushMap;
		}
	}

	public Hashtable RenderedChunks
	{
		get { return renderedChunks; }
		set { renderedChunks = value; }
	}

	void Awake()
	{
        terrainUnloader = GetComponent<TerrainUnloader>();
        renderedChunks = new Hashtable();
    }

	void Start() 
	{

        LoadSprites ();

        if (player != null)
        {
            startPos.x = (int)player.position.x;
            startPos.y = (int)player.position.y;
        }
		unloadDistance = renderDistance * 1.5f;
	}

    void OnLevelWasLoaded()
    {
        TerrainLighting.lightObjects = new Dictionary<string, GameObject>();
    }

    void Update()
    {
        if (player == null)
            player = Player.Instance.transform;

        if (todManager == null)
            todManager = GameManager.Instance.GetComponent<TODManager>();

        if (Time.time > timeTillSpread)
        {
            List<GameObject> objsToDestory = new List<GameObject>();
            foreach (GameObject obj in renderedChunks.Values)
            {
                if (obj.GetComponent<SpriteManager>().ItemID == GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().saplingID)
                    GrowTree(obj, objsToDestory);

                if (obj.GetComponent<SpriteManager>().canSpread)
                    TerrainSpread(obj.GetComponent<SpriteManager>());
            }
            for (int i = 0; i < objsToDestory.Count; i++)
            {
                GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.Remove(objsToDestory[i].gameObject.name);
                try
                {
                    GetComponent<TerrainUnloader>().Unload(objsToDestory[i].transform.gameObject);
                }
                catch (System.NullReferenceException)
                { }
            }
            timeTillSpread = Time.time + GameManager.Instance.greenSpreadCD;
        }
    }

    private void TerrainSpread(SpriteManager spriteManager)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spriteManager.transform.position, .5f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].GetComponent<SpriteManager>() != null && hitColliders[i].GetComponent<SpriteManager>().ItemID == spriteManager.itemToSpreadTo
                && GameManager.Instance.terrainMap.GetComponent<TerrainEditor>().CurrentMap[(int)hitColliders[i].transform.position.x,
                    (int)hitColliders[i].transform.position.y + 1] == 0)
            {
                GameObject tmpObj = hitColliders[i].gameObject;
                timeTillSpread = Time.time + GameManager.Instance.greenSpreadCD;
                GameManager.Instance.terrainMap.GetComponent<TerrainEditor>().CurrentMap[(int)hitColliders[i].transform.position.x,
                    (int)hitColliders[i].transform.position.y] = spriteManager.ItemID;
                GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.Remove(hitColliders[i].name);
                GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().SingleRender((int)hitColliders[i].transform.position.x,
                    (int)hitColliders[i].transform.position.y, spriteManager.LayerID, spriteManager.ItemID);
                GameManager.Instance.terrainMap.GetComponent<TerrainUnloader>().Unload(tmpObj);
            }
        }

        if (spriteManager.canGrowOn && GameManager.Instance.terrainMap.GetComponent<TerrainEditor>().CurrentBrushMap[(int)transform.position.x, (int)transform.position.y + 1] == 0)
        {
            float randy = Random.Range(0f, 1f);
            if (randy < 0.75f)
            {
                int randomIndex = Random.Range(0, GameManager.Instance.terrainMap.GetComponent<LandGenerator>().brushList.Count);
                int plantID = GameManager.Instance.terrainMap.GetComponent<LandGenerator>().brushList[randomIndex].brushIndex;
                GameManager.Instance.terrainMap.GetComponent<TerrainEditor>().CurrentBrushMap[(int)transform.position.x, (int)transform.position.y + 1] = plantID;
                GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().SingleRender((int)transform.position.x, (int)transform.position.y + 1, 9, plantID);
            }
        }
    }

    private void GrowTree(GameObject go, List<GameObject> objs)
    {
        if ((int)go.transform.position.y < (int)((float)GameManager.Instance.terrainMap.GetComponent<LandGenerator>().MapHeight / 1.5f) && (int)go.transform.position.x > 2 &&
            (int)go.transform.position.x < (GameManager.Instance.terrainMap.GetComponent<LandGenerator>().MapWidth - 2))
        {
            int treeRandy = Random.Range(GameManager.Instance.terrainMap.GetComponent<LandGenerator>().minTreeHeight, GameManager.Instance.terrainMap.GetComponent<LandGenerator>().maxTreeHeight);
            if (GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentMap[(int)go.transform.position.x, (int)go.transform.position.y - 1] == GameManager.Instance.terrainMap.GetComponent<LandGenerator>().grassID &&
                GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentMap[(int)go.transform.position.x, (int)go.transform.position.y + 1] == 0
                && GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentBrushMap[(int)go.transform.position.x, (int)go.transform.position.y + 1] == 0 &&
                GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentBrushMap[(int)go.transform.position.x, (int)go.transform.position.y + treeRandy] == 0 &&
                GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentMap[(int)go.transform.position.x, (int)go.transform.position.y + treeRandy] == 0)
            {
                float randy = Random.Range(0f, 1f);
                if (randy < GameManager.Instance.terrainMap.GetComponent<LandGenerator>().treeAbundance)
                {
                    for (int i = 0; i <= treeRandy; i++)
                    {
                        GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentBrushMap[(int)go.transform.position.x, (int)go.transform.position.y + i] = GameManager.Instance.terrainMap.GetComponent<LandGenerator>().logID;
                        if (GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.ContainsKey("brush x: " + (int)go.transform.position.x + " brush y: " + (int)go.transform.position.y))
                            GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.Remove("brush x: " + (int)go.transform.position.x + " brush y: " + (int)go.transform.position.y);

                        GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().SingleRender((int)go.transform.position.x, (int)go.transform.position.y, 19, GameManager.Instance.terrainMap.GetComponent<LandGenerator>().logID);

                        float treeBranchRandy = Random.Range(0f, 1f);
                        if (treeBranchRandy < GameManager.Instance.terrainMap.GetComponent<LandGenerator>().treeBranchAbundance)
                        {
                            int treeBranchSide = Random.Range(0, 2);
                            if (treeBranchSide == 0)
                            {
                                GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentBrushMap[(int)go.transform.position.x + 1, (int)go.transform.position.y + i] = GameManager.Instance.terrainMap.GetComponent<LandGenerator>().logID;
                                if (GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.ContainsKey("brush x: " + (int)go.transform.position.x + " brush y: " + (int)go.transform.position.y))
                                    GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.Remove("brush x: " + (int)go.transform.position.x + " brush y: " + (int)go.transform.position.y);

                                GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().SingleRender((int)go.transform.position.x, (int)go.transform.position.y, 19, GameManager.Instance.terrainMap.GetComponent<LandGenerator>().logID);
                            }
                            else
                            {
                                GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentBrushMap[(int)go.transform.position.x - 1, (int)go.transform.position.y + i] = GameManager.Instance.terrainMap.GetComponent<LandGenerator>().logID;
                                if (GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.ContainsKey("brush x: " + (int)go.transform.position.x + " brush y: " + (int)go.transform.position.y))
                                    GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.Remove("brush x: " + (int)go.transform.position.x + " brush y: " + (int)go.transform.position.y);

                                GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().SingleRender((int)go.transform.position.x, (int)go.transform.position.y, 19, GameManager.Instance.terrainMap.GetComponent<LandGenerator>().logID);
                            }
                        }
                    }
                    for (int i = ((int)go.transform.position.x - 1); i <= ((int)go.transform.position.x + 1); i++)
                    {
                        for (int j = ((int)go.transform.position.y + treeRandy); j <= ((int)go.transform.position.y + treeRandy + 1); j++)
                        {
                            GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentBrushMap[i, j] = GameManager.Instance.terrainMap.GetComponent<LandGenerator>().leaveID;
                            if (GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.ContainsKey("brush x: " + (int)go.transform.position.x + " brush y: " + (int)go.transform.position.y))
                                GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.Remove("brush x: " + (int)go.transform.position.x + " brush y: " + (int)go.transform.position.y);

                            GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().SingleRender((int)go.transform.position.x, (int)go.transform.position.y, 19, GameManager.Instance.terrainMap.GetComponent<LandGenerator>().leaveID);
                        }
                    }
                    objs.Add(go);

                    RenderedChunks.Remove(go.name);
                    GetComponent<TerrainUnloader>().Unload(go);
                    GameManager.Instance.terrainMap.GetComponent<LandGenerator>().CurrentBrushMap[(int)go.transform.position.x, (int)go.transform.position.y] = GameManager.Instance.terrainMap.GetComponent<LandGenerator>().logID;
                    GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().SingleRender((int)go.transform.position.x, (int)go.transform.position.y, 19, GameManager.Instance.terrainMap.GetComponent<LandGenerator>().logID);

                    Render();
                }
            }
        }
    }

    void FixedUpdate()
	{
        if (player != null)
        {
            xMove = (int)(player.position.x - startPos.x);
            yMove = (int)(player.position.y - startPos.y);
            if (Mathf.Abs(xMove) >= renderMove || Mathf.Abs(yMove) >= renderMove)
            {
                Render();
                startPos.x = player.position.x;
                startPos.y = player.position.y;
            }
        }
	}

    public void LoadSprites()
	{
		foreach (var t in tileSets)
		{
			Sprite[] sprites = Resources.LoadAll<Sprite>(t.Name);
			for (int i = 0; i < sprites.Length; i++) 
			{
				if (i >= t.tileData.Count)
				{
					TileData td = new TileData () {sprite = sprites[i]};
					t.tileData.Add(td);
				}
				else
				{
					t.tileData[i].sprite = sprites[i];
					t.tileData [i].itemID = i;
				}
			}
		}
	}

	public void Render()
	{
		if (TerrainMap == null)
            return;

		if (tileSets == null || tileSets.Count == 0)
            return;

        toBeDeleted = new Hashtable();
		foreach (GameObject go in renderedChunks.Values)
		{
			float dist = 0f;
			if (go != null)
                dist = Vector3.Distance (go.transform.position, player.transform.position);
			if (dist > unloadDistance && go.GetComponent<SpriteManager>().ItemID != chestID && !toBeDeleted.ContainsKey(go.name))
                toBeDeleted.Add (go.name, go);
		}

		foreach (GameObject go in toBeDeleted.Values)
		{
			//renderedChunks.Remove (go.name);
            terrainUnloader.Unload(go);
		}
			
		TileSet currenTileSet = tileSets.Find (t => t.Name == tilesetName);
		if (currenTileSet == null)
            currenTileSet = tileSets [0];

		for (int y = (int)player.position.y - (renderDistance / 2); y < (int)player.position.y + (renderDistance / 2); y++)
		{
			for (int x = (int)player.position.x - renderDistance; x < (int)player.position.x + renderDistance; x++)
			{
				if ((int)player.position.y - renderDistance >= 0 && (int)player.position.y + renderDistance <= TerrainMap.GetLength (1) 
					&& !renderedChunks.ContainsKey ("x: " + x + " y: " + y) && !renderedChunks.ContainsKey ("back x: " + x + " back y: " + y) 
					&& !renderedChunks.ContainsKey("brush x: " + x + " brush y: " + y))
				{
					if ((int)player.position.x - renderDistance >= 0 && (int)player.position.x + renderDistance <= TerrainMap.GetLength (0)) 
					{
						TileData tiledata = currenTileSet.tileData [TerrainMap [x, y]];
						if (!tiledata.blank && (tiledata.layerId == 0 || tiledata.layerId == 10)) 
						{
							GameObject go;
                            float xCorrection;
                            float yCorrection;

                            if (TerrainMap[x, y] == woodenPlatformID)
                            {
                                go = GetNextAvailable(woodenPlatformPrefab);
                                xCorrection = x + 0.5f;
                                go.transform.position = new Vector3(xCorrection, y);
                            }
                            else if (TerrainMap[x, y] == woodenDoorID)
                            {
                                go = GetNextAvailable(woodenDoorPrefab);
                                yCorrection = y + 1f;
                                go.transform.position = new Vector3(x, yCorrection);
                            }
                            else
                            {
                                go = GetNextAvailable(prefabDefault);
                                go.transform.position = new Vector3(x, y);
                            }

                            if (!(TerrainMap[x, y] == woodenDoorID))
                            {
                                go.GetComponent<BoxCollider2D>().enabled = tiledata.boxCollider;
                                go.GetComponent<BoxCollider2D>().isTrigger = tiledata.isTrigger;
                            }
							go.transform.parent = this.transform;

							SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer> ();
							spriteRenderer.sprite = tiledata.sprite;
							spriteRenderer.color = Color.black;

                            go.GetComponent<SpriteManager>().ItemName = tiledata.name;
							go.GetComponent<SpriteManager> ().Health = tiledata.health;
							go.GetComponent<SpriteManager> ().ItemID = tiledata.itemID;
                            go.GetComponent<SpriteManager>().ItemType = tiledata.itemType;
                            if (go.tag != "Platform")
                                go.GetComponent<SpriteManager>().LayerID = tiledata.layerId;
                            go.GetComponent<SpriteManager>().ListIndex = tiledata.listIndex;
                            go.GetComponent<SpriteManager>().CanDamage = tiledata.canDamage;
                            go.GetComponent<SpriteManager>().GravityChanger = tiledata.gravityChanger;
                            go.GetComponent<SpriteManager>().CanSpread = tiledata.canSpread;
                            go.GetComponent<SpriteManager>().ItemToSpreadTo = tiledata.itemToSpreadTo;
                            go.GetComponent<SpriteManager>().CanGrowOn = tiledata.canGrowOn;
							go.name = "x: " + x + " y: " + y;

							renderedChunks.Add (go.name, go);

							if (tiledata.layerId == 10) 
							{
                                if (go.GetComponent<SpriteManager>().ItemID == 68)
                                    go.layer = 12;
                                else
                                {
                                    go.layer = 10;
                                    go.GetComponent<SpriteRenderer>().sortingLayerName = "noCollideEnvFront";
                                }

								Color tmp = go.GetComponent<SpriteRenderer> ().color;
								tmp.a = 0.5f;
								go.GetComponent<SpriteRenderer> ().color = tmp;

                                if (go.GetComponent<SpriteManager>().ItemID == 24)
                                {
                                    GameObject lightObj = (GameObject)Instantiate(lavaLightPrefab, go.transform.position, Quaternion.identity);
                                    lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y,
                                        lightObj.transform.position.z);
                                    lightObj.transform.parent = go.transform;

                                    if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                        TerrainLighting.lightObjects.Add(go.name, lightObj);
                                }
							}
                            else if (tiledata.layerId == 9)
                            {
                                go.layer = 9;
                            }
                            else if (tiledata.layerId == 18)
                            {
                                go.layer = 18;
                            }
                            else if (tiledata.layerId == 19)
                            {
                                go.layer = 19;
                            }
                            else if (go.tag != "Platform")
                            {
								go.layer = 0;
							}

							if (go.GetComponent<SpriteManager> ().ItemID == torchID)
                                spriteRenderer.color = new Color (1f, 1f, 1f);

							if ((TerrainMap [x, y + 1] == 0 && BackMap [x, y + 1] == 0) || (TerrainMap [x, y - 1] == 0 && BackMap [x, y - 1] == 0) 
								|| (TerrainMap [x + 1, y] == 0 && BackMap [x + 1, y] == 0) || (TerrainMap [x - 1, y] == 0 && BackMap [x - 1, y] == 0)) 
							{
                                if (tiledata.layerId == 9 || tiledata.layerId == 18 || tiledata.layerId == 19)
                                {
                                    GameObject lightObj = (GameObject)Instantiate(lightPrefab, go.transform.position, Quaternion.identity);
                                    lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y,
                                        lightObj.transform.position.z);
                                    lightObj.transform.parent = go.transform;

                                    if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                        TerrainLighting.lightObjects.Add(go.name, lightObj);
                                }
                                else
                                {
                                    GameObject lightObj = (GameObject)Instantiate(lightPrefab, go.transform.position, Quaternion.identity);
                                    lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y + 1.5f,
                                        lightObj.transform.position.z);
                                    lightObj.transform.parent = go.transform;

                                    if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                        TerrainLighting.lightObjects.Add(go.name, lightObj);
                                }
                            }
						}

                        tiledata = currenTileSet.tileData[BrushMap[x, y]];
                        if (!tiledata.blank)
                        {
                            GameObject go;
                            float xCorrection;
                            //float yCorrection;

                            if (BrushMap[x, y] == craftingTableID)
                            {
                                go = GetNextAvailable(craftingPrefab);
                                xCorrection = x + 0.5f;
                                go.transform.position = new Vector3(xCorrection, y);
                            }
                            else if (BrushMap[x, y] == chestID)
                            {
                                go = GetNextAvailable(chestPrefab);
                                xCorrection = x + 0.5f;
                                //yCorrection = y + 0.5f;
                                go.transform.position = new Vector3(xCorrection, y, 0f);
                            }
                            else if (BrushMap[x, y] == smeltingTableID)
                            {
                                go = GetNextAvailable(smeltingPrefab);
                                xCorrection = x + 0.5f;
                                go.transform.position = new Vector3(xCorrection, y);
                            }
                            else if (BrushMap[x, y] == bedID)
                            {
                                go = GetNextAvailable(bedPrefab);
                                xCorrection = x + 0.5f;
                                go.transform.position = new Vector3(xCorrection, y);
                            }
                            else
                            {
                                go = GetNextAvailable(prefabDefault);
                                go.transform.position = new Vector3(x, y);
                            }

                            go.GetComponent<BoxCollider2D>().enabled = tiledata.boxCollider;
                            go.GetComponent<BoxCollider2D>().isTrigger = tiledata.isTrigger;
                            go.transform.parent = this.transform;

                            SpriteRenderer SpriteRenderer = go.GetComponent<SpriteRenderer>();
                            SpriteRenderer.sprite = tiledata.sprite;
                            SpriteRenderer.color = Color.black;/*new Color(todManager.currentBrushTint, todManager.currentBrushTint, todManager.currentBrushTint);*/

                            SpriteRenderer.sortingOrder = 0;

                            go.GetComponent<SpriteManager>().Health = tiledata.health;
                            go.GetComponent<SpriteManager>().ItemName = tiledata.name;
                            go.GetComponent<SpriteManager>().ItemID = tiledata.itemID;
                            go.GetComponent<SpriteManager>().ItemType = tiledata.itemType;
                            go.GetComponent<SpriteManager>().LayerID = tiledata.layerId;
                            go.GetComponent<SpriteManager>().ListIndex = tiledata.listIndex;
                            go.GetComponent<SpriteManager>().CanDamage = tiledata.canDamage;
                            go.GetComponent<SpriteManager>().GravityChanger = tiledata.gravityChanger;
                            go.GetComponent<SpriteManager>().CanSpread = tiledata.canSpread;
                            go.GetComponent<SpriteManager>().ItemToSpreadTo = tiledata.itemToSpreadTo;
                            go.GetComponent<SpriteManager>().CanGrowOn = tiledata.canGrowOn;
                            go.name = "brush x: " + x + " brush y: " + y;

                            renderedChunks.Add(go.name, go);

                            if (tiledata.layerId == 18)
                                go.layer = 18;
                            else if (tiledata.layerId == 19)
                                go.layer = 19;
                            else
                                go.layer = 9;

                            if (go.GetComponent<SpriteManager>().ItemID == torchID)
                            {
                                GameObject torchObj = (GameObject)Instantiate(torchPrefab, go.transform.position, Quaternion.identity);
                                torchObj.transform.parent = go.transform;
                                go.GetComponent<SpriteRenderer>().color = Color.white;

                                if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                    TerrainLighting.lightObjects.Add(go.name, torchObj);
                            }

                            if ((TerrainMap[x, y + 1] == 0 && BackMap[x, y + 1] == 0) || (TerrainMap[x, y - 1] == 0 && BackMap[x, y - 1] == 0)
                                || (TerrainMap[x + 1, y] == 0 && BackMap[x + 1, y] == 0) || (TerrainMap[x - 1, y] == 0 && BackMap[x - 1, y] == 0))
                            {
                                if (tiledata.layerId == 9 || tiledata.layerId == 18 || tiledata.layerId == 19)
                                {
                                    GameObject lightObj = (GameObject)Instantiate(lightPrefab, go.transform.position, Quaternion.identity);
                                    lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y,
                                        lightObj.transform.position.z);
                                    lightObj.transform.parent = go.transform;

                                    if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                        TerrainLighting.lightObjects.Add(go.name, lightObj);
                                }
                                else
                                {
                                    GameObject lightObj = (GameObject)Instantiate(lightPrefab, go.transform.position, Quaternion.identity);
                                    lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y + 1.5f,
                                        lightObj.transform.position.z);
                                    lightObj.transform.parent = go.transform;

                                    if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                        TerrainLighting.lightObjects.Add(go.name, lightObj);
                                }
                            }
                        }

                        tiledata = currenTileSet.tileData[BackMap[x, y]];
                        if (!tiledata.blank && (TerrainMap[x, y] == 0 || TerrainMap[x, y] == 7 || TerrainMap[x, y] == 24 || TerrainMap[x, y] == 69 || TerrainMap[x, y] == 70))
                        {
                            GameObject backGo;
                            float xCorrection;
                            //float yCorrection;

                            if (BackMap[x, y] == craftingTableID)
                            {
                                backGo = GetNextAvailable(craftingPrefab);
                                xCorrection = x + 0.5f;
                                backGo.transform.position = new Vector3(xCorrection, y);

                                backGo.layer = 0;
                            }
                            else if (BackMap[x, y] == chestID)
                            {
                                backGo = GetNextAvailable(chestPrefab);
                                xCorrection = x + 0.5f;
                                //yCorrection = y + 0.5f;
                                backGo.transform.position = new Vector3(xCorrection, y);

                                backGo.layer = 0;
                            }
                            else if (BackMap[x, y] == smeltingTableID)
                            {
                                backGo = GetNextAvailable(smeltingPrefab);
                                xCorrection = x + 0.5f;
                                backGo.transform.position = new Vector3(xCorrection, y);

                                backGo.layer = 0;
                            }
                            else if (BackMap[x, y] == bedID)
                            {
                                backGo = GetNextAvailable(bedPrefab);
                                xCorrection = x + 0.5f;
                                backGo.transform.position = new Vector3(xCorrection, y);
                            }
                            else
                            {
                                backGo = GetNextAvailable(prefabDefault);
                                backGo.transform.position = new Vector3(x, y);

                                backGo.layer = 12;
                            }

                            backGo.GetComponent<BoxCollider2D>().enabled = tiledata.boxCollider;
                            backGo.GetComponent<BoxCollider2D>().isTrigger = tiledata.isTrigger;
                            backGo.transform.parent = this.transform;

                            SpriteRenderer backSpriteRenderer = backGo.GetComponent<SpriteRenderer>();
                            backSpriteRenderer.sprite = tiledata.sprite;
                            backSpriteRenderer.color = Color.black;
                            backSpriteRenderer.sortingOrder = -1;

                            backGo.GetComponent<SpriteManager>().Health = tiledata.health;
                            backGo.GetComponent<SpriteManager>().ItemName = tiledata.name;
                            backGo.GetComponent<SpriteManager>().ItemID = tiledata.itemID;
                            backGo.GetComponent<SpriteManager>().ItemType = tiledata.itemType;
                            backGo.GetComponent<SpriteManager>().LayerID = tiledata.layerId;
                            backGo.GetComponent<SpriteManager>().ListIndex = tiledata.listIndex;
                            backGo.GetComponent<SpriteManager>().CanDamage = tiledata.canDamage;
                            backGo.GetComponent<SpriteManager>().GravityChanger = tiledata.gravityChanger;
                            backGo.GetComponent<SpriteManager>().CanSpread = tiledata.canSpread;
                            backGo.GetComponent<SpriteManager>().ItemToSpreadTo = tiledata.itemToSpreadTo;
                            backGo.GetComponent<SpriteManager>().CanGrowOn = tiledata.canGrowOn;
                            backGo.name = "back x: " + x + " back y: " + y;

                            renderedChunks.Add(backGo.name, backGo);

                            if (backGo.GetComponent<SpriteManager>().ItemID == torchID)
                                backSpriteRenderer.color = new Color(1f, 1f, 1f);

                            if ((TerrainMap[x, y + 1] == 0 && BackMap[x, y + 1] == 0) || (TerrainMap[x, y - 1] == 0 && BackMap[x, y - 1] == 0)
                                || (TerrainMap[x + 1, y] == 0 && BackMap[x + 1, y] == 0) || (TerrainMap[x - 1, y] == 0 && BackMap[x - 1, y] == 0))
                            {
                                if (tiledata.layerId == 9 || tiledata.layerId == 18 || tiledata.layerId == 19)
                                {
                                    GameObject lightObj = (GameObject)Instantiate(lightPrefab, backGo.transform.position, Quaternion.identity);
                                    lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y,
                                        lightObj.transform.position.z);
                                    lightObj.transform.parent = backGo.transform;

                                    if (!TerrainLighting.lightObjects.ContainsKey(backGo.name))
                                        TerrainLighting.lightObjects.Add(backGo.name, lightObj);
                                }
                                else
                                {
                                    GameObject lightObj = (GameObject)Instantiate(lightPrefab, backGo.transform.position, Quaternion.identity);
                                    lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y + 1.5f,
                                        lightObj.transform.position.z);
                                    lightObj.transform.parent = backGo.transform;

                                    if (!TerrainLighting.lightObjects.ContainsKey(backGo.name))
                                        TerrainLighting.lightObjects.Add(backGo.name, lightObj);
                                }
                            }
                        }
					}
				}
			}
		}

        //foreach (GameObject go in renderedChunks.Values)
        //{
        //    go.GetComponent<SpriteRenderer>().color = Color.white;
        //}
    }

	public void SingleRender(int x, int y, int layerId, int id)
	{
		if (TerrainMap == null) 
			return;

		if (tileSets == null || tileSets.Count == 0) 
			return;

		TileSet currenTileSet = tileSets.Find(t => t.Name == tilesetName);
		if (currenTileSet == null) 
		{
			currenTileSet = tileSets [0];
		}

        TileData tiledata;
        if (layerId == 0 || layerId == 10)
            tiledata = currenTileSet.tileData[TerrainMap[x, y]];
        else if (layerId == 9 || layerId == 18 || layerId == 19)
        {
            tiledata = currenTileSet.tileData[BrushMap[x, y]];
        }
        else
            tiledata = currenTileSet.tileData[BackMap[x, y]];

        if (tiledata.blank) 
		{ }
		else if (layerId == 0 || layerId == 9 || layerId == 10 || layerId == 18 || layerId == 19)
		{
            if (!renderedChunks.ContainsKey ("x: " + x + " y: " + y) && !renderedChunks.ContainsKey("brush x: " + x + " brush y: " + y)) 
			{
				GameObject go;
				float xCorrection;
				float yCorrection;

                if (TerrainMap[x, y] == woodenPlatformID)
                {
                    go = GetNextAvailable(woodenPlatformPrefab);
                    xCorrection = x + 0.5f;
                    go.transform.position = new Vector3(xCorrection, y);
                }
                else if (TerrainMap[x, y] == woodenDoorID)
                {
                    go = GetNextAvailable(woodenDoorPrefab);
                    yCorrection = y + 1f;
                    go.transform.position = new Vector3(x, yCorrection);
                }
                else if (BrushMap [x, y] == craftingTableID)
				{
					go = GetNextAvailable (craftingPrefab);
					xCorrection = x + 0.5f;
					go.transform.position = new Vector3 (xCorrection, y);
                }
				else if (BrushMap[x, y] == chestID) 
				{
					go = GetNextAvailable (chestPrefab);
					xCorrection = x + 0.5f;
					//yCorrection = y + 0.5f;
					go.transform.position = new Vector3 (xCorrection, y);
                }
                else if (BrushMap[x, y] == smeltingTableID)
                {
                    go = GetNextAvailable(smeltingPrefab);
                    xCorrection = x + 0.5f;
                    go.transform.position = new Vector3(xCorrection, y);
                }
                else if (BrushMap[x, y] == bedID)
                {
                    go = GetNextAvailable(bedPrefab);
                    xCorrection = x + 0.5f;
                    go.transform.position = new Vector3(xCorrection, y);
                }
                else 
				{
					go = GetNextAvailable (prefabDefault);
                    go.transform.position = new Vector3(x, y);
                }
				go.transform.parent = this.transform;

				SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer> ();
				spriteRenderer.sprite = tiledata.sprite;
                spriteRenderer.color = Color.black;

                if (!(TerrainMap[x, y] == woodenDoorID))
                {
                    go.GetComponent<BoxCollider2D>().enabled = tiledata.boxCollider;
                    go.GetComponent<BoxCollider2D>().isTrigger = tiledata.isTrigger;
                }

                go.GetComponent<SpriteManager> ().Health = tiledata.health;
                go.GetComponent<SpriteManager>().ItemName = tiledata.name;
                go.GetComponent<SpriteManager> ().ItemID = tiledata.itemID;
                go.GetComponent<SpriteManager>().ItemType = tiledata.itemType;
                if (go.tag != "Platform")
                    go.GetComponent<SpriteManager>().LayerID = tiledata.layerId;
                go.GetComponent<SpriteManager>().ListIndex = tiledata.listIndex;
                go.GetComponent<SpriteManager>().CanDamage = tiledata.canDamage;
                go.GetComponent<SpriteManager>().GravityChanger = tiledata.gravityChanger;
                go.GetComponent<SpriteManager>().CanSpread = tiledata.canSpread;
                go.GetComponent<SpriteManager>().ItemToSpreadTo = tiledata.itemToSpreadTo;
                go.GetComponent<SpriteManager>().CanGrowOn = tiledata.canGrowOn;
                go.name = "x: " + x + " y: " + y;

				if (tiledata.sprite.name == "forest_tileset_6" || tiledata.sprite.name == "forest_tileset_8" || tiledata.layerId == 9) 
				{
					go.layer = 9;
				} 
				else if (tiledata.layerId == 10) 
				{
                    if (go.GetComponent<SpriteManager>().ItemID == 68)
                        go.layer = 12;
                    else
                    {
                        go.layer = 10;
                        go.GetComponent<SpriteRenderer>().sortingLayerName = "noCollideEnvFront";
                    }
                    

					Color color = go.GetComponent<SpriteRenderer> ().color;
					color.a = 0.5f;
					go.GetComponent<SpriteRenderer> ().color = color;
				} 
                else if (tiledata.layerId == 18)
                {
                    go.layer = 18;
                }
                else if (tiledata.layerId == 19)
                {
                    go.layer = 19;
                }
                else if (go.tag != "Platform")
				{
					go.layer = 0;
				}

				if (go.GetComponent<SpriteManager> ().ItemID == torchID) 
				{
					GameObject torchObj = (GameObject)Instantiate (torchPrefab, go.transform.position, Quaternion.identity);
					torchObj.transform.parent = go.transform;
					go.name = "brush x: " + x + " brush y: " + y;

                    spriteRenderer.color = Color.white;

                    if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                        TerrainLighting.lightObjects.Add(go.name, torchObj);
                }

                renderedChunks.Add(go.name, go);

                if (x > 1 && x < GameManager.Instance.terrainMap.GetComponent<LandGenerator>().MapWidth && y > 1 && y < GameManager.Instance.terrainMap.GetComponent<LandGenerator>().MapHeight)
                {
                    if ((TerrainMap[x, y + 1] == 0 && BackMap[x, y + 1] == 0) || (TerrainMap[x, y - 1] == 0 && BackMap[x, y - 1] == 0)
                        || (TerrainMap[x + 1, y] == 0 && BackMap[x + 1, y] == 0) || (TerrainMap[x - 1, y] == 0 && BackMap[x - 1, y] == 0))
                    {
                        if (tiledata.layerId == 9 || tiledata.layerId == 18 || tiledata.layerId == 19)
                        {
                            GameObject lightObj = (GameObject)Instantiate(lightPrefab, go.transform.position, Quaternion.identity);
                            lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y,
                                lightObj.transform.position.z);
                            lightObj.transform.parent = go.transform;

                            if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                TerrainLighting.lightObjects.Add(go.name, lightObj);
                        }
                        else
                        {
                            GameObject lightObj = (GameObject)Instantiate(lightPrefab, go.transform.position, Quaternion.identity);
                            lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y + 1.5f,
                                lightObj.transform.position.z);
                            lightObj.transform.parent = go.transform;

                            if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                TerrainLighting.lightObjects.Add(go.name, lightObj);
                        }
                    }
                }
            }
		}

        if (tiledata.blank)
        { }
        else if (layerId == 12 && TerrainMap[x, y] == 0)
        {
            GameObject backGo;
            float xCorrection;
            //float yCorrection;

            if (BackMap[x, y] == craftingTableID)
            {
                backGo = GetNextAvailable(craftingPrefab);
                xCorrection = x + 0.5f;
                backGo.transform.position = new Vector3(xCorrection, y);

                backGo.layer = 0;
            }
            else if (BackMap[x, y] == chestID)
            {
                backGo = GetNextAvailable(chestPrefab);
                xCorrection = x + 0.5f;
                //yCorrection = y + 0.5f;
                backGo.transform.position = new Vector3(xCorrection, y);

                backGo.layer = 0;
            }
            else if (BrushMap[x, y] == smeltingTableID)
            {
                backGo = GetNextAvailable(smeltingPrefab);
                xCorrection = x + 0.5f;
                backGo.transform.position = new Vector3(xCorrection, y);
            }
            else if (BrushMap[x, y] == bedID)
            {
                backGo = GetNextAvailable(bedPrefab);
                xCorrection = x + 0.5f;
                backGo.transform.position = new Vector3(xCorrection, y);
            }
            else
            {
                backGo = GetNextAvailable(prefabDefault);
                backGo.transform.position = new Vector3(x, y);

                backGo.layer = 12;
            }

            backGo.GetComponent<BoxCollider2D>().enabled = tiledata.boxCollider;
            backGo.GetComponent<BoxCollider2D>().isTrigger = tiledata.isTrigger;
            backGo.transform.parent = this.transform;

            SpriteRenderer backSpriteRenderer = backGo.GetComponent<SpriteRenderer>();
            backSpriteRenderer.sprite = tiledata.sprite;
            backSpriteRenderer.color = Color.black;
            backSpriteRenderer.sortingOrder = -1;

            backGo.GetComponent<SpriteManager>().Health = tiledata.health;
            backGo.GetComponent<SpriteManager>().ItemName = tiledata.name;
            backGo.GetComponent<SpriteManager>().ItemID = tiledata.itemID;
            backGo.GetComponent<SpriteManager>().ItemType = tiledata.itemType;
            backGo.GetComponent<SpriteManager>().LayerID = tiledata.layerId;
            backGo.GetComponent<SpriteManager>().ListIndex = tiledata.listIndex;
            backGo.GetComponent<SpriteManager>().CanDamage = tiledata.canDamage;
            backGo.GetComponent<SpriteManager>().GravityChanger = tiledata.gravityChanger;
            backGo.GetComponent<SpriteManager>().CanSpread = tiledata.canSpread;
            backGo.GetComponent<SpriteManager>().ItemToSpreadTo = tiledata.itemToSpreadTo;
            backGo.GetComponent<SpriteManager>().CanGrowOn = tiledata.canGrowOn;
            backGo.name = "back x: " + x + " back y: " + y;

            renderedChunks.Add(backGo.name, backGo);

            if (backGo.GetComponent<SpriteManager>().ItemID == torchID)
                backSpriteRenderer.color = new Color(1f, 1f, 1f);

            if ((TerrainMap[x, y + 1] == 0 && BackMap[x, y + 1] == 0) || (TerrainMap[x, y - 1] == 0 && BackMap[x, y - 1] == 0)
                || (TerrainMap[x + 1, y] == 0 && BackMap[x + 1, y] == 0) || (TerrainMap[x - 1, y] == 0 && BackMap[x - 1, y] == 0))
            {
                if (tiledata.layerId == 9 || tiledata.layerId == 18 || tiledata.layerId == 19)
                {
                    GameObject lightObj = (GameObject)Instantiate(lightPrefab, backGo.transform.position, Quaternion.identity);
                    lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y,
                        lightObj.transform.position.z);
                    lightObj.transform.parent = backGo.transform;

                    if (!TerrainLighting.lightObjects.ContainsKey(backGo.name))
                        TerrainLighting.lightObjects.Add(backGo.name, lightObj);
                }
                else
                {
                    GameObject lightObj = (GameObject)Instantiate(lightPrefab, backGo.transform.position, Quaternion.identity);
                    lightObj.transform.position = new Vector3(lightObj.transform.position.x, lightObj.transform.position.y + 1.5f,
                        lightObj.transform.position.z);
                    lightObj.transform.parent = backGo.transform;

                    if (!TerrainLighting.lightObjects.ContainsKey(backGo.name))
                        TerrainLighting.lightObjects.Add(backGo.name, lightObj);
                }
            }
        }
    }



	GameObject GetNextAvailable(GameObject go)
	{
		if (go == null)
            go = Resources.Load<GameObject> (go.name);
        if (go != prefabDefault)
            return Instantiate(go);

		return ObjectPooler.current.GetPooledObject();
	}

	public void ClearImmediate()
	{
		int retrycount = 0;

		while (this.transform.childCount > 0 && retrycount < 20)
		{
			foreach (Transform child in this.transform)
			{
				DestroyImmediate (child.gameObject);
			}
			retrycount++;
		}
	}
}
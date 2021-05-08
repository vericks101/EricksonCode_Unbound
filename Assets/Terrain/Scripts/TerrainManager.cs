using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets._2D;

public class TerrainManager : BaseGenerator 
{
	public Transform player;

	public Transform terrain;
	private LandGenerator hillGenerator = null;

    public string worldName;
    public int worldID;

    public bool spawnSet = false;

    public bool enableBorders = true;

    private static TerrainManager instance;
    public static TerrainManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<TerrainManager>();

            return instance;
        }
    }

    void Awake()
	{
		hillGenerator = terrain.GetComponent<LandGenerator> ();
	}

	void Start()
	{
        CodecManager.Instance.terrainLoaded = CodecManager.Instance.LoadTerrain();
        if (!CodecManager.Instance.terrainLoaded)
		    GenerateHillsAndCavesWithRandomContour ();
    }

    void Update()
    {
        if (player == null)
        {
            player = Player.Instance.transform;
            if (!spawnSet)
                SetPlayerSpawn();
        }
    }

	public void GenerateHillsAndCavesWithRandomContour()
	{
		CaveGenerator caveGenerator = terrain.GetComponent<CaveGenerator> ();
		caveGenerator.renderImmediate = false;
		caveGenerator.Generate ();

		hillGenerator.yNoise = UnityEngine.Random.Range (0f, 1f);
		hillGenerator.xScale = UnityEngine.Random.Range (6f, 16f);
		hillGenerator.renderImmediate = false;
		hillGenerator.contourCurve = new AnimationCurve(new Keyframe(0, UnityEngine.Random.Range(0f, 1f)), new Keyframe(0.1f, UnityEngine.Random.Range(0f, 1f)),
            new Keyframe(0.2f, UnityEngine.Random.Range(0f, 1f)), new Keyframe(0.3f, UnityEngine.Random.Range(0f, 1f)), new Keyframe(0.4f, UnityEngine.Random.Range(0f, 1f))
            , new Keyframe(0.5f, UnityEngine.Random.Range(0f, 1f)), new Keyframe(0.6f, UnityEngine.Random.Range(0f, 1f)), new Keyframe(0.7f, UnityEngine.Random.Range(0f, 1f))
            , new Keyframe(0.8f, UnityEngine.Random.Range(0f, 1f)), new Keyframe(0.9f, UnityEngine.Random.Range(0f, 1f)), new Keyframe(1, UnityEngine.Random.Range(0f, 1f)));		
		hillGenerator.Append ();

		OreGenerator oreGenerator = terrain.GetComponent<OreGenerator> ();
		oreGenerator.Append ();

        LabGenerator labGenerator = terrain.GetComponent<LabGenerator>();
        if (labGenerator != null)
            labGenerator.GenerateStructure();
        DesertTempleGenerator desertTempleGenerator = terrain.GetComponent<DesertTempleGenerator>();
        if (desertTempleGenerator != null)
            desertTempleGenerator.GenerateStructure();
        MineshaftGenerator mineshaftGenerator = terrain.GetComponent<MineshaftGenerator>();
        if (mineshaftGenerator != null)
            mineshaftGenerator.GenerateStructure();
        LargeLabGenerator largeLabGenerator = terrain.GetComponent<LargeLabGenerator>();
        if (largeLabGenerator != null)
            largeLabGenerator.GenerateStructure();
        HellBuildingGenerator hellBuildingGenerator = terrain.GetComponent<HellBuildingGenerator>();
        if (hellBuildingGenerator != null)
            hellBuildingGenerator.GenerateStructure();
    }

	void SetPlayerSpawn()
	{
		SpawnpointManager.Instance.transform.position = new Vector3 (hillGenerator.MapWidth / 2, hillGenerator.yOffset, player.position.z) + terrain.position;
		while (CurrentBackMap [(int)SpawnpointManager.Instance.transform.position.x, (int)SpawnpointManager.Instance.transform.position.y] != 0)
		{
            SpawnpointManager.Instance.transform.position = new Vector3 (SpawnpointManager.Instance.transform.position.x, SpawnpointManager.Instance.transform.position.y + 1, SpawnpointManager.Instance.transform.position.z);
		}

        float tempDamping = Camera.main.GetComponentInParent<Camera2DFollow>().damping;
        Camera.main.GetComponentInParent<Camera2DFollow>().damping = 0f;

        player.position = SpawnpointManager.Instance.transform.position;
        Player.Instance.GetComponent<FallDamageManager>().enabled = true;

        StartCoroutine("WaitToDamp");

        CodecManager.Instance.SaveTerrain();
    }

    IEnumerator WaitToDamp()
    {
        yield return new WaitForSeconds(1f);
        if (Camera.main != null)
            Camera.main.GetComponentInParent<Camera2DFollow>().damping = 0.3f;
    }
}
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EnemyDetail
{
	public string name;
	public GameObject gm;
	public float spawnChance;
    public bool nightMob;
}

public class SpawningManager : BaseGenerator
{
	public List<EnemyDetail> enemies;
    public List<GameObject> currentEnemies;
    private bool killingMobs = false;
	[HideInInspector] public int entityCount = 0;
	[SerializeField] private int entityCap;
	private Vector3 spawnPoint;
	[SerializeField] private float spawnDist;
    [SerializeField] private float offsetFromPlayer;
    [SerializeField] private float spawnInterval;

    public float spawnRate;

    private static SpawningManager instance;

	public static SpawningManager Instance
	{
		get
		{
			if (instance == null) instance = GameObject.FindObjectOfType<SpawningManager> ();

			return SpawningManager.instance; 
		}
	}

	private void Update()
	{
		if (!killingMobs && entityCount != entityCap && CurrentSceneManager.Instance != null && CurrentSceneManager.Instance.mobSpawning && SettingsManager.Instance.enemySpawningToggle.isOn)
            InvokeRepeating ("SpawnEnemy", 1f, spawnInterval);
	}

    public void KillMobs()
    {
        GameObject[] mobsToDestroy = new GameObject[currentEnemies.Count];
        killingMobs = true;
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            mobsToDestroy[i] = currentEnemies[i];
        }
        for (int i = 0; i < mobsToDestroy.Length; i++)
        {
            try
            {
                TerrainLighting.lightObjects.Remove(mobsToDestroy[i].name);
            }
            catch(MissingReferenceException)
            { }
            catch(NullReferenceException)
            { }
            currentEnemies.Remove(mobsToDestroy[i]);
            Destroy(mobsToDestroy[i]);
        }
        killingMobs = false;
    }

	public void SpawnEnemy()
	{
		if (!Player.Instance.isDead && SettingsManager.Instance.enemySpawningToggle.isOn) 
		{
			var enemyToSpawn = enemies [UnityEngine.Random.Range (0, enemies.Count)];
            if (spawnRate == 0f)
            {
                if (UnityEngine.Random.Range(0f, 1f) < enemyToSpawn.spawnChance)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                        spawnPoint.x = UnityEngine.Random.Range(Player.Instance.transform.position.x - spawnDist, Player.Instance.transform.position.x - offsetFromPlayer);
                    else
                        spawnPoint.x = UnityEngine.Random.Range(Player.Instance.transform.position.x + offsetFromPlayer, Player.Instance.transform.position.x + spawnDist);
                    spawnPoint.y = UnityEngine.Random.Range(Player.Instance.transform.position.y - spawnDist, Player.Instance.transform.position.y + spawnDist);

                    try
                    {
                        if (Vector2.Distance(spawnPoint, Player.Instance.transform.position) > (spawnDist / 2) && CurrentMap[(int)spawnPoint.x, (int)spawnPoint.y] == 0)
                        {
                            bool rightTime = true;
                            if (enemyToSpawn.nightMob)
                                if (GameManager.Instance.GetComponent<TODManager>().isDay)
                                    rightTime = false;
                            if (rightTime)
                            {
                                GameObject go = (GameObject)Instantiate(enemyToSpawn.gm, spawnPoint, Quaternion.identity);
                                entityCount++;
                                currentEnemies.Add(go);
                                if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                    TerrainLighting.lightObjects.Add(go.name, go);
                            }
                        }
                    }
                    catch (NullReferenceException)
                    {
                        bool rightTime = true;
                        if (enemyToSpawn.nightMob)
                            if (GameManager.Instance.GetComponent<TODManager>().isDay)
                                rightTime = false;
                        if (rightTime)
                        {
                            GameObject go = (GameObject)Instantiate(enemyToSpawn.gm, spawnPoint, Quaternion.identity);
                            entityCount++;
                            currentEnemies.Add(go);
                            if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                TerrainLighting.lightObjects.Add(go.name, go);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    { }
                }
            }
            else if (SettingsManager.Instance.enemySpawningToggle.isOn)
            {
                if (UnityEngine.Random.Range(0f, 1f) < spawnRate)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                        spawnPoint.x = UnityEngine.Random.Range(Player.Instance.transform.position.x - spawnDist, Player.Instance.transform.position.x - offsetFromPlayer);
                    else
                        spawnPoint.x = UnityEngine.Random.Range(Player.Instance.transform.position.x + offsetFromPlayer, Player.Instance.transform.position.x + spawnDist);
                    spawnPoint.y = UnityEngine.Random.Range(Player.Instance.transform.position.y - spawnDist, Player.Instance.transform.position.y + spawnDist);

                    try
                    {
                        if (Vector2.Distance(spawnPoint, Player.Instance.transform.position) > (spawnDist / 2) && CurrentMap[(int)spawnPoint.x, (int)spawnPoint.y] == 0)
                        {
                            bool rightTime = true;
                            if (enemyToSpawn.nightMob)
                                if (GameManager.Instance.GetComponent<TODManager>().isDay)
                                    rightTime = false;
                            if (rightTime)
                            {
                                GameObject go = (GameObject)Instantiate(enemyToSpawn.gm, spawnPoint, Quaternion.identity);
                                entityCount++;
                                currentEnemies.Add(go);
                                if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                    TerrainLighting.lightObjects.Add(go.name, go);
                            }
                        }
                    }
                    catch (NullReferenceException)
                    {
                        bool rightTime = true;
                        if (enemyToSpawn.nightMob)
                            if (GameManager.Instance.GetComponent<TODManager>().isDay)
                                rightTime = false;
                        if (rightTime)
                        {
                            GameObject go = (GameObject)Instantiate(enemyToSpawn.gm, spawnPoint, Quaternion.identity);
                            entityCount++;
                            currentEnemies.Add(go);
                            if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                                TerrainLighting.lightObjects.Add(go.name, go);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    { }
                }
            }

            CancelInvoke ();
		}
	}

    public void SpawnSpecificEnemy(GameObject enemy)
    {
        bool spawned = false;
        if (!Player.Instance.isDead && SettingsManager.Instance.enemySpawningToggle.isOn)
        {
            while (!spawned)
            {
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                    spawnPoint.x = UnityEngine.Random.Range(Player.Instance.transform.position.x - spawnDist, Player.Instance.transform.position.x - offsetFromPlayer);
                else
                    spawnPoint.x = UnityEngine.Random.Range(Player.Instance.transform.position.x + offsetFromPlayer, Player.Instance.transform.position.x + spawnDist);
                spawnPoint.y = UnityEngine.Random.Range(Player.Instance.transform.position.y - spawnDist, Player.Instance.transform.position.y + spawnDist);

                try
                {
                    if (Vector2.Distance(spawnPoint, Player.Instance.transform.position) > (spawnDist / 2) && CurrentMap[(int)spawnPoint.x, (int)spawnPoint.y] == 0)
                    {
                        GameObject go = (GameObject)Instantiate(enemy, spawnPoint, Quaternion.identity);
                        entityCount++;
                        currentEnemies.Add(go);
                        if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                            TerrainLighting.lightObjects.Add(go.name, go);
                        spawned = true;
                    }
                }
                catch (NullReferenceException)
                {
                    GameObject go = (GameObject)Instantiate(enemy, spawnPoint, Quaternion.identity);
                    entityCount++;
                    currentEnemies.Add(go);
                    if (!TerrainLighting.lightObjects.ContainsKey(go.name))
                        TerrainLighting.lightObjects.Add(go.name, go);
                    spawned = true;
                }
                catch (IndexOutOfRangeException)
                { }
            }
        }
    }
}
using UnityEngine;
using System.Collections;

public class BaseGenerator : MonoBehaviour
{
	public bool renderImmediate = true;

	TerrainMap terrainMap = null;
	TerrainMap backMap = null;
	TerrainMap brushMap = null;

	public int MapWidth
	{
		get
		{
			if (terrainMap == null)
                terrainMap = GetComponent<TerrainMap> ();

			return terrainMap.Map.GetLength (0);
		}
	}

	public int MapHeight
	{
		get
		{
			if (terrainMap == null) terrainMap = GetComponent<TerrainMap> ();

			return terrainMap.Map.GetLength (1);
		}
	}

	public int [,] CurrentMap 
	{
		get
		{
			if (terrainMap == null) terrainMap = GameObject.Find ("Terrain").GetComponent<TerrainMap> ();

			return terrainMap.Map;
		}
		set
		{
			if (terrainMap == null) terrainMap = GetComponent<TerrainMap> ();
			terrainMap.Map = value;
		}
	}

	public int [,] CurrentBackMap
	{
		get
		{
			if (backMap == null) backMap = GameObject.Find ("Terrain").GetComponent<TerrainMap> ();

			return backMap.BackMap;
		}
		set
		{
			if (backMap == null) backMap = GetComponent<TerrainMap> ();
			backMap.BackMap = value;
		}
	}

	public int [,] CurrentBrushMap
	{
		get
		{
			if (brushMap == null) brushMap = GameObject.Find ("Terrain").GetComponent<TerrainMap> ();

			return brushMap.BrushMap;
		}
		set
		{
			if (brushMap == null) brushMap = GetComponent<TerrainMap> ();
			brushMap.BackMap = value;
		}
	}

	public void Render()
	{
		TerrainRenderer terrainRenderer = GetComponent<TerrainRenderer> ();
		terrainRenderer.ClearImmediate ();
		terrainRenderer.Render ();
	}
}

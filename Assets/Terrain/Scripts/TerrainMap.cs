using UnityEngine;
using System.Collections;

public class TerrainMap : MonoBehaviour 
{
	public int mapWidth=30;
	public int mapHeight=20;

	public int[,] map;
	public int[,] backMap;
	public int[,] brushMap;

	public int[,] Map
	{
		get
		{
			if (map == null) map = new int[mapWidth, mapHeight];

			return map;
		}
		set { map = value; }
	}

	public int[,] BackMap
	{
		get
		{
			if (backMap == null) backMap = new int[mapWidth, mapHeight];

			return backMap;
		}
		set
		{ backMap = value; }
	}

	public int[,] BrushMap
	{
		get
		{
			if (brushMap == null) brushMap = new int[mapWidth, mapHeight];

			return brushMap;
		}
		set { brushMap = value; }
	}

	public void Reset()
	{
		map = null;
		backMap = null;
	}
}

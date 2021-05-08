using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class OreDetail
{
	public string Name;
	public int StartDepth;
	public int EndDepth;
	public float Abundance;
	public int OreIndex;
}

public class OreGenerator : BaseGenerator 
{
	public List<OreDetail> oreList;

	public int deathLimit = 4;
	public int birthLimit= 4;
	public int numberOfSteps = 3;

    public int dirtID;

    public void Append ()
	{
		foreach (var ore in oreList)
		{
			GenerateOre (ore.StartDepth, ore.EndDepth, CurrentMap, ore.Abundance, ore.OreIndex);
		}
		if (renderImmediate)
            Render ();
	}

	void GenerateOre(int startDepth, int endDepth, int[,] map, float abundance, int oreIndex)
	{
		for (int y = startDepth; y < endDepth; y++)
		{
			for (int x = 0; x < map.GetLength (0); x++)
			{
				if (CurrentMap[x,y] != 0 && CurrentMap[x, y] == dirtID)
				{
					float randy = Random.Range (0f, 1f);
					if (randy < abundance)
                        CurrentMap[x, y] = oreIndex;
				}
			}
		}

		for (int i = 0; i < numberOfSteps; i++)
		{
			for (int y = startDepth; y < endDepth; y++)
			{
				for (int x = 1; x < map.GetLength (0) - 1; x++)
				{
					if (CurrentMap[x,y] == dirtID || CurrentMap[x,y] == oreIndex)
					{
						int nbs = CountAliveNeighbours (CurrentMap, x, y, oreIndex);

						if(CurrentMap[x, y] == oreIndex)
						{
							if(nbs < deathLimit)
							{
								CurrentMap[x, y] = dirtID;
							}
							else
							{
								CurrentMap[x, y] = oreIndex;
							}
						}
						else
						{
							if(nbs > birthLimit)
							{
								CurrentMap[x, y] = oreIndex;
							}
							else
							{
								CurrentMap[x, y] = dirtID;
							}
						}
					}
				}
			}
		}
	}

	int CountAliveNeighbours(int[,] map, int x, int y, int aliveIndex)
	{
		int count = 0;

		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				int neighbour_x = x + i;
				int neighbour_y = y + j;

				if(i == 0 && j == 0)
				{ }
				else if(neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength (0) || neighbour_y >= map.GetLength (1))
				{
					count = count + 1;
				}
				else if(map[neighbour_x,neighbour_y] == aliveIndex)
				{
					count = count + 1;
				}
			}
		}

		return count;
	}

	void FillAll()
	{
		for (int x = 0; x < CurrentMap.GetLength (0); x++) 
		{
			for (int y = 0; y < CurrentMap.GetLength (1); y++)
			{
				CurrentMap[x, y] = dirtID;
			}
		}
	}
}

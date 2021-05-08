using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaveGenerator : BaseGenerator
{
	public float initialChance = 0.48f;
	public int deathLimit = 4;
	public int birthLimit= 4;
	public int numberOfSteps = 3;
	public bool fillEdges = false;
	public bool oneCave = false;
	public int yOffset = 0;
	public enum CATypes { type1, type2 };
	public CATypes caType;

	int startFillIndex = 999;

    public int dirtID;

	public struct Point
	{
		public int x, y;
		public Point (int px, int py)
		{
			x = px;
			y = py;
		}
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

	public void GenerateMap()
	{
		for (int x = 0; x < CurrentMap.GetLength (0); x++) 
		{
			for (int y = 0; y < CurrentMap.GetLength (1) - yOffset; y++) 
			{
				if (Random.Range(0f, 1f) > initialChance)
                    CurrentMap[x, y] = 0;
			}
		}

		for(int i = 0; i < numberOfSteps; i++)
		{
			switch (caType) 
			{
				case CATypes.type1:
					CurrentMap = DoSimulationStep (CurrentMap);
					break;
				case CATypes.type2:
					DoSimulationStep ();
					break;
			}
		}
	}

	public void Generate()
	{
		FillAll();
		GenerateMap();

		if (fillEdges) FillInEdges(CurrentMap);

		if (oneCave) FillAllButLargest(CurrentMap);

		if (renderImmediate) Render ();
	}

	void DoSimulationStep()
	{
		for (int x = 0; x < CurrentMap.GetLength (0); x++) 
		{
			for (int y = 0; y < CurrentMap.GetLength (1); y++)
			{
				int nbs = CountAliveNeighbours(CurrentMap, x, y);
				if (CurrentMap[x, y] == dirtID)
				{
					if (nbs < deathLimit)
					{
						CurrentMap[x, y] = 0;
					}
					else
					{
						CurrentMap[x, y] = dirtID;
					}
				}
				else
				{
					if (nbs > birthLimit)
					{
						CurrentMap[x, y] = dirtID;
					}
					else
					{
						CurrentMap[x, y] = 0;
					}
				}
			}
		}
	}

	int[,] DoSimulationStep(int[,] oldMap)
	{
		int[,] newMap = new int[oldMap.GetLength (0), oldMap.GetLength (1)];

		for (int x = 0; x < oldMap.GetLength (0); x++)
		{
			for (int y = 0; y < oldMap.GetLength (1); y++) 
			{
				int nbs = CountAliveNeighbours(oldMap, x, y);
				if (oldMap[x, y] == dirtID)
				{
					if (nbs < deathLimit)
					{
						newMap[x, y] = 0;
					}
					else
					{
						newMap[x, y] = dirtID;
					}
				}
				else
				{
					if (nbs > birthLimit)
					{
						newMap[x, y] = dirtID;
					}
					else
					{
						newMap[x, y] = 0;
					}
				}
				if (y > oldMap.GetLength (1) - yOffset) newMap[x, y] = oldMap[x, y];
			}
		}

		return newMap;
	}

	int CountAliveNeighbours(int[,] map, int x, int y)
	{
		int count = 0;
		for(int i = -1; i < 2; i++)
		{
			for(int j = -1; j < 2; j++)
			{
				int neighbour_x = x + i;
				int neighbour_y = y + j;

				if (i == 0 && j == 0)
				{ }
				else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength (0) || neighbour_y >= map.GetLength (1))
				{
					count = count + 1;
				}
				else if (map[neighbour_x,neighbour_y] == dirtID)
				{
					count = count + 1;
				}
			}
		}

		return count;
	}

	public void FillInEdges(int[,] map)
	{
		for (int x = 0; x < map.GetLength (0); x++) 
		{
			map[x, 0] = dirtID;
			map[x, map.GetLength (1) - 1 -yOffset] = dirtID;
		}
		for (int y = 0; y < map.GetLength (1) - yOffset; y++) 
		{
			map[0, y] = dirtID;
			map[map.GetLength (0) - 1, y] = dirtID;
		}
	}

	public void FillAllBut(int[,] oldMap, int fillException, int fillWith)
	{
		for (int x = 0; x < oldMap.GetLength (0); x++)
		{
			for (int y = 0; y < oldMap.GetLength (1); y++)
			{
				if(oldMap[x, y] == fillException)
				{
					oldMap[x, y] = 0;
				}
				else
				{
					oldMap[x, y] = fillWith;
				}
			}
		}
	}

	public void FillAllButLargest(int[,] map)
	{
		List<Point> fillCounts = FloodFill (map);
		int largestSoFar = 0; 
		int fillIndex = dirtID;
		foreach (var fillCount in fillCounts) 
		{
			if (fillCount.y > largestSoFar)
			{
				fillIndex = fillCount.x;
				largestSoFar = fillCount.y;
			}
		}

		FillAllBut (map, fillIndex, dirtID);
	}

	public List<Point> FloodFill(int[,] oldMap)
	{
		int fillIndex = startFillIndex;
		List<Point> fillCounts = new List<Point>();

		for (int x = 0; x < oldMap.GetLength (0); x++) 
		{
			for (int y = 0; y < oldMap.GetLength (1); y++)
			{
				var open = new Queue<Point> ();
				var fillCount = new Point(fillIndex, 0);

				if (oldMap[x,y] == 0)
				{
					open.Enqueue (new Point(x, y));
					oldMap[x, y] = fillIndex;

					while (open.Count > 0)
					{
						var current = open.Dequeue ();
						int currentX = current.x;
						int currentY = current.y;

						if (current.x > 0)
						{
							if (oldMap[currentX-1, currentY] == 0)
							{
								oldMap[currentX-1, currentY]= fillIndex;
								open.Enqueue (new Point (currentX-1, currentY));
							}
						}
						if (current.x < oldMap.GetLength (0) - 1)
						{
							if (oldMap[currentX + 1, currentY] == 0)
							{
								oldMap[currentX + 1, currentY] = fillIndex;
								open.Enqueue (new Point (currentX + 1, currentY));
							}
						}
						if (current.y > 0)
						{
							if (oldMap[currentX, currentY - 1] == 0)
							{
								oldMap[currentX, currentY - 1] = fillIndex;
								open.Enqueue (new Point (currentX, currentY - 1));
							}
						}
						if (current.y < oldMap.GetLength (1) - 1)
						{
							if (oldMap[currentX, currentY + 1] == 0)
							{
								oldMap[currentX, currentY + 1] = fillIndex;
								open.Enqueue (new Point (currentX, currentY + 1));
							}
						}
						fillCount.y++;
					}
					fillCounts.Add (fillCount);
					fillIndex++;
				}
			}
		}

		return fillCounts;
	}

	public Point? FindNearestSpace(int[,] map, Point startPoint)
	{
		var open = new Queue<Point> ();
		int[,] tempmap = new int[map.GetLength (0), map.GetLength (1)];
		tempmap[startPoint.x,startPoint.y] = dirtID;

		open.Enqueue(new Point (startPoint.x, startPoint.y));
		while (open.Count > 0) 
		{
			var current = open.Dequeue ();
			if (map[current.x, current.y] == 0)
			{
				return new Point(current.x, current.y);				
			}
			else
			{
				if (current.x > 0)
				{
					if (map[current.x-1,current.y] == 0)
					{
						return new Point (current.x - 1, current.y);
					}
					else
					{
						if (tempmap[current.x - 1,current.y] == 0)
						{
							tempmap[current.x - 1,current.y] = dirtID;
							open.Enqueue (new Point (current.x - 1, current.y));
						}
					}
				}
				if (current.x < map.GetLength (0) - 1)
				{
					if (map[current.x + 1, current.y] == 0)
					{
						return new Point (current.x + 1, current.y);	
					}
					else
					{
						if (tempmap[current.x + 1, current.y] == 0)
						{
							tempmap[current.x + 1, current.y] = dirtID;
							open.Enqueue (new Point (current.x + 1, current.y));
						}
					}
				}
				if (current.y > 0)
				{
					if (map[current.x, current.y - 1] == 0)
					{
						return new Point(current.x, current.y - 1);	
					}
					else
					{
						if (tempmap[current.x, current.y - 1] == 0)
						{
							tempmap[current.x,current.y - 1] = dirtID;
							open.Enqueue (new Point (current.x, current.y - 1));
						}
					}
				}
				if (current.y < map.GetLength (1) - 1)
				{
					if (map[current.x, current.y + 1] == 0)
					{
						return new Point(current.x,current.y+1);	
					}
					else
					{
						if (tempmap[current.x, current.y + 1] == 0)
						{
							tempmap[current.x,current.y + 1] = dirtID;
							open.Enqueue (new Point (current.x, current.y + 1));
						}
					}
				}
			}
		}

		return null;
	}
}
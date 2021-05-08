using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BrushDetail
{
    public string name;
    public int maxHeight;
    public float abundance;
    public int brushIndex;
}

public class LandGenerator : BaseGenerator
{
    public List<BrushDetail> brushList;

	public float heightScale = 1.0f;
	public float xScale = 1.0f;
	public float yNoise = 0f;
	public float yOffset = 0f;

	public int seaLevel = 0;

	public float treeBranchAbundance = 0f;
	public float treeAbundance = 0f;
	public float waterAbundance = 0f;

	public int minShrubbleSpawn = 100;
	public int maxShrubbleSpawn = 200;

	public int minTreeHeight = 4;
	public int maxTreeHeight = 6;

	public int minTreeSpawn = 100;
	public int maxTreeSpawn = 200;

	public AnimationCurve contourCurve = AnimationCurve.Linear (0f, 0f, 1f, 1f);

    public int dirtID;
    public int backwallID;
    public int grassID;
    public int logID;
    public int brushID;
    public int leaveID;
    public int liquidID;

    public void GenerateMap ()
	{
		GenerateHills ();

        GenerateWater();

        GenerateBackWall ();

		GenerateGrass ();

		GenerateTrees ();

		GenerateBrush ();
	}

	public void Append ()
	{
		GenerateMap ();
		if (renderImmediate) Render ();
	}

	public void Generate ()
	{
		for (int y = 0; y < MapHeight; y++) 
		{
			for (int x = 0; x < MapWidth; x++) 
			{
				CurrentMap [x, y] = dirtID;
			}
		}

		GenerateMap ();
		if (renderImmediate)
            Render ();
	}

	void GenerateHills()
	{
		float height = 0f;

		for (int y = 0; y < MapHeight; y++) 
		{
			for (int x = 0; x < MapWidth; x++) 
			{
				CurrentBackMap [x, y] = backwallID;

				float xf = (float)x / (float)(MapWidth - 1);
				float yf = (float)(y - yOffset) / (float)(MapHeight - 1);

				height = contourCurve.Evaluate (xf) * heightScale * Mathf.PerlinNoise (xf * xScale, yNoise);

				if (yf > height) 
				{
					CurrentMap [x, y] = 0;
					CurrentBackMap [x, y] = 0;
				}
			}
		}
	}

	void GenerateGrass()
	{
		for (int y = minShrubbleSpawn; y < maxShrubbleSpawn; y++)
		{
			for (int x = 0; x < MapWidth; x++) 
			{
				if (y < MapHeight) if (CurrentMap [x, y] == dirtID && CurrentMap [x, y + 1] == 0 && CurrentBackMap [x, y + 1] != backwallID)
                        CurrentMap [x, y] = grassID;
			}
		}
	}

	void GenerateBrush()
	{
        for (int y = (int)yOffset; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (y < MapHeight)
                {
                    int bRandy = Random.Range(0, brushList.Count);
                    var b = brushList[bRandy];
                    float randy = Random.Range(0f, 1f);
                    if (randy < b.abundance)
                    {
                        if (CurrentMap[x, y] == grassID && CurrentMap[x, y + 1] == 0 && CurrentBrushMap[x, y + 1] == 0)
                        {
                            int brushHeight = Random.Range(1, b.maxHeight);
                            for (int i = 1; i <= brushHeight; i++)
                            {
                                CurrentBrushMap[x, y + i] = b.brushIndex;
                            }
                        }
                    }
                }
            }
        }
	}

	void GenerateTrees()
	{
		for (int y = minTreeSpawn; y < maxTreeSpawn; y++)
		{
			for (int x = 0; x < MapWidth; x++)
			{
				if (y < (int)((float)MapHeight / 1.5f) && x > 2 && x < (MapWidth - 2))
				{
					int treeRandy = Random.Range (minTreeHeight, maxTreeHeight);
					if (CurrentMap [x, y] == grassID && CurrentMap [x, y + 1] == 0 && CurrentBrushMap[x, y + 1] == 0 && CurrentBrushMap [x - 1, y + 1] == 0 
						&& CurrentBrushMap [x + 1, y + 1] == 0 && CurrentBrushMap[x, y + treeRandy] == 0 && CurrentBrushMap[x, y + treeRandy * 2] == 0)
					{
						float randy = Random.Range (0f, 1f);
						if (randy < treeAbundance) 
						{
							for (int i = 0; i <= treeRandy; i++) 
							{
								CurrentBrushMap [x, y + i + 1] = logID;

								float treeBranchRandy = Random.Range (0f, 1f);
								if (treeBranchRandy < treeBranchAbundance) 
								{
									int treeBranchSide = Random.Range (0, 2);
									if (treeBranchSide == 0) 
									{
										CurrentBrushMap [x + 1, y + i + 1] = logID;
									}
									else 
									{
										CurrentBrushMap [x - 1, y + i + 1] = logID;
									}
								}
							}
							for (int i = (x - 1); i <= (x + 1); i++) 
							{
								for (int j = (y + treeRandy); j <= (y + treeRandy + 1); j++) 
								{
									CurrentBrushMap [i, j] = leaveID;
								}
							}
						}
					}
				}
			}
		}
	}

	void GenerateWater()
	{
		bool justWatered = false;
		bool justWateredInCave = false;
		float waterRandy;
        int tempX = 0;
        int tempY = 0;

        for (int y = 0; y < seaLevel; y++)
        {
            tempX = 0;
            tempY++;
            waterRandy = Random.Range(0f, 1f);
            if (waterRandy < waterAbundance)
                justWatered = false;

            for (int x = 0; x < MapWidth; x++)
            {
                tempX++;
                if (justWateredInCave)
                {
                    waterRandy = Random.Range(0f, 1f);
                    if (waterRandy < waterAbundance)
                    {
                        justWatered = false;
                    }
                    else
                    {
                        justWatered = true;
                        justWateredInCave = true;
                    }
                    justWateredInCave = false;
                }

                if (CurrentMap[x, y] == 0 && !justWatered && x > 1 && x < MapWidth - 1)
                {
                    if (CurrentMap[x - 1, y] == dirtID || CurrentMap[x - 1, y] == liquidID || CurrentMap[x + 1, y] == liquidID || CurrentMap[x - 1, y] == 0
                        || CurrentMap[x + 1, y] == 0)
                    {
                        if (CurrentMap[x + 1, y] == dirtID && CurrentMap[x - 1, y] == liquidID)
                        {
                            CurrentMap[x, y] = liquidID;
                            justWateredInCave = true;
                        }
                        else
                        {
                            CurrentMap[x, y] = liquidID;
                        }
                    }
                }
            }
        }

        justWatered = true;

        for (int y = seaLevel; y > 0; y--)
        {
            for (int x = MapWidth - 1; x > 0; x--)
            {
                if (x > 1 && x < MapWidth - 1 && y < seaLevel - 1 && y > 1)
                {
                    if (CurrentMap[x, y] == 0)
                        if (CurrentMap[x, y + 1] == liquidID || CurrentMap[x + 1, y] == liquidID || CurrentMap[x - 1, y] == liquidID)
                            CurrentMap[x, y] = liquidID;

                    if (CurrentMap[x, y] == liquidID)
                    {
                        if (CurrentMap[x + 1, y] == 0)
                            CurrentMap[x + 1, y] = liquidID;
                        if (CurrentMap[x - 1, y] == 0)
                            CurrentMap[x - 1, y] = liquidID;
                    }
                }
            }
        }

        for (int y = 0; y < seaLevel; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (x > 1 && x < MapWidth - 1 && y > 1 && y < seaLevel - 1)
                {
                    if (CurrentMap[x, y] == liquidID)
                    {
                        if (CurrentMap[x + 1, y] == 0)
                            CurrentMap[x + 1, y] = liquidID;
                        if (CurrentMap[x - 1, y] == 0)
                            CurrentMap[x - 1, y] = liquidID;
                    }
                }
            }
        }
    }

	void GenerateBackWall()
	{
		for (int y = 0; y < MapHeight; y++) 
		{
			for (int x = 0; x < MapWidth; x++) 
			{
                if (y > 1 && y < MapHeight - 1 && x > 1 && x < MapWidth - 1)
                {
                    if (CurrentMap[x, y] != 0)
                    {
                        CurrentBackMap[x, y] = backwallID;
                    }
                }
			}
		}
	}
}
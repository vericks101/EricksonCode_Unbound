using UnityEngine;

public class MineshaftGenerator : BaseGenerator
{
    public int xOffset;

    public int rewardCount;
    public ChestItem[] chestRewards;

    public Structure[] structures;

    public ColorToPrefab[] colorMappings;

    public void GenerateChestRewards(int x, int y)
    {
        string content = string.Empty;

        for (int i = 0; i < rewardCount; i++)
        {
            ChestItem randomItem = chestRewards[Random.Range(0, chestRewards.Length)];
            content += i + "-" + randomItem.name + "-" + randomItem.amount + ";";
        }

        CodecManager.Instance.SaveChestData(null, null, null, true, content, "brush x " + x + " brush y " + y);
    }

    public void GenerateStructure()
    {
        for (int y = structures[0].lowerDepth; y < structures[0].upperDepth; y++)
        {
            for (int x = xOffset; x < CurrentMap.GetLength(0) - xOffset; x++)
            {
                if (Random.Range(0f, 1f) < structures[0].spawnChance)
                {
                    int random = Random.Range(0, structures.Length);

                    for (int i = 0; i < structures[random].currentMap.width; i++)
                    {
                        for (int j = 0; j < structures[random].currentMap.height; j++)
                        {
                            GenerateTile(i, j, x, y, random);
                        }
                    }
                    x += structures[random].currentMap.width;
                }
            }
            y += structures[0].currentMap.height;
        }
    }

    private void GenerateTile(int i, int j, int x, int y, int random)
    {
        Color currentPixelColor = structures[random].currentMap.GetPixel(i, j);
        if (currentPixelColor.a != 0f)
        {
            foreach (ColorToPrefab colorMapping in colorMappings)
            {
                if (colorMapping.color.Equals(currentPixelColor))
                {
                    try
                    {
                        CurrentMap[i + x, j + y] = colorMapping.id;
                    }
                    catch (System.IndexOutOfRangeException)
                    { }
                }
            }
        }
        Color backPixelColor = structures[random].backMap.GetPixel(i, j);
        if (backPixelColor.a != 0f)
        {
            foreach (ColorToPrefab colorMapping in colorMappings)
            {
                if (colorMapping.color.Equals(backPixelColor))
                {
                    try
                    {
                        CurrentBackMap[i + x, j + y] = colorMapping.id;
                    }
                    catch (System.IndexOutOfRangeException)
                    { }
                }
            }
        }
        Color brushPixelColor = structures[random].brushMap.GetPixel(i, j);
        if (brushPixelColor.a != 0f)
        {
            foreach (ColorToPrefab colorMapping in colorMappings)
            {
                if (colorMapping.color.Equals(brushPixelColor))
                {
                    try
                    {
                        CurrentBrushMap[i + x, j + y] = colorMapping.id;
                        if (colorMapping.id == 9)
                            GenerateChestRewards(i + x, j + y);
                    }
                    catch (System.IndexOutOfRangeException)
                    { }
                }
            }
        }
    }
}
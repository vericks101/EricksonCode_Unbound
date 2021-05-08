using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TileData 
{
	public string name;
	public int health;
	public int itemID;
    public ItemType itemType;
    public int layerId;
    public int listIndex;
	public Sprite sprite;
	public bool boxCollider;
    public bool isTrigger;
    public bool canDamage;
    public bool gravityChanger;
    public bool canSpread;
    public int itemToSpreadTo;
    public bool canGrowOn;
	public bool blank;
}

[System.Serializable]
public class TileSet 
{
	public string Name;
	public List<TileData> tileData;

	public Sprite GetSprite(int index)
	{
		return tileData[index].sprite;
	}

	public bool IsBoxCollider(int index)
	{
		return tileData[index].boxCollider;
	}

    public bool IsTrigger(int index)
    {
        return tileData[index].isTrigger;
    }
}
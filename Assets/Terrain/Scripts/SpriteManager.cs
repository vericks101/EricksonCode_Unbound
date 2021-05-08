using UnityEngine;
using System.Collections;

public class SpriteManager : MonoBehaviour 
{
	private int health;
    private string itemName;
	private int itemID;
    private ItemType itemType;
    private int layerId;
    private int listIndex;
	private bool isLit = false;
    private bool canDamage;
    private bool gravityChanger;
    public bool canSpread;
    public int itemToSpreadTo;
    public bool canGrowOn;
    public bool isTorched;

	public int Health
	{
		get { return health; }
		set { health = value; }
	}

    public string ItemName
    {
        get { return itemName; }
        set { itemName = value; }
    }

	public int ItemID
	{
		get { return itemID; }
		set { itemID = value; }
	}

    public ItemType ItemType
    {
        get { return itemType; }
        set { itemType = value; }
    }

    public int LayerID
    {
        get { return layerId; }
        set { layerId = value; }
    }

    public int ListIndex
    {
        get { return listIndex; }
        set { listIndex = value; }
    }

    public bool IsLit
	{
		get { return isLit; }
		set { isLit = value; }
	}

    public bool CanDamage
    {
        get { return canDamage; }
        set { canDamage = value; }
    }

    public bool GravityChanger
    {
        get { return gravityChanger; }
        set { gravityChanger = value; }
    }

    public bool CanSpread
    {
        get { return canSpread; }
        set { canSpread = value; }
    }

    public int ItemToSpreadTo
    {
        get { return itemToSpreadTo; }
        set { itemToSpreadTo = value; }
    }

    public bool CanGrowOn
    {
        get { return canGrowOn; }
        set { canGrowOn = value; }
    }

    public void DamageSprite(int damage)
	{
		health -= damage;
	}

    void OnTriggerStay2D(Collider2D other)
    {
        if ((other.name == "Player" || other.name == "Enemy") && Time.time > GameManager.Instance.liquidCooldownTimer && canDamage)
        {
            Player.Instance.DamagePlayer(10f, 0f, new Vector2(0f, 0f), 0f, 0f);
            GameManager.Instance.liquidCooldownTimer = Time.time + GameManager.Instance.liquidDmgCooldown;
        }

        if (other.name == "Player" && gravityChanger)
            Player.Instance.GetComponent<Rigidbody2D>().gravityScale = GameManager.Instance.inWaterGravity;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "Player" && gravityChanger)
            Player.Instance.GetComponent<Rigidbody2D>().gravityScale = GameManager.Instance.normalGavity;
    }
}

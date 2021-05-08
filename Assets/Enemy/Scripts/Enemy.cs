using UnityEngine;
using System.Collections;

public enum MoveType { FLYING, WALKING }

[System.Serializable]
public class EnemyDrop
{
    public float dropChance;
    public string itemName;
}

public class Enemy : MonoBehaviour 
{
    public string enemyName;
	[SerializeField] private float damage = 0f;
    [SerializeField] private float maxHealth = 100f;
	[SerializeField] private float health = 0f;
	[SerializeField] private float speed = 2f;
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float knockPow = 200f;
    [SerializeField] private float wallHitKnockAmt = 200f;
    [SerializeField] private float shakeAmt = 0.1f;
	[SerializeField] private float shakeTime = 0.1f;
	[SerializeField] private float enemyScaleSize = 1.5f;
    [SerializeField] private float inverseScale = 1;
    [SerializeField] private float altInverseScale = -1;

	[SerializeField] private Transform target;
	[SerializeField] private float agroDist;
	[SerializeField] private float despawnRange;

	[SerializeField] private Transform wallCheck;
	[SerializeField] private float wallCheckRadius;
	[SerializeField] private LayerMask whatIsWall;
	private bool hittingWall;

    [SerializeField] private int moneyReward;
    [SerializeField] private int expReward;
    [SerializeField] private EnemyDrop[] drops;

    [SerializeField] private MoveType moveType;

    private bool moveRight;
    private bool grounded;
    [SerializeField] private float groundedRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private GameObject bloodPrefab;

    public bool isTorched;

    private void Awake()
	{
		target = GameObject.FindGameObjectWithTag ("Player").transform;
        health = maxHealth;
        GetComponentInChildren<StatusIndicator>().SetHealth((int)health, (int)maxHealth);
    }

	private void FixedUpdate()
	{
		MoveEnemy ();
	}

	private void MoveEnemy()
	{
        if (transform.localScale.x < 0f)
            altInverseScale = -inverseScale;
        else if (transform.localScale.x > 0f)
            altInverseScale = 1;

        if (moveType == MoveType.FLYING)
        {
            hittingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsWall);
            if (hittingWall)
                moveRight = !moveRight;

            if (target != null)
            {
                var toTargetDist = Vector2.Distance(transform.position, target.position);
                if (toTargetDist > despawnRange)
                {
                    SpawningManager.Instance.entityCount--;
                    Destroy(gameObject);
                }

                if (toTargetDist > agroDist && moveRight)
                {
                    transform.localScale = new Vector3(inverseScale * enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(altInverseScale * GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x, 
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + 1, transform.position.y), speed * Time.deltaTime);
                }
                else if (toTargetDist > agroDist && !moveRight)
                {
                    transform.localScale = new Vector3(inverseScale * -enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(altInverseScale * -GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x - 1, transform.position.y), speed * Time.deltaTime);
                }
                if (toTargetDist < agroDist && transform.position.x < target.position.x)
                {
                    transform.localScale = new Vector3(inverseScale * enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(altInverseScale * GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                    moveRight = true;
                }
                else if (toTargetDist < agroDist && transform.position.x > target.position.x)
                {
                    transform.localScale = new Vector3(inverseScale * -enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(altInverseScale * -GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                    moveRight = false;
                }
            }
            else
            {
                if (GameObject.FindGameObjectWithTag("Player") != null) target = GameObject.FindGameObjectWithTag("Player").transform;

                if (moveRight)
                {
                    transform.localScale = new Vector3(enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + 1, transform.position.y), speed * Time.deltaTime);
                }
                else if (!moveRight)
                {
                    transform.localScale = new Vector3(-enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(-GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x - 1, transform.position.y), speed * Time.deltaTime);
                }
            }
        }
        else if (moveType == MoveType.WALKING)
        {
            grounded = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    grounded = true;
            }

            hittingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsWall);
            if (hittingWall && grounded)
            {
                grounded = false;
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
            }

            if (target != null)
            {
                var toTargetDist = Vector2.Distance(transform.position, target.position);
                if (toTargetDist > despawnRange)
                {
                    SpawningManager.Instance.entityCount--;
                    Destroy(gameObject);
                }

                if (toTargetDist > agroDist && moveRight)
                {
                    transform.localScale = new Vector3(inverseScale * enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(altInverseScale * GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);
                }
                else if (toTargetDist > agroDist && !moveRight)
                {
                    transform.localScale = new Vector3(inverseScale * -enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(altInverseScale * -GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, GetComponent<Rigidbody2D>().velocity.y);
                }
                if (toTargetDist < agroDist && transform.position.x < target.position.x)
                {
                    transform.localScale = new Vector3(inverseScale * enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(altInverseScale * GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);
                    moveRight = true;
                }
                else if (toTargetDist < agroDist && transform.position.x > target.position.x)
                {
                    transform.localScale = new Vector3(inverseScale * -enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(altInverseScale * -GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, GetComponent<Rigidbody2D>().velocity.y);
                    moveRight = false;
                }
            }
            else
            {
                if (GameObject.FindGameObjectWithTag("Player") != null) target = GameObject.FindGameObjectWithTag("Player").transform;

                if (moveRight)
                {
                    transform.localScale = new Vector3(enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + 1, transform.position.y), speed * Time.deltaTime);
                }
                else if (!moveRight)
                {
                    transform.localScale = new Vector3(-enemyScaleSize, enemyScaleSize, 1f);
                    GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale = new Vector3(-GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.x,
                        GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.y, GetComponentInChildren<StatusIndicator>().GetComponent<RectTransform>().localScale.z);
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x - 1, transform.position.y), speed * Time.deltaTime);
                }
            }
        }
	}

	public void DamageEnemy(float damage, Vector2 hitNormal, float knockPow, bool crit)
	{
        float strengthIncrease = Mathf.Ceil(damage * Player.Instance.damageIncreaseFactor);
        damage = CharacterPanel.Instance.weaponDamage + strengthIncrease;

        if (CurrentSceneManager.Instance.planetElement == CharacterPanel.Instance.weaponElement || CharacterPanel.Instance.weaponElement == Element.LUMINANT
            || CharacterPanel.Instance.weaponElement == Element.COMMON)
        {
            if (crit)
                damage *= 2f;
            health -= damage;
            GetComponentInChildren<StatusIndicator>().SetHealth((int)health, (int)maxHealth);

            CombatTextManager.Instance.CreateText(transform.position, damage.ToString(), Color.white, crit, false);
            KnockbackManager.Instance.ApplyKnockback(GetComponent<Rigidbody2D>(), knockPow, hitNormal);
            GameObject tmpBlood = Instantiate(bloodPrefab, transform.position, transform.rotation);
            Destroy(tmpBlood, 2f);
            AudioManager.instance.PlaySound("Hit_Hurt2");
        }
        else
        {
            damage = 1;
            health -= damage;
            GetComponentInChildren<StatusIndicator>().SetHealth((int)health, (int)maxHealth);

            CombatTextManager.Instance.CreateText(transform.position, damage.ToString(), Color.white, crit, false);
            KnockbackManager.Instance.ApplyKnockback(GetComponent<Rigidbody2D>(), knockPow, hitNormal);
            GameObject tmpBlood = Instantiate(bloodPrefab, transform.position, transform.rotation);
            Destroy(tmpBlood, 2f);
            AudioManager.instance.PlaySound("Hit_Hurt2");
        }

        if (health <= 0)
		{
			if (SpawningManager.Instance != null)
				SpawningManager.Instance.entityCount--;

            if (CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent != null && !CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent.specificKill)
            {
                CurrentSceneManager.Instance.GetComponent<EventManager>().killCounter++;
                CurrentSceneManager.Instance.GetComponent<EventManager>().eventFillImage.fillAmount = Mathf.Clamp01(((float)CurrentSceneManager.Instance.GetComponent<EventManager>().killCounter / 
                    (float)CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent.killsRequired));
            }

            GameManager.Instance.GetComponent<MoneyManager>().CreateMoney(moneyReward, transform.position);
            Player.Instance.GetComponent<EXPManager>().UpdateCurrentExperience(expReward);
            for (int i = 0; i < drops.Length; i++)
            {
                if (Random.Range(0f, 1f) < drops[i].dropChance)
                {
                    GameObject tmpDrp = (GameObject)GameObject.Instantiate(InventoryManager.Instance.dropItem, transform.position, transform.rotation);
                    tmpDrp.AddComponent<ItemScript>();
                    tmpDrp.AddComponent<BoxCollider2D>();
                    tmpDrp.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                    Item tmp = null;
                    if (tmp == null)
                    {
                        tmp = InventoryManager.Instance.ItemContainer.Consumeables.Find(item => item.ItemName == drops[i].itemName);
                    }
                    if (tmp == null)
                    {
                        tmp = InventoryManager.Instance.ItemContainer.Equipment.Find(item => item.ItemName == drops[i].itemName);
                    }
                    if (tmp == null)
                    {
                        tmp = InventoryManager.Instance.ItemContainer.Weapons.Find(item => item.ItemName == drops[i].itemName);
                    }
                    if (tmp == null)
                    {
                        tmp = InventoryManager.Instance.ItemContainer.Materials.Find(item => item.ItemName == drops[i].itemName);
                    }
                    if (tmp == null)
                    {
                        tmp = InventoryManager.Instance.ItemContainer.Placeables.Find(item => item.ItemName == drops[i].itemName);
                    }
                    tmpDrp.GetComponent<ItemScript>().Item = tmp;
                    tmpDrp.GetComponent<ItemScript>().itemCount = 1;
                    tmpDrp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tmp.ItemSprite);
                }
            }

            MenuManager.Instance.enemyKills++;
            QuestManager.Instance.enemyKilled = enemyName;
            SpawningManager.Instance.currentEnemies.Remove(gameObject);
            TerrainLighting.lightObjects.Remove(gameObject.name);
			Destroy (gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
        if (other.gameObject.tag != "Player")
        {
            KnockbackManager.Instance.ApplyKnockback(GetComponent<Rigidbody2D>(), wallHitKnockAmt, other.contacts[0].normal);
        }

		if (other.gameObject.GetComponent<Player> () != null) 
		{

			if (Player.Instance.shield.activeInHierarchy && other.gameObject.tag == "Player" && Time.time > other.gameObject.GetComponent<Player>().invincTimer)
			{
                var collisionNormal = other.contacts[0].normal;
                if (Player.Instance.mana.CurrentVal >= Player.Instance.GetComponentInChildren<Shield>().fuelCost)
                {
                    other.gameObject.GetComponent<Player>().DamagePlayer(Mathf.Ceil(Player.Instance.GetComponentInChildren<Shield>().shieldDef * damage),
                        knockPow, collisionNormal, shakeAmt, shakeTime);
                    Player.Instance.mana.CurrentVal -= Player.Instance.GetComponentInChildren<Shield>().fuelCost;
                }
                else
                    other.gameObject.GetComponent<Player>().DamagePlayer(damage, knockPow, collisionNormal, shakeAmt, shakeTime);
            }
            else if (!Player.Instance.shield.activeInHierarchy && other.gameObject.tag == "Player" && Time.time > other.gameObject.GetComponent<Player>().invincTimer)
            {
                var collisionNormal = other.contacts[0].normal;
                other.gameObject.GetComponent<Player>().DamagePlayer(damage, knockPow, collisionNormal, shakeAmt, shakeTime);
            }
			if (other.gameObject.GetComponent<Player> ().isDead)
                target = null;

			other.gameObject.GetComponent<Player> ().SetLayerRecursively (other.gameObject, 15);
		}
	}
}
using UnityEngine;
using System.Collections;

public enum GunType { ROCKET, PISTOL, ASSAULT_RIFLE }

public class Gun : MonoBehaviour 
{
    public GunType type;

	public float fireRate;
    public float fuelCost;
	public float damage;
	public float knockPow;
	public float velocity;
	public float shakeAmt;
	public float shakeTime;
	public LayerMask toHit;
	private float timeToFire = 0f;
	private Transform firePoint;

	public Transform bulletTrailPrefab;
	public Transform hitPrefab;
	public Transform muzzleFlashPrefab;
	public float minMFPSize;
	public float maxMFPSize;
	private float timeToSpawnEffect = 0f;
	public float effectSpawnRate;

    public string fireSound;

    [SerializeField] private float critChance;

	void Awake()
	{
		firePoint = transform.Find ("FirePoint");
	}

	void Update()
	{
		if (fireRate == 0f) 
		{
			if (Input.GetButtonDown ("Fire1") && GameManager.canUseWeapons && !Player.Instance.inBed) 
			{
                if (Player.Instance.mana.CurrentVal >= fuelCost)
                {
                    Player.Instance.mana.CurrentVal -= fuelCost;
                    Shoot();

                    CameraShake.Instance.Shake(shakeAmt, shakeTime);
                }
                else
                    AudioManager.instance.PlaySound("Error1");
			}
		} 
		else 
		{
			if (Input.GetButton ("Fire1") && Time.time > timeToFire && GameManager.canUseWeapons && !Player.Instance.inBed) 
			{
				timeToFire = Time.time + 1f / fireRate;
                if (Player.Instance.mana.CurrentVal >= fuelCost)
                {
                    Player.Instance.mana.CurrentVal -= fuelCost;
                    Shoot();

                    CameraShake.Instance.Shake(shakeAmt, shakeTime);
                }
                else
                    AudioManager.instance.PlaySound("Error1");
			}
		}
	}

	public void Shoot()
	{
        if (type != GunType.ROCKET)
        {
            Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);

            RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100f, toHit);
            if (hit.collider != null && hit.collider.GetComponent<Enemy>() != null)
            {
                float randy = Random.Range(0f, 1f);
                hit.collider.GetComponent<Enemy>().DamageEnemy(damage, -hit.normal, knockPow, (randy < critChance));
            }

            if (Time.time >= timeToSpawnEffect)
            {
                Vector2 hitPos;
                Vector3 hitNormal;

                if (hit.collider == null)
                {
                    hitPos = (mousePosition - firePointPosition) * 1000f;
                    hitNormal = new Vector3(9999f, 9999f, 9999f);
                }
                else
                {
                    hitPos = hit.point;
                    hitNormal = hit.normal;
                }

                Effect(hitPos, hitNormal);
                timeToSpawnEffect = Time.time + 1f / effectSpawnRate;
            }
        }
        else if (type == GunType.ROCKET)
        {
            Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
            Quaternion rotation = transform.parent.transform.rotation;
            GameObject tmp = (GameObject)Instantiate(bulletTrailPrefab.gameObject, firePointPosition, rotation);
            tmp.GetComponent<Missle>().speed = velocity;
            tmp.GetComponent<Missle>().damage = damage;
            tmp.GetComponent<Missle>().knockPow = knockPow;
        }

        AudioManager.instance.PlaySound(fireSound);
    }

	void Effect(Vector3 hitPos, Vector3 hitNormal)
	{
		Transform trail = (Transform)Instantiate (bulletTrailPrefab, firePoint.position, firePoint.rotation);

		LineRenderer lr = trail.GetComponent<LineRenderer> ();
		if (lr != null) 
		{
			lr.SetPosition (0, firePoint.position);
			lr.SetPosition (1, hitPos);
		}
		Destroy (trail.gameObject, 0.04f);

		if (hitNormal != new Vector3 (9999f, 9999f, 9999f)) 
		{
			Transform hitParticle = (Transform)Instantiate (hitPrefab, hitPos, Quaternion.FromToRotation (Vector3.right, hitNormal));
			Destroy (hitParticle.gameObject, 1f);
		}

		Transform clone = (Transform)Instantiate (muzzleFlashPrefab, firePoint.position, firePoint.rotation);
		clone.parent = firePoint;
		float size = Random.Range (minMFPSize, maxMFPSize);
		clone.localScale = new Vector3 (size, size, size);
		Destroy (clone.gameObject, 0.02f);
	}
}
using UnityEngine;
using System.Collections;

public class WeaponCheck : MonoBehaviour 
{
    [SerializeField] private float critChance;
    [SerializeField] private float knockPow;

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Enemy")
        {
            float randy = Random.Range(0f, 1f);
            other.gameObject.GetComponent<Enemy>().DamageEnemy(20f, -other.contacts[0].normal, knockPow, (randy < critChance));
        }
	}
}
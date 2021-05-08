using UnityEngine;
using System.Collections;

public class KnockbackManager : MonoBehaviour 
{
	private static KnockbackManager instance;

	public static KnockbackManager Instance
	{
		get
		{
			if (instance == null) instance = GameObject.FindObjectOfType<KnockbackManager> ();

			return KnockbackManager.instance;
		}
	}

	public void ApplyKnockback(Rigidbody2D rb, float knockback, Vector2 hitNormal)
	{
		rb.AddForce(hitNormal * knockback);
	}
}
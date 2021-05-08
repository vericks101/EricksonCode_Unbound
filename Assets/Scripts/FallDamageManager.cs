using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class FallDamageManager : MonoBehaviour 
{
	private float lastPositionY = 0f;
	private float fallDistance = 0f;
	private float fallDamageScale = 0.5f;
	private Transform player;

	private void Start()
	{
		player = transform;
	}

	private void Update()
	{
		if (lastPositionY > player.transform.position.y) 
			fallDistance += lastPositionY - player.transform.position.y;
		lastPositionY = player.transform.position.y;

		if (fallDistance >= 10f && GetComponent<PlatformerCharacter2D>().m_Grounded) 
		{
			var damage = (int)(fallDamageScale * fallDistance);
			player.GetComponent<Player> ().DamagePlayer (damage, 0f, new Vector2(0f, 0f), 0.1f, 0.1f);

			ResetFall ();
		}
		if (fallDistance <= 10f && GetComponent<PlatformerCharacter2D>().m_Grounded) 
			ResetFall ();
	}

	private void ResetFall()
	{
		fallDistance = 0f;
		lastPositionY = 0f;
	}
}
using UnityEngine;
using System.Collections;

public class MoveTrail : MonoBehaviour 
{
	public float moveSpeed = 0f;

	void Update()
	{
		transform.Translate (Vector3.right * Time.deltaTime * moveSpeed);
		Destroy (gameObject, 1f);
	}
}
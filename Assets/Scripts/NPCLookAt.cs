using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLookAt : MonoBehaviour
{
    private Transform playerPosition;
    private bool facingRight;

    private void Start()
    {
        playerPosition = Player.Instance.transform;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        if (playerPosition.position.x < transform.position.x)
            facingRight = true;
    }

    private void Update()
    {
        if (playerPosition.position.x < transform.position.x && facingRight)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingRight = false;
        }
        else if (playerPosition.position.x > transform.position.x && !facingRight)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            facingRight = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAbove : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float yOffset;

    private void Start()
    {
        if (Player.Instance != null)
            player = Player.Instance.transform;
    }

    private void Update()
    {
        if (player == null)
        {
            if (Player.Instance != null)
                player = Player.Instance.transform;
        }
        else if (player != null)
        {
            if (player.position.y >= transform.position.y - yOffset)
                GetComponent<BoxCollider2D>().enabled = false;
            else if (player.position.y < transform.position.y)
                GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}

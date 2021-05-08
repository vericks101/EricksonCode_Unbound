using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        if (Player.Instance != null)
            player = Player.Instance.transform;
    }

    private void Update()
    {
        if (player == null && Player.Instance != null)
                player = Player.Instance.transform;
        else if (player != null)
            transform.position = new Vector3(player.position.x, player.position.y, player.position.z);
    }
}

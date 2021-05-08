using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDelete : MonoBehaviour
{
    [SerializeField] private float deleteDistance;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, Player.Instance.transform.position);
        if (distance > deleteDistance)
            Destroy(gameObject);
    }
}

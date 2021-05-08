using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayPickup : MonoBehaviour
{
    [SerializeField] private float delayTime;
    private float delayTimer;
    public bool canPickup;

    private void Start()
    {
        delayTimer = Time.time + delayTime;
        canPickup = false;
    }

    private void Update()
    {
        if (!canPickup)
        {
            if (Time.time >= delayTimer)
                canPickup = true;
        }
    }
}

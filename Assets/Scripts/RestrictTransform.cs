using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictTransform : MonoBehaviour
{
    [SerializeField] private bool restrictX;
    [SerializeField] private bool restrictY;
    [SerializeField] private bool restrictZ;
    private float initX;
    private float initY;
    private float initZ;

    private void Start()
    {
        if (SpawnpointManager.Instance != null)
        {
            initX = SpawnpointManager.Instance.transform.position.x;
            initY = SpawnpointManager.Instance.transform.position.y;
            initZ = SpawnpointManager.Instance.transform.position.z;
        }
    }

    public void UpdateInit()
    {
        if (SpawnpointManager.Instance != null)
        {
            initX = SpawnpointManager.Instance.transform.position.x;
            initY = SpawnpointManager.Instance.transform.position.y;
            initZ = SpawnpointManager.Instance.transform.position.z;
        }
    }

    private void Update()
    {
        if (restrictX)
            transform.position = new Vector3(initX, transform.position.y, transform.position.z);
        if (restrictY)
            transform.position = new Vector3(transform.position.x, initY, transform.position.z);
        if (restrictZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, initZ);
    }
}
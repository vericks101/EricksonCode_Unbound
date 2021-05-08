using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainLoader : MonoBehaviour
{
    public void Load()
    {
        GameObject obj = ObjectPooler.current.GetPooledObject();
        if (obj == null)
            return;

        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.SetActive(true);
    }
}
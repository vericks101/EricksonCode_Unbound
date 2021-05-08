using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainUnloader : MonoBehaviour
{
    private const string objDefName = "sprite_basic";
    private const string defTagName = "Untagged";

    public void Unload(GameObject go)
    {
        if (go != null)
        {
            //GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().toBeDeleted.Remove(go.name);
            GameManager.Instance.terrainMap.GetComponent<TerrainRenderer>().RenderedChunks.Remove(go.name);
            ObjectPooler.current.pooledObjects.Remove(go);

            if (TerrainLighting.lightObjects.ContainsKey(go.name))
                TerrainLighting.lightObjects.Remove(go.name);

            go.name = objDefName;
            go.GetComponent<SpriteManager>().Health = 100;
            go.GetComponent<SpriteManager>().ItemID = 0;
            go.GetComponent<SpriteManager>().IsLit = false;
            go.GetComponent<SpriteManager>().LayerID = 0;
            go.GetComponent<SpriteRenderer>().tag = defTagName;
            go.GetComponent<SpriteRenderer>().sortingLayerID = 0;
            go.GetComponent<SpriteRenderer>().sortingOrder = 0;
            go.GetComponent<SpriteManager>().CanGrowOn = false;
            go.GetComponent<SpriteManager>().CanSpread = false;
            go.layer = 0;
            go.GetComponent<SpriteRenderer>().color = Color.white;

            Destruct(go);
            //go.SetActive(false);
            Destroy(go);
        }
    }

    void Destruct(GameObject obj)
    {
        List<GameObject> toBeDestroyed = new List<GameObject>();
        foreach (Transform child in obj.transform)
        {
            if (child.GetComponent<TerrainLighting>() != null)
                toBeDestroyed.Add(child.gameObject);
        }
        foreach(var go in toBeDestroyed)
        {
            Destroy(go);
        }
    }
}
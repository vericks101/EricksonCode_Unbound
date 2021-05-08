using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Mode { SPOTLIGHT, AREA }

public class TerrainLighting : MonoBehaviour 
{
	[SerializeField] public Mode mode;

	[SerializeField] private LayerMask layerMask;
	private List<GameObject> collidedSprites = new List<GameObject> ();
    public static Dictionary<string, GameObject> lightObjects = new Dictionary<string, GameObject>();

    public float tintScale;
    public float maxTint;

    public int lightingDistance;
    public float lightFalloffScale;

    void ResetSpriteTint(GameObject go)
	{
        if (go.tag == "PlayerMasked")
            go.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
        else
            go.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, go.GetComponent<SpriteRenderer>().color.a);
        if (go.GetComponent<SpriteManager>() != null)
            go.GetComponent<SpriteManager>().isTorched = false;
        if (go.GetComponent<ItemScript>() != null)
            go.GetComponent<ItemScript>().isTorched = false;
	}

	public void Destruct()
	{
        if (mode == Mode.AREA)
            Player.Instance.isTorched = false;

        foreach (GameObject go in collidedSprites) 
		{
			if (go != null && go.GetComponent<SpriteRenderer>() != null) 
				ResetSpriteTint (go);
		}
        lightObjects.Remove(gameObject.name);
        //GameManager.Instance.GetComponent<TODManager>().UpdateLighting(GameManager.Instance.GetComponent<TODManager>().currentLightTint);
	}

    public void UpdateSurroundingTiles(float tintScale, int distanceCount, GameObject tileObj)
    {
        if(tileObj.GetComponentInParent<SpriteManager>() != null)
            tileObj.GetComponentInParent<SpriteRenderer>().color = new Color(0f, 0f, 0f, tileObj.GetComponentInParent<SpriteRenderer>().color.a);

        if (mode == Mode.SPOTLIGHT)
        {
            if (GameManager.Instance.terrainMap.GetComponent<TerrainEditor>().CurrentBackMap[(int)tileObj.transform.position.x, (int)tileObj.transform.position.y] == 0)
            {
                if (tileObj.GetComponentInParent<SpriteManager>() != null)
                    tileObj.GetComponentInParent<SpriteRenderer>().color = new Color(tileObj.GetComponentInParent<SpriteRenderer>().color.r + GameManager.Instance.GetComponent<TODManager>().currentLightTint,
                         tileObj.GetComponentInParent<SpriteRenderer>().color.g + GameManager.Instance.GetComponent<TODManager>().currentLightTint,
                          tileObj.GetComponentInParent<SpriteRenderer>().color.b + GameManager.Instance.GetComponent<TODManager>().currentLightTint, tileObj.GetComponentInParent<SpriteRenderer>().color.a);
            }
            else
            {
                if (tileObj.GetComponentInParent<SpriteManager>() != null)
                    tileObj.GetComponentInParent<SpriteRenderer>().color = new Color(tileObj.GetComponentInParent<SpriteRenderer>().color.r + tintScale,
                         tileObj.GetComponentInParent<SpriteRenderer>().color.g + tintScale, tileObj.GetComponentInParent<SpriteRenderer>().color.b + tintScale, tileObj.GetComponentInParent<SpriteRenderer>().color.a);
            }

            Collider2D hit = Physics2D.OverlapPoint(new Vector2(tileObj.transform.position.x, tileObj.transform.position.y - 1));
            if (distanceCount >= 0 && hit != null)
            {
                if (hit.GetComponent<SpriteRenderer>() != null && hit.GetComponent<Player>() == null)
                    hit.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, hit.GetComponent<SpriteRenderer>().color.a);

                if (hit.GetComponent<SpriteRenderer>() != null && hit.GetComponent<Player>() == null && hit.GetComponent<SpriteManager>() == null)
                {
                    if (GameManager.Instance.terrainMap.GetComponent<TerrainEditor>().CurrentBackMap[(int)tileObj.transform.position.x, (int)tileObj.transform.position.y] == 0)
                        hit.GetComponent<SpriteRenderer>().color = new Color(hit.GetComponent<SpriteRenderer>().color.r + GameManager.Instance.GetComponent<TODManager>().currentLightTint,
                            hit.GetComponent<SpriteRenderer>().color.g + GameManager.Instance.GetComponent<TODManager>().currentLightTint,
                            hit.GetComponent<SpriteRenderer>().color.b + GameManager.Instance.GetComponent<TODManager>().currentLightTint, hit.GetComponent<SpriteRenderer>().color.a);
                    else
                        hit.GetComponent<SpriteRenderer>().color = new Color(hit.GetComponent<SpriteRenderer>().color.r + tintScale, hit.GetComponent<SpriteRenderer>().color.g + tintScale,
                            hit.GetComponent<SpriteRenderer>().color.b + tintScale, hit.GetComponent<SpriteRenderer>().color.a);
                }

                UpdateSurroundingTiles(Mathf.Clamp(tintScale - lightFalloffScale, 0f, 1f), distanceCount - 1, hit.gameObject);
            }
        }
        else if (mode == Mode.AREA)
        {
            if (tileObj.GetComponentInParent<SpriteManager>() != null)
                tileObj.GetComponentInParent<SpriteRenderer>().color = new Color(1f, 1f, 1f, tileObj.GetComponentInParent<SpriteRenderer>().color.a);
            Collider2D[] hits = Physics2D.OverlapCircleAll(tileObj.transform.position, distanceCount);
            for (int i = 0; i < hits.Length; i++)
            {
                if (!collidedSprites.Contains(hits[i].gameObject))
                    collidedSprites.Add(hits[i].gameObject);

                float objTint = Mathf.Clamp(1f - (lightFalloffScale * (Mathf.Abs(Vector3.Distance(tileObj.transform.position, hits[i].transform.position)))), 0f, 1f);
                if (hits[i].GetComponent<SpriteManager>() != null)
                    hits[i].GetComponent<SpriteRenderer>().color = new Color(hits[i].GetComponent<SpriteRenderer>().color.r + objTint, hits[i].GetComponent<SpriteRenderer>().color.g + objTint,
                        hits[i].GetComponent<SpriteRenderer>().color.b + objTint, hits[i].GetComponent<SpriteRenderer>().color.a);
                if (hits[i].GetComponent<Player>() != null)
                {
                    for (int j = 0; j < Player.Instance.playerLightableObjects.Length; j++)
                    {
                        if (Player.Instance.playerLightableObjects[j].tag == "PlayerMasked")
                            Player.Instance.playerLightableObjects[j].GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f - objTint);
                        else
                            Player.Instance.playerLightableObjects[j].GetComponent<SpriteRenderer>().color = new Color(objTint, objTint, objTint, Player.Instance.playerLightableObjects[j].GetComponent<SpriteRenderer>().color.a);
                    }
                }

                if (hits[i].GetComponent<SpriteRenderer>() != null && hits[i].GetComponent<Player>() == null && hits[i].GetComponent<SpriteManager>() == null)
                    hits[i].GetComponent<SpriteRenderer>().color = new Color(hits[i].GetComponent<SpriteRenderer>().color.r + objTint, hits[i].GetComponent<SpriteRenderer>().color.g + objTint,
                        hits[i].GetComponent<SpriteRenderer>().color.b + objTint, hits[i].GetComponent<SpriteRenderer>().color.a);
            }
        }
    }
}
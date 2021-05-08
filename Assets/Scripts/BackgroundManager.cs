using UnityEngine;
using System.Collections;

[System.Serializable]
public class BackgroundElement
{
    public Sprite backgroundSprite;
    public Vector3 position;
    public bool noLighting;
    public bool noTiling;
    public Transform parentTransform;
}

public class BackgroundManager : MonoBehaviour
{
    public BackgroundElement[] backgroundSprites;
    [SerializeField] private int oilStartNum;
    [SerializeField] private int nonTileStartNum;

    void Start()
    {
        //GameManager.Instance.GetComponent<TODManager>().currentBackgrounds = new SpriteRenderer[backgroundSprites.Length];
        ReplaceBackgroundsRecursive(Camera.main.transform.parent.gameObject);
        for (int i = 0; i < backgroundSprites.Length; i++)
        {
            GameObject tmp = new GameObject();
            
            tmp.AddComponent<SpriteRenderer>();
            if (!backgroundSprites[i].noTiling)
            {
                tmp.AddComponent<Tiling>();
                tmp.GetComponent<Tiling>().reverseScale = true;
            }
            tmp.GetComponent<SpriteRenderer>().sprite = backgroundSprites[i].backgroundSprite;
            if (!backgroundSprites[i].noTiling)
                tmp.GetComponent<SpriteRenderer>().sortingOrder = oilStartNum;
            else
                tmp.GetComponent<SpriteRenderer>().sortingOrder = nonTileStartNum;

            tmp.tag = "Background";
            tmp.name = "Background" + i;
            if (backgroundSprites[i].noTiling)
                tmp.transform.parent = Camera.main.transform.parent;
            else
                tmp.transform.parent = backgroundSprites[i].parentTransform;
            tmp.transform.localPosition = new Vector3(backgroundSprites[i].position.x, backgroundSprites[i].position.y, backgroundSprites[i].position.z);

            if (!backgroundSprites[i].noLighting)
                GameManager.Instance.GetComponent<TODManager>().currentBackgrounds.Add(tmp.GetComponent<SpriteRenderer>());

            oilStartNum--;
        }
    }

    void ReplaceBackgroundsRecursive(GameObject go)
    {
        if (go.tag == "Background")
            Destroy(go);
        foreach (Transform child in go.transform)
        {
            if (child.tag == "Background")
            {
                ReplaceBackgroundsRecursive(child.gameObject);
            }
        }
    }
}

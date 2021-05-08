using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SpriteRenderer))]
public class Tiling : MonoBehaviour
{
    public int offsetX = 2;

    public bool hasARightBuddy = false;
    public bool hasALeftBuddy = false;

    public bool reverseScale = false;

    private float spriteWidth = 0f;
    private Camera cam;
    private Transform myTransform;

    void Awake()
    {
        cam = Camera.main;
        myTransform = transform;
    }

    void Start()
    {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = sRenderer.sprite.bounds.size.x;
    }

    void Update()
    {
        if (!hasALeftBuddy || !hasARightBuddy)
        {
            float camHorizontalExtent = cam.orthographicSize * Screen.width / Screen.height;

            float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtent;
            float edgeVisisblePositionLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtent;

            if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasARightBuddy)
            {
                MakeNewBuddy(1);
                hasARightBuddy = true;
            }
            else if (cam.transform.position.x <= edgeVisisblePositionLeft + offsetX && !hasALeftBuddy)
            {
                MakeNewBuddy(-1);
                hasALeftBuddy = true;
            }
        }
    }

    void MakeNewBuddy(int rightOrLeft)
    {
        if (GameManager.Instance != null)
        {
            Vector3 newPosition = new Vector3(myTransform.position.x + spriteWidth * rightOrLeft, myTransform.position.y, myTransform.position.z);
            Transform newBuddy = (Transform)Instantiate(myTransform, newPosition, myTransform.rotation);
            GameManager.Instance.GetComponent<TODManager>().currentBackgrounds.Add(newBuddy.GetComponent<SpriteRenderer>());

            if (reverseScale)
            {
                newBuddy.localScale = new Vector3(newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
            }

            newBuddy.parent = myTransform.parent;
            if (rightOrLeft > 0)
            {
                newBuddy.GetComponent<Tiling>().hasALeftBuddy = true;
            }
            else
            {
                newBuddy.GetComponent<Tiling>().hasARightBuddy = true;
            }
        }
    }
}

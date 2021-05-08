using UnityEngine;

public class Bed : MonoBehaviour
{
    public float timeScale;
    private float normalScale;
    private GameObject playerObj;
    public bool inBed = false;

    private void Start()
    {
        normalScale = GameManager.Instance.GetComponent<TODManager>().TimeScale;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
            playerObj = collider.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player")
            playerObj = null;
        if (inBed)
            GameManager.Instance.GetComponent<TODManager>().TimeScale = normalScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerObj != null && !inBed)
            GetIn();
        else if ((Input.GetKeyDown(KeyCode.E) && playerObj != null) || (Input.GetKeyDown(KeyCode.E) && inBed))
            GetOut();
    }

    private void GetIn()
    {
        normalScale = GameManager.Instance.GetComponent<TODManager>().TimeScale;
        GameManager.Instance.GetComponent<TODManager>().TimeScale = timeScale;
        Player.Instance.inBed = true;
        inBed = true;

        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer.enabled = false;
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer.enabled = false;
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer.enabled = false;
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer.enabled = false;
        }

        if (!GameManager.Instance.invSelect.activeInHierarchy && Player.Instance.shield.activeInHierarchy)
            GameManager.Instance.SwitchSelect(true);
        else if (!GameManager.Instance.invSelect.activeInHierarchy)
            GameManager.Instance.SwitchSelect(false);

        Player.Instance.GetComponent<Rigidbody2D>().drag = 100f;
        Player.Instance.GetComponent<Rigidbody2D>().mass = 100f;

        SpawnpointManager.Instance.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
    }

    private void GetOut()
    {
        GameManager.Instance.GetComponent<TODManager>().TimeScale = normalScale;
        Player.Instance.inBed = false;
        inBed = false;

        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer.enabled = true;
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer.enabled = true;
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer.enabled = true;
        }
        for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites.Length; i++)
        {
            if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer != null)
                GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer.enabled = true;
        }

        Player.Instance.GetComponent<Rigidbody2D>().drag = 0f;
        Player.Instance.GetComponent<Rigidbody2D>().mass = 1f;
    }
}
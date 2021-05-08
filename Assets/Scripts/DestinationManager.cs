using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Galaxy
{
    public GameObject galaxyObj;
    public int questRequirement;
    public string name;
    public string galaxyThreat;
}

public class DestinationManager : MonoBehaviour
{
    private static int curDestinationIndex;

    private float fadeTime = 0f;
    private bool canFade = false;

    public GameObject destinationHolder;
    public GameObject galaxyHolder;
    public Text galaxyText;
    [SerializeField] private Text galaxyThreat;
    public Galaxy[] galaxies;
    private int curIndex;

    public static DestinationManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
                Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void OnGalaxyChange(int index)
    {
        if (gameObject.GetComponent<CanvasGroup>().alpha <= 0f)
        {
            for (int i = 1; i < galaxies.Length; i++)
            {
                galaxies[i].galaxyObj.GetComponent<Animator>().Play("GalaxyFadeOut", 0, 1f);
            }
        }

        if (destinationHolder.GetComponent<CanvasGroup>().alpha <= 0f)
            destinationHolder.GetComponent<Animator>().SetTrigger("FadeIn");
        destinationHolder.GetComponent<CanvasGroup>().interactable = true;
        destinationHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;
        //solarSystemObj.GetComponent<Animator>().SetTrigger("FadeOut");
        //solarSystemObj.GetComponent<CanvasGroup>().alpha = 0f;
        //solarSystemObj.GetComponent<CanvasGroup>().interactable = false;
        //solarSystemObj.GetComponent<CanvasGroup>().blocksRaycasts = false;

        if (galaxyHolder.GetComponent<CanvasGroup>().alpha >= 1f)
            galaxyHolder.GetComponent<Animator>().SetTrigger("FadeOut");
        galaxyHolder.GetComponent<CanvasGroup>().interactable = false;
        galaxyHolder.GetComponent<CanvasGroup>().blocksRaycasts = false;

        curIndex = index;
        galaxyText.text = galaxies[curIndex].name;
        galaxyThreat.text = galaxies[curIndex].galaxyThreat;
        if (galaxies[curIndex].galaxyObj.activeInHierarchy)
            galaxies[curIndex].galaxyObj.GetComponent<Animator>().SetTrigger("FadeIn");
        //galaxies[index].GetComponent<CanvasGroup>().alpha = 1f;
        //galaxies[index].GetComponent<CanvasGroup>().interactable = true;
        //galaxies[index].GetComponent<CanvasGroup>().blocksRaycasts = true;

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    public void OnDestinationChange(int index)
    {
        if (CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent == null && index != UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex)
        {
            curDestinationIndex = index;
            if (!GameManager.Instance.invSelect.activeInHierarchy)
            {
                if (Player.Instance.shield.activeInHierarchy && !GameManager.Instance.armItems[0].item.GetComponent<SpriteRenderer>().enabled)
                    GameManager.Instance.SwitchSelect(true);
                else
                    GameManager.Instance.SwitchSelect(false);
            }

            CodecManager.Instance.SaveInventories();
            CodecManager.Instance.SaveTerrain();
            CodecManager.Instance.SavePlayerData();
            CodecManager.Instance.SavePreviewData();
            CodecManager.Instance.SaveCharacterTreeData();
            CodecManager.Instance.SaveMenuData();
            CodecManager.Instance.SaveQuestData();

            AudioManager.instance.PlaySound("Teleport0");

            Player.Instance.GetComponent<FallDamageManager>().enabled = false;

            StartCoroutine(CurrentSceneManager.Instance.FadeOut());
            fadeTime = GameManager.Instance.GetComponent<FadingManager>().BeginFade(1) + Time.time;
            canFade = true;

            if (Player.Instance.chest != null && Player.Instance.chest.IsOpen)
                Player.Instance.chest.Open(false);
            Player.Instance.chest = null;
        }
        else
            AudioManager.instance.PlaySound("Error");
    }

    void OnLevelWasLoaded()
    {
        if (gameObject.GetComponent<CanvasGroup>().alpha >= 1f)
            GetComponent<Animator>().SetTrigger("FadeOut");
        //gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        gameObject.GetComponent<CanvasGroup>().interactable = false;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

        //solarSystemObj.GetComponent<Animator>().SetTrigger("FadeIn");
        //solarSystemObj.GetComponent<CanvasGroup>().alpha = 1f;
        //solarSystemObj.GetComponent<CanvasGroup>().interactable = true;
        //solarSystemObj.GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (galaxies[curIndex].galaxyObj.GetComponent<CanvasGroup>().alpha >= 1f)
            galaxies[curIndex].galaxyObj.GetComponent<Animator>().SetTrigger("FadeOut");
        //galaxies[curIndex].GetComponent<CanvasGroup>().alpha = 0f;
        //galaxies[curIndex].GetComponent<CanvasGroup>().interactable = false;
        //galaxies[curIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;

        for (int i = 1; i < galaxies.Length; i++)
        {
            galaxies[i].galaxyObj.GetComponent<CanvasGroup>().alpha = 0f;
            galaxies[i].galaxyObj.GetComponent<CanvasGroup>().interactable = false;
            galaxies[i].galaxyObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    void Update()
    {
        if (Time.time > fadeTime && canFade)
        {
            LoadingScreenManager.LoadScene(curDestinationIndex);
            canFade = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && gameObject.GetComponent<CanvasGroup>().interactable == true)
        {
            GetComponent<Animator>().SetTrigger("FadeOut");
            gameObject.GetComponent<CanvasGroup>().alpha = 0f;
            gameObject.GetComponent<CanvasGroup>().interactable = false;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

            //solarSystemObj.GetComponent<Animator>().SetTrigger("FadeIn");
            //solarSystemObj.GetComponent<CanvasGroup>().alpha = 1f;
            //solarSystemObj.GetComponent<CanvasGroup>().interactable = true;
            //solarSystemObj.GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (galaxies[curIndex].galaxyObj.activeInHierarchy)
                galaxies[curIndex].galaxyObj.GetComponent<Animator>().SetTrigger("FadeOut");
            //galaxies[curIndex].GetComponent<CanvasGroup>().alpha = 0f;
            //galaxies[curIndex].GetComponent<CanvasGroup>().interactable = false;
            //galaxies[curIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void UpdateAvailableSystems()
    {
        for (int i = 0; i < galaxies.Length; i++)
        {
            galaxies[i].galaxyObj.SetActive(false);

            if (QuestManager.Instance.questCompleted[galaxies[i].questRequirement])
                galaxies[i].galaxyObj.SetActive(true);
        }
    }

    public void OnCloseButtonPressed()
    {
        if (gameObject.GetComponent<CanvasGroup>().interactable == true)
        {
            GetComponent<Animator>().SetTrigger("FadeOut");
            gameObject.GetComponent<CanvasGroup>().alpha = 0f;
            gameObject.GetComponent<CanvasGroup>().interactable = false;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

            //solarSystemObj.GetComponent<Animator>().SetTrigger("FadeIn");
            //solarSystemObj.GetComponent<CanvasGroup>().alpha = 1f;
            //solarSystemObj.GetComponent<CanvasGroup>().interactable = true;
            //solarSystemObj.GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (galaxies[curIndex].galaxyObj.activeInHierarchy)
                galaxies[curIndex].galaxyObj.GetComponent<Animator>().SetTrigger("FadeOut");
            //galaxies[curIndex].GetComponent<CanvasGroup>().alpha = 0f;
            //galaxies[curIndex].GetComponent<CanvasGroup>().interactable = false;
            //galaxies[curIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        AudioManager.instance.PlaySound("Select_Pickup");
    }
}

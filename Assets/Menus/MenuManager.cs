using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets._2D;

[System.Serializable]
public class Tab
{
	public string name;
	public GameObject tab;
}

public class MenuManager : MonoBehaviour 
{
	public GameObject menu;

    public GameObject characterTreeObj;
    public Button characterTreeButton;

	[SerializeField] private Tab[] tabs;
	private int selected;
    [SerializeField] private Button mainMenuButton;

    private float fadeTime = 0f;
    private bool canFade = false;

    public GameObject aiObject;
    public GameObject playerReturnButton;
    public GameObject aiMenuUI;

    public int overallIncome;
    public int enemyKills;
    public int overallDeaths;

    [SerializeField] private CanvasGroup mainMenuConfirmUI;

    private static MenuManager instance;

    public static MenuManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<MenuManager>();

            return instance;
        }
    }

    void OnEnable()
    {
        mainMenuButton.onClick.AddListener(delegate { OnMainMenuPress(); });
        characterTreeButton.onClick.AddListener(delegate { OnCharacterTreePress(); });

        aiObject = GameObject.Find("AIHolder");
        aiObject.SetActive(false);

        playerReturnButton = GameObject.Find("PlayerNavigationHolder");
        //aiMenuUI.SetActive(false);
        if (aiMenuUI.transform.parent.GetComponent<CanvasGroup>().alpha >= 1f)
            aiMenuUI.transform.parent.GetComponent<Animator>().SetTrigger("Close");
        aiMenuUI.transform.parent.GetComponent<CanvasGroup>().interactable = false;
        aiMenuUI.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void OnLevelWasLoaded()
    {
        if (aiObject != null)
            aiObject.SetActive(false);
        //aiMenuUI.SetActive(false);
        if (aiMenuUI.transform.parent.GetComponent<CanvasGroup>().alpha >= 1f)
            aiMenuUI.transform.parent.GetComponent<Animator>().SetTrigger("Close");
        aiMenuUI.transform.parent.GetComponent<CanvasGroup>().interactable = false;
        aiMenuUI.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameManager.canUseWeapons = true;
    }

    void Update()
	{
        if (!aiObject.activeInHierarchy && aiMenuUI.transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MenuCanvas"))
            aiObject.SetActive(true);
        else if (aiObject.activeInHierarchy && aiMenuUI.transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MenuCanvasClose"))
            aiObject.SetActive(false);

        if (Input.GetKeyDown(KeyCode.LeftControl) && !Player.Instance.isDead)
        {
            if (aiMenuUI.transform.parent.GetComponent<CanvasGroup>().alpha == 0f)
            {
                DestinationManager.Instance.UpdateAvailableSystems();
                aiMenuUI.transform.parent.GetComponent<Animator>().SetTrigger("Open");
                aiMenuUI.transform.parent.GetComponent<CanvasGroup>().interactable = true;
                aiMenuUI.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;

                GameManager.canUseWeapons = false;
            }
            else if (aiMenuUI.transform.parent.GetComponent<CanvasGroup>().alpha == 1f)
            {
                aiMenuUI.transform.parent.GetComponent<Animator>().SetTrigger("Close");
                aiMenuUI.transform.parent.GetComponent<CanvasGroup>().interactable = false;
                aiMenuUI.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;

                GameManager.canUseWeapons = true;
            }

            if (!aiObject.activeInHierarchy && aiMenuUI.transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                aiObject.SetActive(true);
                //aiMenuUI.SetActive(true);
            }
            else if (aiMenuUI.transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                aiObject.SetActive(false);
                //aiMenuUI.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (ChatManager.Instance.chatBoxActive)
                ChatManager.Instance.chatBoxActive = false;
            else
            {
                if (GetComponent<CanvasGroup>().alpha == 0f)
                {
                    GetComponent<Animator>().SetTrigger("Open");
                    ChangeActiveTab(1);
                    GameManager.canUseWeapons = false;
                }
                else if (GetComponent<CanvasGroup>().alpha == 1f)
                {
                    GetComponent<Animator>().SetTrigger("Close");
                    tabs[selected].tab.GetComponent<Animator>().SetTrigger("Close");
                    GameManager.canUseWeapons = true;
                }

                ChangeActiveMenu();

                QuestUIManager.Instance.infoTitle.text = "";
                QuestUIManager.Instance.infoObjective.text = "";
                QuestUIManager.Instance.infoRewards.text = "";
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ChatManager.Instance.chatBoxActive)
            {
                ChatManager.Instance.chatBoxActive = false;
                ChatManager.Instance.DeactivateChatBox();
            }
            else
            {
                if (GetComponent<CanvasGroup>().alpha == 0f)
                {
                    GetComponent<Animator>().SetTrigger("Open");
                    ChangeActiveTab(0);
                    GameManager.canUseWeapons = false;
                }
                else if (GetComponent<CanvasGroup>().alpha == 1f)
                {
                    GetComponent<Animator>().SetTrigger("Close");
                    tabs[selected].tab.GetComponent<Animator>().SetTrigger("Close");
                    GameManager.canUseWeapons = true;
                }

                ChangeActiveMenu();

                QuestUIManager.Instance.infoTitle.text = "";
                QuestUIManager.Instance.infoObjective.text = "";
                QuestUIManager.Instance.infoRewards.text = "";
            }
        }

        if (Time.time > fadeTime && canFade)
        {
            LoadingScreenManager.LoadScene(0);
            canFade = false;

            Destroy(CanvasManagement.Instance.gameObject);
            Destroy(GameManager.Instance.gameObject);
            Destroy(Player.Instance.gameObject);
            Destroy(DialogueManager.Instance.gameObject);
            Destroy(QuestManager.Instance.gameObject);
            Destroy(SettingsManager.Instance.transform.parent.gameObject);
        }
    }

	void ChangeActiveMenu()
	{
        Player.Instance.charPanel.Open(true);

        if (menu.GetComponent<CanvasGroup> ().alpha <= 0f) 
		{
			menu.GetComponent<CanvasGroup> ().alpha = 1f;
			menu.GetComponent<CanvasGroup> ().blocksRaycasts = true;

            QuestUIManager.Instance.infoTitle.text = "";
            QuestUIManager.Instance.infoObjective.text = "";
            QuestUIManager.Instance.infoRewards.text = "";
        } 
		else if (menu.GetComponent<CanvasGroup> ().alpha >= 1f) 
		{
			menu.GetComponent<CanvasGroup> ().alpha = 0f;
			menu.GetComponent<CanvasGroup> ().blocksRaycasts = false;
            tabs[0].tab.GetComponent<CanvasGroup>().alpha = 0f;
            tabs[0].tab.GetComponent<CanvasGroup>().interactable = false;

            QuestUIManager.Instance.infoTitle.text = "";
            QuestUIManager.Instance.infoObjective.text = "";
            QuestUIManager.Instance.infoRewards.text = "";
        }

        AudioManager.instance.PlaySound("Select_Pickup");
    }

	public void ChangeActiveTab(int selectedIndex)
	{
        if (characterTreeObj.GetComponent<CanvasGroup>().alpha == 1f)
        {
            characterTreeObj.GetComponent<CanvasGroup>().alpha = 0f;
            characterTreeObj.GetComponent<CanvasGroup>().interactable = false;
            characterTreeObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        if (tabs[selected].tab.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MenuCanvas"))
            tabs[selected].tab.GetComponent<Animator>().SetTrigger("Close");
        tabs[selected].tab.GetComponent<CanvasGroup>().interactable = false;
        tabs[selected].tab.GetComponent<CanvasGroup>().blocksRaycasts = false;

        selected = selectedIndex;
		if (menu.activeInHierarchy) 
		{
            tabs[selected].tab.GetComponent<Animator>().SetTrigger("Open");
            tabs[selected].tab.GetComponent<CanvasGroup>().interactable = true;
            tabs[selected].tab.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    public void OnMainMenuPressYes()
    {
        AudioManager.instance.currentTrack.Stop();
        AudioManager.instance.currentTrack = null;

        CodecManager.Instance.SaveInventories();
        CodecManager.Instance.SavePlayerData();
        CodecManager.Instance.SavePreviewData();
        CodecManager.Instance.SaveCharacterTreeData();
        CodecManager.Instance.SaveMenuData();
        CodecManager.Instance.SaveQuestData();
        CodecManager.Instance.SaveTerrain();

        AudioManager.instance.PlaySound("Select_Pickup");

        fadeTime = GameManager.Instance.GetComponent<FadingManager>().BeginFade(1) + Time.time;
        canFade = true;

        mainMenuConfirmUI.alpha = 0f;
        mainMenuConfirmUI.interactable = false;
        mainMenuConfirmUI.blocksRaycasts = false;
    }

    public void OnMainMenuPressNo()
    {
        AudioManager.instance.PlaySound("Select_Pickup");

        mainMenuConfirmUI.alpha = 0f;
        mainMenuConfirmUI.interactable = false;
        mainMenuConfirmUI.blocksRaycasts = false;
    }

    void OnMainMenuPress()
    {
        AudioManager.instance.PlaySound("Select_Pickup");

        mainMenuConfirmUI.alpha = 1f;
        mainMenuConfirmUI.interactable = true;
        mainMenuConfirmUI.blocksRaycasts = true;
    }

    void OnCharacterTreePress()
    {
        characterTreeObj.GetComponent<CanvasGroup>().alpha = 1f;
        characterTreeObj.GetComponent<CanvasGroup>().interactable = true;
        characterTreeObj.GetComponent<CanvasGroup>().blocksRaycasts = true;

        tabs[selected].tab.GetComponent<Animator>().SetTrigger("Close");
        tabs[selected].tab.GetComponent<CanvasGroup>().interactable = false;
        tabs[selected].tab.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
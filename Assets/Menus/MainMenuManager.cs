using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    public Slider hairSlider;
    public Slider headSlider;
    public Slider bodySlider;
    public Slider feetSlider;

    public GameObject menuParentObj;
    public GameObject mainCanvas;
    public GameObject menuCanvas;
    public GameObject characterCanvas;
    public GameObject settingsCanvas;
    public GameObject creditsCanvas;
    public GameObject tutorialCanvas;

    public Button mainPlayButton;
    public Button menuSingleButton;
    public Button menuSettingsButton;
    public Button menuCreditsButton;
    public Button menuExitButton;

    public Button charBackButton;
    public GameObject charCreate;
    public GameObject charHolder;
    public GameObject charBack;
    public GameObject charDelete;
    public Button deleteConfirm;
    public Button deleteDeny;

    public Button settingsBack;
    public Button creditsBack;

    public Button charPlayButton;
    public GameObject curSelectedProfile;

    public GameObject[] charSlots;

    public Image hairImage;
    public Image headImage;
    public Image bodyImage;
    public Image feetImage;

    private float fadeTime = 0f;
    private bool canFade = false;

    public Text characterNameText;

    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;

    public GameObject charImage;

    public static bool characterCreated = false;

    public Toggle developmentToggle;
    [SerializeField] private CanvasGroup developmentModeText;
    public static bool developmentMode = true;

    private static MainMenuManager instance;

    public static MainMenuManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<MainMenuManager>();

            return instance;
        }
    }

    void OnEnable()
    {
        mainPlayButton.onClick.AddListener(delegate { OnMainPlayPress(); });
        menuSingleButton.onClick.AddListener(delegate { OnSinglePress(); });
        menuSettingsButton.onClick.AddListener(delegate { OnSettingsPress(); });
        menuCreditsButton.onClick.AddListener(delegate { OnCreditsPress(); });
        menuExitButton.onClick.AddListener(delegate { OnExitPress(); });

        charBackButton.onClick.AddListener(delegate { OnCharBackPress(); });
        deleteConfirm.onClick.AddListener(delegate { DeleteConfirm(); });
        deleteDeny.onClick.AddListener(delegate { DeleteDeny(); });

        settingsBack.onClick.AddListener(delegate { OnSettingsBackPress(); });
        creditsBack.onClick.AddListener(delegate { OnCreditsBackPress(); });

        charPlayButton.onClick.AddListener(delegate { OnPlayPress(); });

        menuParentObj.GetComponent<Animator>().SetTrigger("Sway");
        menuCanvas.GetComponent<Animator>().SetTrigger("Open");
    }

    void Awake()
    {
        hairSlider.maxValue = GetComponent<CharacterSpriteManager>().hairSprites.Length - 1;
        headSlider.maxValue = GetComponent<CharacterSpriteManager>().headSprites.Length - 1;
        bodySlider.maxValue = GetComponent<CharacterSpriteManager>().bodySprites.Length - 1;
        feetSlider.maxValue = GetComponent<CharacterSpriteManager>().feetSprites.Length - 1;
    }

    void Start()
    {
        StartCoroutine("FadeIn");
    }

    public void OnDevelopmentToggleEnter()
    {
        developmentModeText.alpha = 1f;
        developmentModeText.interactable = true;
        developmentModeText.blocksRaycasts = true;
    }

    public void OnDevelopmentToggleExit()
    {
        developmentModeText.alpha = 0f;
        developmentModeText.interactable = false;
        developmentModeText.blocksRaycasts = false;
    }

    void OnSettingsBackPress()
    {
        menuParentObj.GetComponent<Animator>().SetTrigger("Sway");
        settingsCanvas.GetComponent<Animator>().SetTrigger("Close");
        settingsCanvas.GetComponent<CanvasGroup>().interactable = false;
        settingsCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

        menuCanvas.GetComponent<Animator>().SetTrigger("Open");
        menuCanvas.GetComponent<CanvasGroup>().interactable = true;
        menuCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    void OnCreditsBackPress()
    {
        menuParentObj.GetComponent<Animator>().SetTrigger("Sway");
        creditsCanvas.GetComponent<Animator>().SetTrigger("Close");
        creditsCanvas.GetComponent<CanvasGroup>().interactable = false;
        creditsCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

        menuCanvas.GetComponent<Animator>().SetTrigger("Open");
        menuCanvas.GetComponent<CanvasGroup>().interactable = true;
        menuCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    void OnMainPlayPress()
    {
        mainCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        mainCanvas.GetComponent<CanvasGroup>().interactable = false;
        mainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

        menuCanvas.GetComponent<CanvasGroup>().alpha = 1f;
        menuCanvas.GetComponent<CanvasGroup>().interactable = true;
        menuCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    void OnSinglePress()
    {
        menuParentObj.GetComponent<Animator>().SetTrigger("Sway");
        menuCanvas.GetComponent<Animator>().SetTrigger("Close");
        menuCanvas.GetComponent<CanvasGroup>().interactable = false;
        menuCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

        CodecManager.Instance.LoadProfiles(charSlots);

        characterCanvas.GetComponent<Animator>().SetTrigger("Open");
        characterCanvas.GetComponent<CanvasGroup>().interactable = true;
        characterCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        AudioManager.instance.PlaySound("Select_Pickup");
    }
    
    void OnSettingsPress()
    {
        menuParentObj.GetComponent<Animator>().SetTrigger("Sway");
        settingsCanvas.GetComponent<Animator>().SetTrigger("Open");
        settingsCanvas.GetComponent<CanvasGroup>().interactable = true;
        settingsCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        menuCanvas.GetComponent<Animator>().SetTrigger("Close");
        menuCanvas.GetComponent<CanvasGroup>().interactable = false;
        menuCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    void OnCreditsPress()
    {
        menuParentObj.GetComponent<Animator>().SetTrigger("Sway");
        creditsCanvas.GetComponent<Animator>().SetTrigger("Open");
        creditsCanvas.GetComponent<CanvasGroup>().interactable = true;
        creditsCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        menuCanvas.GetComponent<Animator>().SetTrigger("Close");
        menuCanvas.GetComponent<CanvasGroup>().interactable = false;
        menuCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    void OnExitPress()
    {
        Application.Quit();

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    void OnCharBackPress()
    {
        menuParentObj.GetComponent<Animator>().SetTrigger("Sway");
        characterCanvas.GetComponent<Animator>().SetTrigger("Close");
        characterCanvas.GetComponent<CanvasGroup>().interactable = false;
        characterCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

        menuCanvas.GetComponent<Animator>().SetTrigger("Open");
        menuCanvas.GetComponent<CanvasGroup>().interactable = true;
        menuCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    public void SwitchToCharCreation(bool charCreation, ProfileManager profile)
    {
        if (charCreation)
        {
            charHolder.GetComponentInParent<CanvasGroup>().alpha = 0f;
            charHolder.GetComponentInParent<CanvasGroup>().interactable = false;
            charHolder.GetComponentInParent<CanvasGroup>().blocksRaycasts = false;

            charCreate.GetComponent<CanvasGroup>().alpha = 1f;
            charCreate.GetComponent<CanvasGroup>().interactable = true;
            charCreate.GetComponent<CanvasGroup>().blocksRaycasts = true;

            AudioManager.instance.PlaySound("Select_Pickup");
        }
        else
        {
            charCreate.GetComponent<CanvasGroup>().alpha = 0f;
            charCreate.GetComponent<CanvasGroup>().interactable = false;
            charCreate.GetComponent<CanvasGroup>().blocksRaycasts = false;

            AudioManager.instance.PlaySound("Select_Pickup");

            curSelectedProfile = profile.gameObject;
        }
    }

    void OnPlayPress()
    {
        if (curSelectedProfile != null)
        {
            StartCoroutine("FadeOut");
            fadeTime = GetComponent<FadingManager>().BeginFade(1) + Time.time;
            canFade = true;

            AudioManager.instance.PlaySound("Select_Pickup");
        }
    }

    void Update()
    {
        if (Time.time > fadeTime && canFade)
            LoadingScreenManager.LoadScene(1);
    }

    IEnumerator FadeWait(float fadeTime)
    {
        yield return new WaitForSeconds(fadeTime);
    }

    public void OpenDeleteMenu()
    {
        charDelete.GetComponent<CanvasGroup>().alpha = 1f;
        charDelete.GetComponent<CanvasGroup>().interactable = true;
        charDelete.GetComponent<CanvasGroup>().blocksRaycasts = true;

        AudioManager.instance.PlaySound("Select_Pickup");
    }

    public void DeleteConfirm()
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + "/Profile" + CodecManager.profileToLoadID);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
        catch(IOException)
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + "/Profile" + CodecManager.profileToLoadID);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
        curSelectedProfile = null;
        characterNameText.text = string.Empty;
        //PlayerPrefs.DeleteAll();

        charDelete.GetComponent<CanvasGroup>().alpha = 0f;
        charDelete.GetComponent<CanvasGroup>().interactable = false;
        charDelete.GetComponent<CanvasGroup>().blocksRaycasts = false;

        MainMenuManager.Instance.hairImage.GetComponent<CanvasGroup>().alpha = 0f;
        MainMenuManager.Instance.headImage.GetComponent<CanvasGroup>().alpha = 0f;
        MainMenuManager.Instance.bodyImage.GetComponent<CanvasGroup>().alpha = 0f;
        MainMenuManager.Instance.feetImage.GetComponent<CanvasGroup>().alpha = 0f;

        AudioManager.instance.PlaySound("Select_Pickup");

    }

    public void DeleteDeny()
    {
        charDelete.GetComponent<CanvasGroup>().alpha = 0f;
        charDelete.GetComponent<CanvasGroup>().interactable = false;
        charDelete.GetComponent<CanvasGroup>().blocksRaycasts = false;

        AudioManager.instance.PlaySound("Select_Pickup");
    }
    
    public void OnMouseOver()
    {
        AudioManager.instance.PlaySound("Hover_Over");
    }

    public IEnumerator FadeOut()
    {
        MainMenuManager.Instance.musicMixer.SetFloat("MusicVol", SettingsManager.Instance.musicVolumeSlider.value);
        float startVol = SettingsManager.Instance.musicVolumeSlider.value;
        float rate = 1.0f / AudioManager.instance.fadeTime;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            MainMenuManager.Instance.musicMixer.SetFloat("MusicVol", Mathf.Lerp(startVol, -80f, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }

        MainMenuManager.Instance.musicMixer.SetFloat("MusicVol", -80f);
    }

    private IEnumerator FadeIn()
    {

        MainMenuManager.Instance.musicMixer.SetFloat("MusicVol", -80f);
        float startVol = -80f;
        float rate = 1.0f / AudioManager.instance.fadeTime;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            MainMenuManager.Instance.musicMixer.SetFloat("MusicVol", Mathf.Lerp(startVol, SettingsManager.Instance.musicVolumeSlider.value, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }

        MainMenuManager.Instance.musicMixer.SetFloat("MusicVol", SettingsManager.Instance.musicVolumeSlider.value);
    }
}
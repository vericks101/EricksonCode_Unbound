using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets._2D;
using System.Collections;
using System.IO;

public class SettingsManager : MonoBehaviour 
{
	public Toggle fullscreenToggle;
	public Toggle devModeToggle;
	public Toggle enemySpawningToggle;
	public Dropdown resolutionDropdown;
	public Dropdown textureQualityDropdown;
	public Dropdown antialiasingDropdown;
	public Dropdown vSyncDropdown;
	public Slider musicVolumeSlider;
	public Slider sfxVolumeSlider;
    public Button applyButton;

    public Button statsButton;
    public Text questCompletionText;
    public Text overallIncomeText;
    public Text overallEnemyKillsText;
    public Text overallDeathsText;

	public Resolution[] resolutions;
	public GameSettings gameSettings;
	public SpawningManager spawningManager;

    public static SettingsManager Instance;

    void Awake()
    {
        if (GameManager.Instance != null)
            spawningManager = CurrentSceneManager.Instance.GetComponent<SpawningManager>();

        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this.transform.parent.gameObject);
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.transform.parent);
        }
    }

	void OnEnable()
	{
		gameSettings = new GameSettings ();

		fullscreenToggle.onValueChanged.AddListener (delegate { OnFullscreenToggle(); });
		devModeToggle.onValueChanged.AddListener (delegate { OnDevModeToggle(); });
		enemySpawningToggle.onValueChanged.AddListener (delegate { OnEnemySpawningToggle(); });
		resolutionDropdown.onValueChanged.AddListener (delegate { OnResolutionChange(); });
		textureQualityDropdown.onValueChanged.AddListener (delegate { OnTextureQualityChange(); });
		antialiasingDropdown.onValueChanged.AddListener (delegate { OnAntialiasingChange(); });
		vSyncDropdown.onValueChanged.AddListener (delegate { OnVSyncChange(); });
		musicVolumeSlider.onValueChanged.AddListener (delegate { OnMusicVolumeChange(); });
		sfxVolumeSlider.onValueChanged.AddListener (delegate { OnSFXVolumeChange(); });
        applyButton.onClick.AddListener(delegate { OnApplySettingsPress(); });
        if (statsButton != null)
            statsButton.onClick.AddListener(delegate { OnStatsPress(); });

        resolutions = Screen.resolutions;
        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        LoadSettings();
	}

    void OnLevelWasLoaded()
    {
        LoadSettings();
    }

    void OnStatsPress()
    {
        int questsCompleted = 0;
        for (int i = 0; i < QuestManager.Instance.questCompleted.Length; i++)
        {
            if (QuestManager.Instance.questCompleted[i])
                questsCompleted++;
        }

        float completionPercentage = Mathf.Floor(((float)questsCompleted / (float)QuestManager.Instance.quests.Length) * 100f);
        questCompletionText.text = completionPercentage.ToString() + "%";
        overallIncomeText.text = "$" + MenuManager.Instance.overallIncome.ToString();
        overallEnemyKillsText.text = MenuManager.Instance.enemyKills.ToString();
        overallDeathsText.text = MenuManager.Instance.overallDeaths.ToString();
    }

	public void OnFullscreenToggle()
	{
		gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
	}

	public void OnDevModeToggle()
	{
        if (Player.Instance != null)
        {
            gameSettings.devMode = Player.Instance.devMode = devModeToggle.isOn;

            if (devModeToggle.isOn)
            {
                Player.Instance.GetComponent<Rigidbody2D>().isKinematic = true;
                Player.Instance.GetComponent<Platformer2DUserControl>().enabled = false;
                Player.Instance.GetComponent<BoxCollider2D>().enabled = false;
                Player.Instance.GetComponent<CircleCollider2D>().enabled = false;
            }
            else
            {
                Player.Instance.GetComponent<Rigidbody2D>().isKinematic = false;
                Player.Instance.GetComponent<Platformer2DUserControl>().enabled = true;
                Player.Instance.GetComponent<BoxCollider2D>().enabled = true;
                Player.Instance.GetComponent<CircleCollider2D>().enabled = true;
            }
        }
        else
            gameSettings.devMode = devModeToggle.isOn;
    }

	public void OnEnemySpawningToggle()
	{
		gameSettings.enemySpawning = enemySpawningToggle.isOn;
        if (!gameSettings.enemySpawning)
        {
            if (SpawningManager.Instance != null)
                SpawningManager.Instance.KillMobs();
        }
            
        if (spawningManager != null)
		    spawningManager.enabled = gameSettings.enemySpawning;
	}

	public void OnResolutionChange()
	{
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        gameSettings.resolutionIndex = resolutionDropdown.value;
	}

	public void OnTextureQualityChange()
	{
        gameSettings.textureQuality = textureQualityDropdown.value;
		QualitySettings.SetQualityLevel (gameSettings.textureQuality, false);
	}

	public void OnAntialiasingChange()
	{
        if (antialiasingDropdown.value != 0)
            QualitySettings.antiAliasing = gameSettings.antialiasing = (int)Mathf.Pow(2, antialiasingDropdown.value);
        else
            QualitySettings.antiAliasing = gameSettings.antialiasing = 0;
    }

	public void OnVSyncChange()
	{
		QualitySettings.vSyncCount = gameSettings.vSync = vSyncDropdown.value;
	}

	public void OnMusicVolumeChange()
	{
        if (GameManager.Instance != null)
        {
            GameManager.Instance.musicMixer.SetFloat("MusicVol", musicVolumeSlider.value);

            float overallInterval = musicVolumeSlider.maxValue - musicVolumeSlider.minValue;
            float lengthInvertal = musicVolumeSlider.value - musicVolumeSlider.minValue;
            gameSettings.musicVolume = lengthInvertal / overallInterval;
        }
        if (MainMenuManager.Instance != null)
        {
            MainMenuManager.Instance.musicMixer.SetFloat("MusicVol", musicVolumeSlider.value);

            float overallInterval = musicVolumeSlider.maxValue - musicVolumeSlider.minValue;
            float lengthInvertal = musicVolumeSlider.value - musicVolumeSlider.minValue;
            gameSettings.musicVolume = lengthInvertal / overallInterval;
        }
	}

	public void OnSFXVolumeChange()
	{
        if (GameManager.Instance != null)
        {
            GameManager.Instance.sfxMixer.SetFloat("SFXVol", sfxVolumeSlider.value);

            float overallInterval = sfxVolumeSlider.maxValue - sfxVolumeSlider.minValue;
            float lengthInvertal = sfxVolumeSlider.value - sfxVolumeSlider.minValue;
            gameSettings.sfxVolume = lengthInvertal / overallInterval;
        }
        if (MainMenuManager.Instance != null)
        {
            MainMenuManager.Instance.sfxMixer.SetFloat("SFXVol", sfxVolumeSlider.value);

            float overallInterval = sfxVolumeSlider.maxValue - sfxVolumeSlider.minValue;
            float lengthInvertal = sfxVolumeSlider.value - sfxVolumeSlider.minValue;
            gameSettings.sfxVolume = lengthInvertal / overallInterval;
        }
    }

    public void OnApplySettingsPress()
    {
        SaveSettings();
    }

	public void SaveSettings()
	{
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
	}

	public void LoadSettings()
	{
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

        musicVolumeSlider.value = musicVolumeSlider.minValue + (Mathf.Abs(musicVolumeSlider.maxValue - musicVolumeSlider.minValue) * gameSettings.musicVolume);
        sfxVolumeSlider.value = sfxVolumeSlider.minValue + Mathf.Abs(sfxVolumeSlider.maxValue - sfxVolumeSlider.minValue) * gameSettings.sfxVolume;
        antialiasingDropdown.value = gameSettings.antialiasing;
        vSyncDropdown.value = gameSettings.vSync;
        textureQualityDropdown.value = gameSettings.textureQuality;
        resolutionDropdown.value = gameSettings.resolutionIndex;
        fullscreenToggle.isOn = gameSettings.fullscreen;

        Screen.fullScreen = gameSettings.fullscreen;
        resolutionDropdown.RefreshShownValue();
    }
}
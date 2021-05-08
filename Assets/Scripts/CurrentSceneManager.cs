using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrentSceneManager : MonoBehaviour
{
    public bool mobSpawning;

    public bool noTODLighting;

    public bool noEvents;

    public bool damageOverTime;
    public float damageOverTimeAmount;
    public float damageOverTimeCooldown;
    private float timeSinceLastDOT;
    public bool cancelDOTEffect;

    public Element planetElement;

    public bool ignoreTrackChange;
    public string dayTrack;
    public string nightTrack;

    public string systemName;
    public string planetName;
    [SerializeField] private Text systemText;
    [SerializeField] private Text planetText;
    [SerializeField] private Animator destinationNamingAnim;

    [SerializeField] private bool noDestinationNaming;

    private static CurrentSceneManager instance;
    public static CurrentSceneManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<CurrentSceneManager>();
            return instance;
        }
    }

    private void Awake()
    {
        try
        {
            destinationNamingAnim = GameObject.Find("DestinationNamingCanvas").GetComponent<Animator>();
            systemText = destinationNamingAnim.gameObject.transform.Find("SystemNameText").GetComponent<Text>();
            planetText = destinationNamingAnim.gameObject.transform.Find("PlanetNameText").GetComponent<Text>();

            systemText.text = systemName;
            planetText.text = planetName;
        }
        catch(System.NullReferenceException)
        { }
    }

    private void Start()
    {
        if (systemName != null && planetName != null && destinationNamingAnim != null && !noDestinationNaming)
            destinationNamingAnim.SetTrigger("Fading");

        StartCoroutine("FadeIn");

        if (noTODLighting)
        {
            if (Player.Instance != null)
                Player.Instance.transform.Find("Graphics").GetComponent<SpriteRenderer>().color = Color.white;
            //SetTintRecursively(Player.Instance.transform.Find("Arm").gameObject, Color.white);
        }
    }

    private void Update()
    {
        if (damageOverTime && !cancelDOTEffect && Player.Instance != null && !Player.Instance.isDead && Time.time > timeSinceLastDOT)
        {
            Player.Instance.DamagePlayer(damageOverTimeAmount, 0f, new Vector2(0f, 0f), 0.1f, 0.1f);
            timeSinceLastDOT = Time.time + damageOverTimeCooldown;
        }
    }

    void SetTintRecursively(GameObject obj, Color tint)
    {
        if (obj.GetComponent<SpriteRenderer>() != null)
            obj.GetComponent<SpriteRenderer>().color = tint;

        foreach (Transform child in obj.transform)
        {
            if (child.transform.GetComponent<SpriteRenderer>() != null)
            {
                SetTintRecursively(child.gameObject, tint);
            }
        }
    }

    public IEnumerator FadeOut()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.musicMixer.SetFloat("MusicVol", SettingsManager.Instance.musicVolumeSlider.value);
        float startVol = SettingsManager.Instance.musicVolumeSlider.value;
        float rate = 1.0f / AudioManager.instance.fadeTime;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.musicMixer.SetFloat("MusicVol", Mathf.Lerp(startVol, -80f, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.musicMixer.SetFloat("MusicVol", -80f);
    }

    private IEnumerator FadeIn()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.musicMixer.SetFloat("MusicVol", -80f);
        float startVol = -80f;
        float rate = 1.0f / AudioManager.instance.fadeTime;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.musicMixer.SetFloat("MusicVol", Mathf.Lerp(startVol, SettingsManager.Instance.musicVolumeSlider.value, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.musicMixer.SetFloat("MusicVol", SettingsManager.Instance.musicVolumeSlider.value);
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TODManager : MonoBehaviour
{
    private const string SUN_ANIM_NAME = "SunMove";
    private const string MOON_ANIM_NAME = "MoonMove";
    private const string TINT_CONTROL_NAME = "TintControlAnim";
    private const string NIGHT_TINT_CONTROL_NAME = "TintControlNightAnim";
    private const string BACKGROUND_TINT_NAME = "BackgroundColorAnim";

    [SerializeField] private Animator sunAnimator;
    [SerializeField] private Animator moonAnimator;
    [SerializeField] private Animator backgroundAnimator;
    [SerializeField] private Animator gameManagerAnimator;

    [SerializeField] private float timeScale;
    public float TimeScale
    {
        get { return timeScale; }
        set
        {
            timeScale = value;
            sunAnimator.speed = timeScale;
            moonAnimator.speed = timeScale;
            backgroundAnimator.speed = timeScale;
            gameManagerAnimator.speed = timeScale;
        }
    }

    public float normalizedTime;
    [SerializeField] private bool firstTime = true;
    public bool isDay = false;
    public float curTime;

    [SerializeField] private GameObject lightPrefab;

    public Color currentBackgroundColor;
    public List<SpriteRenderer> currentBackgrounds;

    public float currentLightTint;
    public float currentBrushTint;
    public float currentMaxTint;

    [SerializeField] private float updatePeriod;
    private float lastUpdateTime;

    public int playerLightingDistance;
    public float playerTintScale;

    void Awake()
    {
        TimeScale = timeScale;

        sunAnimator.speed = timeScale;
        moonAnimator.speed = timeScale;
        backgroundAnimator.speed = timeScale;
        gameManagerAnimator.speed = timeScale;
    }

    void Start()
    {
        lightPrefab.GetComponent<TerrainLighting>().tintScale = currentLightTint;
        UpdateLighting(currentLightTint);

        for (int i = 0; i < Player.Instance.playerLightableObjects.Length; i++)
        {
            if (!TerrainLighting.lightObjects.ContainsValue(Player.Instance.playerLightableObjects[i]))
                TerrainLighting.lightObjects.Add(Player.Instance.playerLightableObjects[i].name, Player.Instance.playerLightableObjects[i]);
        }
    }

    void OnLevelWasLoaded()
    {
        if (isDay)
        {
            AudioManager.instance.StopTrack(AudioManager.instance.currentTrack.name);
            AudioManager.instance.ChangeSoundTrack(CurrentSceneManager.Instance.dayTrack, CurrentSceneManager.Instance.ignoreTrackChange);
        }
        else if (!isDay)
        {
            AudioManager.instance.StopTrack(AudioManager.instance.currentTrack.name);
            AudioManager.instance.ChangeSoundTrack(CurrentSceneManager.Instance.nightTrack, CurrentSceneManager.Instance.ignoreTrackChange);
        }

        for (int i = 0; i < Player.Instance.playerLightableObjects.Length; i++)
        {
            TerrainLighting.lightObjects.Add(Player.Instance.playerLightableObjects[i].name, Player.Instance.playerLightableObjects[i]);
        }
    }

    void Update()
    {
        if (isDay && !sunAnimator.GetCurrentAnimatorStateInfo(0).IsName(SUN_ANIM_NAME) && normalizedTime > 0f && firstTime)
            ChangeToDay();
        else if (!isDay && !moonAnimator.GetCurrentAnimatorStateInfo(0).IsName(MOON_ANIM_NAME) && normalizedTime > 0f && firstTime)
            ChangeToNight();
        else
        {
            if (isDay && (curTime >= 1f || !sunAnimator.GetCurrentAnimatorStateInfo(0).IsName(SUN_ANIM_NAME)) 
                && (sunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f || !sunAnimator.GetCurrentAnimatorStateInfo(0).IsName(SUN_ANIM_NAME)))
                ChangeToNight();
            else if (!isDay && (curTime >= 1f || !moonAnimator.GetCurrentAnimatorStateInfo(0).IsName(MOON_ANIM_NAME)) 
                && (moonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f || !moonAnimator.GetCurrentAnimatorStateInfo(0).IsName(MOON_ANIM_NAME)))
                ChangeToDay();
        }

        if (Time.time > (lastUpdateTime + updatePeriod))
        {
            lightPrefab.GetComponent<TerrainLighting>().tintScale = currentLightTint;
            UpdateLighting(currentLightTint);
            for (int i = 0; i < currentBackgrounds.Count; i++)
            {
                if (currentBackgrounds[i] != null)
                    currentBackgrounds[i].color = currentBackgroundColor;
            }
        
            lastUpdateTime = Time.time;
        }

        AnimatorStateInfo animationState = gameManagerAnimator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] animatorClip = gameManagerAnimator.GetCurrentAnimatorClipInfo(0);

        if (animatorClip.Length > 0)
            curTime = animatorClip[0].clip.length * animationState.normalizedTime;
    }
    
    void ChangeToDay()
    {
        for (int i = 0; i < CurrentSceneManager.Instance.GetComponent<SpawningManager>().enemies.Count; i++)
            CurrentSceneManager.Instance.GetComponent<SpawningManager>().enemies[i].spawnChance /= 1.25f;

        if (firstTime)
            gameManagerAnimator.Play(TINT_CONTROL_NAME, 0, normalizedTime);
        else
            gameManagerAnimator.Play(TINT_CONTROL_NAME, 0);

        if (firstTime)
            backgroundAnimator.Play(BACKGROUND_TINT_NAME, 0, normalizedTime);
        else
            backgroundAnimator.Play(BACKGROUND_TINT_NAME, 0);

        isDay = true;
        curTime = 0;
        if (!firstTime)
            normalizedTime = 0;

        if (firstTime)
            sunAnimator.Play(SUN_ANIM_NAME, 0, normalizedTime);
        else
            sunAnimator.Play(SUN_ANIM_NAME, 0, 0);

        firstTime = false;

        if (AudioManager.instance.currentTrack != null && CurrentSceneManager.Instance != null && CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent == null)
            AudioManager.instance.StopTrack(AudioManager.instance.currentTrack.name);
        AudioManager.instance.ChangeSoundTrack(CurrentSceneManager.Instance.dayTrack, CurrentSceneManager.Instance.ignoreTrackChange);
    }

    void ChangeToNight()
    {
        if (CurrentSceneManager.Instance != null)
        {
            for (int i = 0; i < CurrentSceneManager.Instance.GetComponent<SpawningManager>().enemies.Count; i++)
                CurrentSceneManager.Instance.GetComponent<SpawningManager>().enemies[i].spawnChance *= 1.25f;
        }

        if (firstTime)
            gameManagerAnimator.Play(NIGHT_TINT_CONTROL_NAME, 0, normalizedTime);
        else
            gameManagerAnimator.Play(NIGHT_TINT_CONTROL_NAME, 0, 0);

        isDay = false;
        curTime = 0;
        if (!firstTime)
            normalizedTime = 0;

        if (firstTime)
            moonAnimator.Play(MOON_ANIM_NAME, 0, normalizedTime);
        else
            moonAnimator.Play(MOON_ANIM_NAME, 0, 0);

        firstTime = false;

        if (AudioManager.instance.currentTrack != null && CurrentSceneManager.Instance != null && CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent == null)
            AudioManager.instance.StopTrack(AudioManager.instance.currentTrack.name);
        AudioManager.instance.ChangeSoundTrack(CurrentSceneManager.Instance.nightTrack, CurrentSceneManager.Instance.ignoreTrackChange);
    }

    public void UpdateLighting(float tintScale)
    {
        if (CurrentSceneManager.Instance.noTODLighting)
        {
            for (int i = 0; i < Player.Instance.playerLightableObjects.Length; i++)
            {
                if (Player.Instance.playerLightableObjects[i].tag == "PlayerMasked")
                    Player.Instance.playerLightableObjects[i].GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
                else
                    Player.Instance.playerLightableObjects[i].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Player.Instance.playerLightableObjects[i].GetComponent<SpriteRenderer>().color.a);
            }
        }

        foreach (var pair in TerrainLighting.lightObjects)
        {
            if (pair.Value != null)
            {
                if (pair.Value.layer == 8 || pair.Value.layer == 14 || pair.Value.layer == 15 || pair.Value.layer == 16)
                    UpdatePlayerTiles(pair.Value, currentLightTint);
                else if (pair.Value.GetComponent<TerrainLighting>() != null && pair.Value.GetComponent<TerrainLighting>().mode == Mode.SPOTLIGHT)
                    pair.Value.GetComponent<TerrainLighting>().UpdateSurroundingTiles(currentLightTint, pair.Value.GetComponent<TerrainLighting>().lightingDistance, pair.Value);
            }
        }
        foreach(var pair in TerrainLighting.lightObjects)
        {
            if (pair.Value != null)
            {
                if (pair.Value.GetComponent<TerrainLighting>() != null && pair.Value.GetComponent<TerrainLighting>().mode == Mode.AREA)
                    pair.Value.GetComponent<TerrainLighting>().UpdateSurroundingTiles(currentLightTint, pair.Value.GetComponent<TerrainLighting>().lightingDistance, pair.Value);
            }
        }
    }

    private void UpdatePlayerTiles(GameObject go, float currentLightTint)
    {
        float lightingTint = 0f;
        try
        {
            for (int i = 0; i < playerLightingDistance; i++)
            {
                if (GameManager.Instance.terrainMap.GetComponent<TerrainEditor>().CurrentMap[(int)go.transform.position.x, (int)go.transform.position.y + i] != 0
                    || GameManager.Instance.terrainMap.GetComponent<TerrainEditor>().CurrentBackMap[(int)go.transform.position.x, (int)go.transform.position.y + i] != 0)
                    lightingTint = Mathf.Clamp(lightingTint + playerTintScale, 0f, 1f);
            }
            if (go.tag == "PlayerMasked")
            {
                if (lightingTint > 0.1f)
                    go.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, lightingTint);
                else
                    go.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f - currentLightTint);
            }
            else if (go.tag != "PlayerColored")
            {
                if (lightingTint > 0.1f)
                    go.GetComponent<SpriteRenderer>().color = new Color(1f - lightingTint, 1f - lightingTint, 1f - lightingTint, go.GetComponent<SpriteRenderer>().color.a);
                else
                    go.GetComponent<SpriteRenderer>().color = new Color(currentLightTint, currentLightTint, currentLightTint, go.GetComponent<SpriteRenderer>().color.a);
            }
        }
        catch(System.NullReferenceException)
        { }
        catch(System.IndexOutOfRangeException)
        { }
    }
}
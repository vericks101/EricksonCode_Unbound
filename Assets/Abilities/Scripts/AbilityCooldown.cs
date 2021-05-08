using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityCooldown : MonoBehaviour
{
    public KeyCode key;
    public Image darkMask;
    public Text cooldownTextDisplay;

    public Ability ability;
    public GameObject objHolder;
    [SerializeField]
    private Image myButtonImage;
    [SerializeField]
    private AudioSource abilitySource;
    [SerializeField]
    private float cooldownDuration;
    [SerializeField]
    private float nextReadyTime;
    [SerializeField]
    private float cooldownTimeLeft;

    void Start()
    {
        Initialize(ability, objHolder);
    }

    void Update()
    {
        bool cooldownComplete = (Time.time > nextReadyTime);
        if (cooldownComplete)
        {
            AbilityReady();
            if (Input.GetKey(key) && !ChatManager.Instance.chatBoxActive && !Player.Instance.isDead)
            {
                ButtonTriggered();
            }
        }
        else
        {
            Cooldown();
        }
    }

    public void Initialize(Ability selectedAbility, GameObject weaponHolder)
    {
        ability = selectedAbility;
        myButtonImage = GetComponent<Image>();
        abilitySource = GetComponent<AudioSource>();
        myButtonImage.sprite = ability.aSprite;
        darkMask.sprite = ability.aSprite;
        cooldownDuration = ability.aBaseCooldown;
        ability.Initialize(weaponHolder);
        AbilityReady();
    }

    void AbilityReady()
    {
        cooldownTextDisplay.enabled = false;
        darkMask.enabled = false;
    }

    void Cooldown()
    {
        cooldownTimeLeft -= Time.deltaTime;
        float roundedCd = Mathf.Round(cooldownTimeLeft);
        cooldownTextDisplay.text = roundedCd.ToString();
        darkMask.fillAmount = cooldownTimeLeft / cooldownDuration;
    }

    void ButtonTriggered()
    {
        if (ability.aBaseCooldown != -1 && ability.manaCost <= Player.Instance.mana.CurrentVal)
        {
            nextReadyTime = cooldownDuration + Time.time;
            cooldownTimeLeft = cooldownDuration;
            darkMask.enabled = true;
            cooldownTextDisplay.enabled = true;
            Player.Instance.mana.CurrentVal -= ability.manaCost;

            abilitySource.clip = ability.aSound;
            abilitySource.Play();
            ability.TriggerAbility();
        }
    }
}

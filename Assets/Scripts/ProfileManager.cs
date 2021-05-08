using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ProfileManager : MonoBehaviour
{
    public Image hairImage;
    public Image headImage;
    public Image bodyImage;
    public Image feetImage;

    private Button profileButton;
    private InputField nameInput;
    private Button charCreateButton;
    private Button charDelete;
    private Button charConfirm;
    private Button tutorialFinish;

    public string profileName;
    public int id;

    private float fadeTime = 0f;
    private bool canFade = false;

    void Awake()
    {
        profileButton = gameObject.transform.Find("BackPanel").GetComponent<Button>();
        nameInput = GameObject.Find("NameInput").GetComponent<InputField>();
        charCreateButton = GameObject.Find("CreateButton").GetComponent<Button>();
        charDelete = GameObject.Find("DeleteButton").GetComponent<Button>();
        charConfirm = GameObject.Find("YesButton").GetComponent<Button>();
        tutorialFinish = GameObject.Find("FinishButton").GetComponent<Button>();
    }

    void OnEnable()
    {
        profileButton.onClick.AddListener(delegate { OnProfileButtonPress(); });
        nameInput.onEndEdit.AddListener(delegate { OnCharacterNameDoneEdit(); });
        charCreateButton.onClick.AddListener(delegate { OnCreateCharacterPress(); });
        charDelete.onClick.AddListener(delegate { OnCharDeletePress(); });
        charConfirm.onClick.AddListener(delegate { OnYesDeletePress(); });
        tutorialFinish.onClick.AddListener(delegate { OnTutorialFinishPress(); });
    }

    void OnProfileButtonPress()
    {
        CodecManager.profileToLoadID = id;
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Profile" + CodecManager.profileToLoadID);

        MainMenuManager.Instance.hairImage.GetComponent<CanvasGroup>().alpha = 1f;
        MainMenuManager.Instance.headImage.GetComponent<CanvasGroup>().alpha = 1f;
        MainMenuManager.Instance.bodyImage.GetComponent<CanvasGroup>().alpha = 1f;
        MainMenuManager.Instance.feetImage.GetComponent<CanvasGroup>().alpha = 1f;

        if (!CodecManager.Instance.LoadProfile("Profile", id))
            MainMenuManager.Instance.SwitchToCharCreation(true, this);
        else
        {
            MainMenuManager.Instance.SwitchToCharCreation(false, this);
            MainMenuManager.Instance.characterNameText.text = gameObject.transform.Find("NameText").GetComponent<Text>().text;
        }
    }

    public void OnCharacterNameDoneEdit()
    {
        if (nameInput.text.Length > 0)
            MainMenuManager.Instance.charSlots[id].GetComponent<ProfileManager>().profileName = nameInput.text;
    }

    void OnCreateCharacterPress()
    {
        if (MainMenuManager.Instance.charSlots[id].GetComponent<ProfileManager>().profileName.Length > 0
            && MainMenuManager.Instance.charSlots[id].GetComponent<ProfileManager>().profileName != "New Profile")
        {
            string tmpString = MainMenuManager.Instance.charSlots[id].GetComponent<ProfileManager>().profileName.Replace(" ", string.Empty);
            if (tmpString.Length > 0)
            {
                CodecManager.Instance.SaveProfile(profileName, id);
                CodecManager.Instance.LoadProfile("Profile", id);

                //MainMenuManager.Instance.tutorialCanvas.GetComponent<Animator>().SetTrigger("Open");
                //MainMenuManager.Instance.tutorialCanvas.GetComponent<CanvasGroup>().alpha = 1f;
                //MainMenuManager.Instance.tutorialCanvas.GetComponent<CanvasGroup>().interactable = true;
                //MainMenuManager.Instance.tutorialCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                MainMenuManager.characterCreated = true;
                MainMenuManager.developmentMode = MainMenuManager.Instance.developmentToggle.isOn;
                fadeTime = MainMenuManager.Instance.GetComponent<FadingManager>().BeginFade(1) + Time.time;
                canFade = true;
            }
            else
                AudioManager.instance.PlaySound("Error");
        }
        else
            AudioManager.instance.PlaySound("Error");
    }

    private void OnTutorialFinishPress()
    {
        //StartCoroutine("FadeOut");
        MainMenuManager.characterCreated = true;
        fadeTime = MainMenuManager.Instance.GetComponent<FadingManager>().BeginFade(1) + Time.time;
        canFade = true;
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

    void OnCharDeletePress()
    {
        CodecManager.Instance.DeleteProfile(id);
    }

    void OnYesDeletePress()
    {
        profileName = "New Profile";
        GetComponentInChildren<Text>().text = profileName;
        id = 0;
    }
}
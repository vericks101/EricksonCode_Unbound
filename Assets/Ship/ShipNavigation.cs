using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShipNavigation : MonoBehaviour
{
    public GameObject navigationUI;
    private Dropdown navigationDropdown;
    private int currentNavIndex;

    private float fadeTime = 0f;
    private bool canFade = false;

    void Awake()
    {
        navigationDropdown = navigationUI.GetComponentInChildren<Dropdown>();
    }

    void OnEnable()
    {
        navigationDropdown.onValueChanged.AddListener(delegate { OnNaviationChange(); });
    }

    void OnNaviationChange()
    {
        currentNavIndex = navigationDropdown.value;
    }

    public void OnButtonPress()
    {
        CodecManager.Instance.SaveInventories();
        CodecManager.Instance.SavePlayerData();
        CodecManager.Instance.SavePreviewData();
        CodecManager.Instance.SaveCharacterTreeData();
        CodecManager.Instance.SaveMenuData();
        CodecManager.Instance.SaveQuestData();

        //SceneManager.LoadScene(currentNavIndex);
        fadeTime = GameManager.Instance.GetComponent<FadingManager>().BeginFade(1) + Time.time;
        canFade = true;
        //StartCoroutine(FadeWait(fadeTime));
    }

    void Update()
    {
        if (Time.time > fadeTime && canFade)
            LoadingScreenManager.LoadScene(currentNavIndex);
    }

    IEnumerator FadeWait(float fadeTime)
    {
        yield return new WaitForSeconds(fadeTime);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNotice : MonoBehaviour
{
    [SerializeField] private Text questTitleText;
    [SerializeField] private Text questDescText;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button denyButton;

    private QuestTrigger currentTrigger;

    [SerializeField] private CanvasGroup canvasGroup;

    public bool isActive;

    private bool fadingOut;
    private bool fadingIn;
    [SerializeField] private float fadeTime;
    private bool instantClose = false;

    private const float PRESS_WAIT_TIME = 0.01f;
    private bool returnPressed = false;
    private float waitTimer;

    private static QuestNotice instance;
    public static QuestNotice Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<QuestNotice>();

            return instance;
        }
    }

    private void Update()
    {
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                returnPressed = true;
                waitTimer = Time.time + PRESS_WAIT_TIME;
            }
            if (returnPressed && Time.time >= waitTimer)
            {
                returnPressed = false;
                AcceptQuest();
            }
        }
    }

    public void EnableQuestNotice(QuestTrigger trigger, string questTitle, string questDesc)
    {
        currentTrigger = null;
        currentTrigger = trigger;

        questTitleText.text = questTitle;
        questDescText.text = questDesc;

        //canvasGroup.alpha = 1f;
        StartCoroutine("FadeIn");
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isActive = true;
    }

    public void DenyQuest()
    {
        //canvasGroup.alpha = 0f;
        StartCoroutine("FadeOut");
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        isActive = false;
    }

    public void AcceptQuest()
    {
        currentTrigger.TriggerQuest();

        //canvasGroup.alpha = 0f;
        StartCoroutine("FadeOut");
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        isActive = false;
    }

    private IEnumerator FadeOut()
    {
        if (!fadingOut)
        {
            fadingOut = true;
            fadingIn = false;
            StopCoroutine("FadeIn");

            float startAlpha = canvasGroup.alpha;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;

            while (progress < 1.0)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, progress);
                progress += rate * Time.deltaTime;

                if (instantClose)
                    break;

                yield return null;
            }

            canvasGroup.alpha = 0;

            fadingOut = false;
            instantClose = false;
        }
    }

    private IEnumerator FadeIn()
    {
        if (!fadingIn)
        {
            fadingOut = false;
            fadingIn = true;
            StopCoroutine("FadeOut");

            float startAlpha = canvasGroup.alpha;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;

            while (progress < 1.0)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, progress);
                progress += rate * Time.deltaTime;

                yield return null;
            }

            canvasGroup.alpha = 1;
            fadingIn = false;
        }
    }
}
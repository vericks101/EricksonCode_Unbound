using System.Collections;
using UnityEngine;

public class Star : MonoBehaviour
{
    private bool fadingOut = false;
    private bool fadingIn = false;
    private bool firstTime = false;

    public float fadeTime;
    private float randomTime;

    private CanvasGroup canvasGroup;
    private CanvasGroup destinationManager;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        destinationManager = FindObjectOfType<DestinationManager>().GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (destinationManager.alpha >= 1f && Time.time >= randomTime)
        {
            if (!firstTime)
                randomTime = Time.time + Random.Range(0, 5f);

            if (!fadingOut && canvasGroup.alpha >= 1f && Time.time >= randomTime)
                StartCoroutine("FadeOut");
            else if (!fadingIn && !fadingOut && canvasGroup.alpha <= 0f && Time.time >= randomTime)
                StartCoroutine("FadeIn");

            firstTime = true;
        }
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

                yield return null;
            }

            canvasGroup.alpha = 0;

            fadingOut = false;
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
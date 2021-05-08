using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public GameObject weatherAudio;
    [SerializeField] private AudioClip weatherClip;

    public GameObject weatherParticleSystem;
    public UnityEngine.Material particleMaterial;
    public GameObject impactPrefab;

    [SerializeField] private float weatherLastTime;
    private float weatherLastTimer;
    [SerializeField] private float weatherCooldownTime;
    private float weatherCooldownTimer;

    private bool weatherOn;

    [SerializeField] private float weatherChance;

    [SerializeField] private float gravityScale;

    private bool fadingIn;
    private bool fadingOut;
    [SerializeField] private float fadeTime;

    private void Start()
    {
        weatherAudio = GameObject.Find("WeatherSounds");
        weatherParticleSystem = GameObject.Find("WeatherParticleSystem");
        if (weatherParticleSystem != null)
            weatherParticleSystem.GetComponent<ParticleSystem>().Stop();
        weatherOn = false;
        if (weatherAudio != null)
            weatherAudio.GetComponent<AudioSource>().clip = weatherClip;
    }

    private void OnLevelWasLoaded()
    {
        weatherAudio = GameObject.Find("WeatherSounds");
        weatherParticleSystem = GameObject.Find("WeatherParticleSystem");
        if (weatherParticleSystem != null)
            weatherParticleSystem.GetComponent<ParticleSystem>().Stop();
        weatherOn = false;
        if (weatherAudio != null)
            weatherAudio.GetComponent<AudioSource>().clip = weatherClip;
    }

    private void Update()
    {
        if (weatherCooldownTimer <= Time.time && !weatherOn)
        {
            weatherCooldownTimer = Time.time + weatherCooldownTime;
            weatherLastTimer = Time.time + weatherLastTime;

            if (Random.Range(0f, 1f) < weatherChance)
                EnableWeather();
        }

        if (weatherLastTimer <= Time.time && weatherOn)
            DisableWeather();
    }

    private void EnableWeather()
    {
        if (weatherParticleSystem != null)
        {
            weatherParticleSystem.GetComponent<Renderer>().material = particleMaterial;
            weatherParticleSystem.GetComponent<ParticleSystem>().gravityModifier = gravityScale;
            weatherParticleSystem.GetComponent<ParticleSystem>().Play();
            weatherAudio.GetComponent<AudioSource>().Play();
            StartCoroutine("FadeIn");
            weatherOn = true;
        }
    }

    private void DisableWeather()
    {
        if (weatherParticleSystem != null)
        {
            weatherParticleSystem.GetComponent<ParticleSystem>().Stop();
            StartCoroutine("FadeOut");
        }
        weatherOn = false;
    }

    private IEnumerator FadeOut()
    {
        if (!fadingOut)
        {
            fadingOut = true;
            fadingIn = false;
            StopCoroutine("FadeIn");

            weatherAudio.GetComponent<AudioSource>().volume = 1f;
            float startVol = 1f;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;

            while (progress < 1.0)
            {
                weatherAudio.GetComponent<AudioSource>().volume = Mathf.Lerp(startVol, 0f, progress);
                progress += rate * Time.deltaTime;

                yield return null;
            }

            weatherAudio.GetComponent<AudioSource>().volume = 0f;
            weatherAudio.GetComponent<AudioSource>().Stop();

            fadingOut = false;
            //StartCoroutine("FadeIn");
        }
    }

    private IEnumerator FadeIn()
    {
        if (!fadingIn)
        {
            fadingOut = false;
            fadingIn = true;
            StopCoroutine("FadeOut");

            weatherAudio.GetComponent<AudioSource>().volume = 0f;
            float startVol = 0f;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;

            while (progress < 1.0)
            {
                weatherAudio.GetComponent<AudioSource>().volume = Mathf.Lerp(startVol, 1f, progress);
                progress += rate * Time.deltaTime;

                yield return null;
            }

            weatherAudio.GetComponent<AudioSource>().volume = 1f;
            fadingIn = false;
        }
    }
}
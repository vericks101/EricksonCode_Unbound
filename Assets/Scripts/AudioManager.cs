using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

[System.Serializable]
public class Sound
{
	public string name;
	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume = 0.7f;
	[Range(0.5f, 1.5f)]
	public float pitch = 1f;

	[Range(0f, 0.5f)]
	public float randomVolume = 0.1f;
	[Range(0f, 0.5f)]
	public float randomPitch = 0.1f;

	public bool loop = false;

	private AudioSource source;

	public void SetSource(AudioSource _source)
	{
		source = _source;
		source.clip = clip;
		source.loop = loop;
	}

	public void Play()
	{
		source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
		source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
		source.Play();
	}

	public void Stop()
	{
        if (source != null)
		    source.Stop();
	}
}

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup sfxMixerGroup;
    public AudioMixerGroup trackMixerGroup;

	public static AudioManager instance;

	[SerializeField] Sound[] sounds;
    [SerializeField] Sound[] tracks;
    public Sound currentTrack;

    public float fadeTime;
    private bool fadingIn;
    private bool fadingOut;

	void Awake()
	{
		if (instance != null)
		{
			if (instance != this)
			{
				Destroy(this.gameObject);
			}
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
	}

	void Start()
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
			_go.transform.SetParent(this.transform);
			sounds[i].SetSource(_go.AddComponent<AudioSource>());
            _go.GetComponent<AudioSource>().outputAudioMixerGroup = sfxMixerGroup;
		}
        for (int i = 0; i < tracks.Length; i++)
        {
            GameObject _go = new GameObject("Track_" + i + "_" + tracks[i].name);
            _go.transform.SetParent(this.transform);
            tracks[i].SetSource(_go.AddComponent<AudioSource>());
            _go.GetComponent<AudioSource>().outputAudioMixerGroup = trackMixerGroup;
        }
	}

    public void ChangeSoundTrack(string _name, bool ignore)
    {
        if (!ignore)
        {
            if (CurrentSceneManager.Instance.GetComponent<EventManager>() != null && CurrentSceneManager.Instance.GetComponent<EventManager>().CurrentEvent == null)
            {
                for (int i = 0; i < tracks.Length; i++)
                {
                    if (tracks[i].name == _name)
                    {
                        currentTrack = tracks[i];
                        GameManager.Instance.musicMixer.SetFloat("MusicVol", -80f);
                        currentTrack.Play();
                        StartCoroutine("FadeIn");
                        return;
                    }
                }
            }
        }
    }

	public void PlaySound(string _name)
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i].name == _name)
			{
				sounds[i].Play();
				return;
			}
		}
	}

	public void StopSound(string _name)
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i].name == _name)
			{
				sounds[i].Stop();
				return;
			}
		}
	}

    public void StopTrack(string _name)
    {
        for (int i = 0; i < tracks.Length; i++)
        {
            if (tracks[i].name == _name)
            {
                tracks[i].Stop();
                return;
            }
        }
    }

    private IEnumerator FadeOut()
    {
        if (!fadingOut)
        {
            fadingOut = true;
            fadingIn = false;
            StopCoroutine("FadeIn");

            GameManager.Instance.musicMixer.SetFloat("MusicVol", SettingsManager.Instance.musicVolumeSlider.value);
            float startVol = SettingsManager.Instance.musicVolumeSlider.value;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;

            while (progress < 1.0)
            {
                GameManager.Instance.musicMixer.SetFloat("MusicVol", Mathf.Lerp(startVol, -80f, progress));
                progress += rate * Time.deltaTime;

                yield return null;
            }

            currentTrack.volume = 0f;

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

            GameManager.Instance.musicMixer.SetFloat("MusicVol", -80f);
            float startVol = -80f;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;

            while (progress < 1.0)
            {
                GameManager.Instance.musicMixer.SetFloat("MusicVol", Mathf.Lerp(startVol, SettingsManager.Instance.musicVolumeSlider.value, progress));
                progress += rate * Time.deltaTime;

                yield return null;
            }

            currentTrack.volume = 1f;
            fadingIn = false;
        }
    }
}

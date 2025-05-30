using UnityEngine;

public class PersistentManager : MonoBehaviour
{
	public static PersistentManager Instance;

    public AudioSource musicSource;

    private float _musicVolume = 1;
    private bool _musicMute = false;

    public void ChangeScene ()
	{
        SetMusicVolume(_musicVolume);
        SetMusicMute(_musicMute);
        RestartSound();
    }

    public float GetMusicVolume ()
	{
        return _musicVolume;
	}

    public bool GetMusicMute ()
	{
        return _musicMute;
	}

    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;
        musicSource.volume = volume;
    }

    public void SetMusicMute (bool isMute)
	{
        _musicMute = isMute;
        musicSource.mute = isMute;
	}

    public void RestartSound ()
	{
        musicSource.Stop();
        musicSource.Play();
	}

	private void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = GetComponent<AudioSource>();
        }
        else
            Destroy(gameObject);
    }
}

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource _musicSource;
    private float _musicVolume = 1;
    private bool _musicMute = false;

    public float GetMusicVolume ()
    {
        return _musicVolume;
    }

    public bool GetMusicMute ()
    {
        return _musicMute;
    }

    public void SetMusicVolume (float volume)
    {
        _musicVolume = volume;
        _musicSource.volume = volume;
    }

    public void SetMusicMute (bool isMute)
    {
        _musicMute = isMute;
        _musicSource.mute = isMute;
    }

    public void RestartMusic ()
    {
        _musicSource.Stop();
        _musicSource.Play();
    }

    private void OnEnable ()
    {
        _musicSource = GetComponent<AudioSource>();
    }

}

using System.Collections.Generic;
using UnityEngine;

public enum SoundFxKey
{
    None,
    Click,
    Yes,
    No,
}

public class SoundManager : MonoBehaviour
{
    public AudioSource musicSound;
    public AudioSource clickSound;
    public AudioSource yesSound;
    public AudioSource noSound;

    private float _musicVolume = 1;
    private bool _musicMute = false;

    private Dictionary<SoundFxKey, AudioSource> MapFxSounds => new()
    {
        { SoundFxKey.Click, clickSound },
        { SoundFxKey.Yes, yesSound },
        { SoundFxKey.No, noSound },
    };

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
        musicSound.volume = volume;
    }

    public void SetMusicMute (bool isMute)
    {
        _musicMute = isMute;
        musicSound.mute = isMute;
    }

    public void RestartMusic ()
    {
        musicSound.Stop();
        musicSound.Play();
    }

    public void PlaySoundFX (SoundFxKey soundKey)
	{
        if (soundKey == SoundFxKey.None)
            return;

        if (MapFxSounds.TryGetValue(soundKey, out AudioSource sound))
            sound.Play();
	}

}

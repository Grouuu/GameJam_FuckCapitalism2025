using UnityEngine;

public class PersistentManager : MonoBehaviour
{
	public static PersistentManager Instance { get; private set; }

    [HideInInspector] public SoundManager soundManager;
    [HideInInspector] public SaveManager saveManager;
    [HideInInspector] public I2Manager localizationManager;

    public async Awaitable InitPersistentData ()
	{
        await InitSaveData();
        InitSounds();
    }

	private void Awake ()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
	}

	private void OnEnable ()
    {
        soundManager = GetComponent<SoundManager>();
        saveManager = GetComponent<SaveManager>();
        localizationManager = GetComponent<I2Manager>();
    }

    private async Awaitable InitSaveData ()
    {
        if (!saveManager.HasSaveLoaded())
        {
            saveManager.Init();
            await saveManager.LoadData();

            string gameVersion = saveManager.GetSaveData<string>(SaveItemKey.Version);

            // destroy game save if outdated
            if (!string.IsNullOrEmpty(gameVersion) && gameVersion != saveManager.GetGameVersion())
                await saveManager.DeleteGameSave();

            saveManager.UpdateGameVersion();
        }
    }

    private void InitSounds ()
	{
        if (saveManager.HasKey(SaveItemKey.MusicVolume))
            soundManager.SetMusicVolume(saveManager.GetSaveData<float>(SaveItemKey.MusicVolume));

        if (saveManager.HasKey(SaveItemKey.MusicMute))
            soundManager.SetMusicMute(saveManager.GetSaveData<bool>(SaveItemKey.MusicMute));
    }

}

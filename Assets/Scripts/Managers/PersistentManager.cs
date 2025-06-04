using UnityEngine;

public class PersistentManager : MonoBehaviour
{
	public static PersistentManager Instance;

    [HideInInspector] public SoundManager soundManager;
    [HideInInspector] public SaveManager saveManager;

    public async Awaitable InitPersistentData ()
	{
        await InitSaveData();
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

}

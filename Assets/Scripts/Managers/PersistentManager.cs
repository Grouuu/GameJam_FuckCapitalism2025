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
        }
    }

}

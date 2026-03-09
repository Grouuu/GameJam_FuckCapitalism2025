using Grouuu.Constants;
using UnityEngine;

namespace Grouuu.PersistentManagers
{
    public class PersistentController : MonoBehaviour
    {
        public static PersistentController Instance { get; private set; }

        [HideInInspector] public SoundManager SoundManager;
        [HideInInspector] public SaveManager SaveManager;
        [HideInInspector] public I2Manager LocalizationManager;

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
            SoundManager = GetComponent<SoundManager>();
            SaveManager = GetComponent<SaveManager>();
            LocalizationManager = GetComponent<I2Manager>();
        }

        private async Awaitable InitSaveData ()
        {
            if (!SaveManager.HasSaveLoaded())
            {
                SaveManager.Init();
                await SaveManager.LoadData();

                string gameVersion = SaveManager.GetSaveData<string>(SaveItemKey.Version);

                // destroy game save if outdated
                if (!string.IsNullOrEmpty(gameVersion) && gameVersion != SaveManager.GetGameVersion())
                    await SaveManager.DeleteGameSave();

                SaveManager.UpdateGameVersion();
            }
        }

        private void InitSounds ()
        {
            if (SaveManager.HasKey(SaveItemKey.MusicVolume))
                SoundManager.SetMusicVolume(SaveManager.GetSaveData<float>(SaveItemKey.MusicVolume));

            if (SaveManager.HasKey(SaveItemKey.MusicMute))
                SoundManager.SetMusicMute(SaveManager.GetSaveData<bool>(SaveItemKey.MusicMute));
        }

    }

}
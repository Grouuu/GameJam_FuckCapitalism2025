using Cysharp.Threading.Tasks;

namespace Grouuu.Data
{
    public class DialogResultData : ResultData
    {
        public string dialogName;
        public bool isYes;

        public static DialogResultData CreateFrom (ResultData resultData)
        {
            return new()
            {
                eventsDay = resultData.eventsDay,
                varChanges = resultData.varChanges,
            };
        }
    }

    public class DialogData
    {
        public string id;
        public string name;
        public string characterName;
        public int priority;
        public bool isRepeateable;
        public RequirementData requirements;
        public DialogResultData yesResult;
        public DialogResultData noResult;
        public EditSceneEffect enterSceneEffets;
        public EditSceneEffect exitSceneEffets;
        public EditSceneEffect yesEnterSceneEffets;
        public EditSceneEffect yesExitSceneEffets;
        public EditSceneEffect noEnterSceneEffets;
        public EditSceneEffect noExitSceneEffets;

        // runtime values
        public bool isUsed = false;

        public bool isAvailable ()
        {
            if (!IsRespectRequirements())
                return false;

            if (isUsed && !isRepeateable)
                return false;

            if (!IsResultsSafe())
                return false;

            return true;
        }

        public bool IsRespectRequirements ()
        {
            if (requirements != null && !requirements.IsOK())
                return false;

            return true;
        }

        public bool IsResultsSafe ()
        {
            return GameController.GameManagers.VarsManager.IsResultSafe(yesResult) && GameController.GameManagers.VarsManager.IsResultSafe(noResult);
        }

        public void GenerateResultValue ()
        {
            yesResult.UpdateResult();
            noResult.UpdateResult();
        }

        public UniTask UpdateEnterSceneEffects ()
        {
            return GameController.GameManagers.SceneEffectsManager.UpdateSceneEffects(enterSceneEffets);
        }

        public UniTask UpdateExitSceneEffects ()
        {
            return GameController.GameManagers.SceneEffectsManager.UpdateSceneEffects(exitSceneEffets);
        }

        public UniTask UpdateYesEnterSceneEffects ()
        {
            return GameController.GameManagers.SceneEffectsManager.UpdateSceneEffects(yesEnterSceneEffets);
        }

        public UniTask UpdateYesExitSceneEffects ()
        {
            return GameController.GameManagers.SceneEffectsManager.UpdateSceneEffects(yesExitSceneEffets);
        }

        public UniTask UpdateNoEnterSceneEffects ()
        {
            return GameController.GameManagers.SceneEffectsManager.UpdateSceneEffects(noEnterSceneEffets);
        }

        public UniTask UpdateNoExitSceneEffects ()
        {
            return GameController.GameManagers.SceneEffectsManager.UpdateSceneEffects(noExitSceneEffets);
        }
    }
}
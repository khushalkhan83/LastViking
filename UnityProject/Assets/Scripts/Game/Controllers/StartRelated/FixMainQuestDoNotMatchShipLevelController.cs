using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;

namespace Game.Controllers
{
    // this controller supposed to run after QuestSaveController and SideQuestsSaveController (located in pre init state)
    public class FixMainQuestDoNotMatchShipLevelController : IFixMainQuestDoNotMatchShipLevelController, IController
    {
        [Inject] public QuestsModel QuestsModel { get; private set; }
        [Inject] public QuestsSaveModel QuestsSaveModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public ShelterModelsProvider ShelterModelsProvider {get; private set;}
        
        private ShelterModel ShelterModel => ShelterModelsProvider[ShelterModelID.Ship];


        void IController.Enable() 
        {
            ShelterModel.Init();
            TutorialModel.Init();

            TryFixWrongShelterLevel();
        }

        void IController.Start() 
        {

        }

        void IController.Disable() 
        {
        }

        private void TryFixWrongShelterLevel()
        {
            if (!TutorialModel.IsComplete) return;

            var chapter = QuestsModel.GetChapter();
            var shipLevel = ShelterModel.Level;

            if (chapter == shipLevel) return;

            if (ShelterUpgradedInCutsceneButChapterNotYetChanged(chapter, shipLevel)) return;

            if(!QuestsModel.TryGetQuetsIndexByChapter(shipLevel, out int questIndex)) return;

            ResetQuest(questIndex);
        }

        private void ResetQuest(int questIndex)
        {
            QuestsModel.ActivateQuest_Internal(questIndex);
            QuestsSaveModel.SetMainQuestIndex(questIndex);

            var stageIndex = QuestsModel.k_defaultStageIndex;
            QuestsModel.ActivateStage_Internal(stageIndex);
            QuestsSaveModel.SetMainQuestStageIndex(stageIndex);
        }

        private bool ShelterUpgradedInCutsceneButChapterNotYetChanged(int chapter, int shipLevel)
        {
            return IsUpgradeCutscene() && chapter + 1 == shipLevel;
        }
        
        private bool IsUpgradeCutscene() => QuestsModel.IsLastStage;
    }
}

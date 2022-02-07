using Game.Audio;
using Game.Models;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.QuestSystem.Map.Controllers
{
    public class QuestSoundsController : MonoBehaviour
    {
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        private SideQuestsModel SideQuestsModel => ModelsSystem.Instance._sideQuestsModel;
        private MedallionsModel MedallionsModel => ModelsSystem.Instance._medallionsModel;
        private AudioSystem AudioSystem => AudioSystem.Instance;

        private void OnEnable()
        {
            QuestsModel.OnActivateStage += PlayQuestUpdatedSound;
            SideQuestsModel.OnStageChanged += PlaySideQuestUpdatedSound;
            MedallionsModel.OnCollect += OnMedallionCollect;
        }

        private void OnDisable()
        {
            QuestsModel.OnActivateStage -= PlayQuestUpdatedSound;
            SideQuestsModel.OnStageChanged -= PlaySideQuestUpdatedSound;
            MedallionsModel.OnCollect -= OnMedallionCollect;
        }

        private void PlayQuestUpdatedSound()
        {
            if(QuestsModel.StagePlayUpdateSound)
                ObjectiveCompleatedSound();
        }

        private void PlaySideQuestUpdatedSound(QuestData data)
        {
            if(SideQuestsModel.StagePlayUpdateSound(data))
                ObjectiveCompleatedSound();
        }

        private void OnMedallionCollect()
        {
            ObjectiveCompleatedSound();
        }

        private void ObjectiveCompleatedSound() => AudioSystem.PlayOnce(AudioID.ObjectiveCompleate);
    }
}
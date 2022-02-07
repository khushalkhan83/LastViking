using Core;
using Core.Controllers;
using Game.Models;
using Game.QuestSystem.Data;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class RateMeController : MonoBehaviour, IRateMeController, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public RateMeViewModel RateMeViewModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public QuestsLifecycleModel QuestsLifecycleModel { get; private set; }
        [Inject] public CinematicModel CinematicModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }

        #region Debug
        private bool BlockPopupsOnStart => EditorGameSettings.Instance.BlockPopupsOnStart;

        #endregion

        void IController.Enable()
        {
            // if(BlockPopupsOnStart) return;
            
            // QuestsLifecycleModel.OnQuestEvent += OnQuestEvent;
            // QuestsLifecycleModel.OnSideQuestEvent += OnSideQuestEvent;
            // if(TutorialModel.IsComplete) ShowRateMeView();
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            // QuestsLifecycleModel.OnQuestEvent += OnQuestEvent;
            // QuestsLifecycleModel.OnSideQuestEvent += OnSideQuestEvent;
        }

        private void OnQuestEvent(QuestsLifecycleModel.QuestEvent questEvent)
        {
            if(questEvent == QuestsLifecycleModel.QuestEvent.AfterScrollPickup  || questEvent == QuestsLifecycleModel.QuestEvent.AfterWolfJump)
            {
                ShowRateMeView();
            }
        }

        private void OnSideQuestEvent(QuestsLifecycleModel.SideQuestEvent sideQuestEvent)
        {
            if(sideQuestEvent == QuestsLifecycleModel.SideQuestEvent.AfterPalmJump 
                || sideQuestEvent == QuestsLifecycleModel.SideQuestEvent.AfterZombieJump
                || sideQuestEvent == QuestsLifecycleModel.SideQuestEvent.AfterWaterfallBomb)
            {
                ShowRateMeView();
            }
        }

        private void ShowRateMeView()
        {
            if (RateMeViewModel.IsCanShow)
            {
                StartCoroutine(ShowRateViewProcess());
            }
        }

        private IEnumerator ShowRateViewProcess()
        {
            yield return new WaitForSeconds(RateMeViewModel.DelayBeforeShowView);
            if(!PlayerHealthModel.IsDead && !CinematicModel.CinematicStarted)
            {
                ViewsSystem.Show<RateMeView>(ViewConfigID.RatePopup);
            }
        }
    }
}

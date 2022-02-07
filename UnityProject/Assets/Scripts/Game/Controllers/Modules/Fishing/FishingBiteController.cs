using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Controllers;
using Core;
using Game.Models;
using Game.Views;
using Core.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class FishingBiteController : IController, IFishingBiteController
    {
        [Inject] public FishingModel fishingModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public MiniGameStateModel minigameState { get; private set; }

        public IView View { get; private set; }

        public void Enable()
        {
            fishingModel.OnStartFishing += Fishing;
            fishingModel.OnTryHook += Hook;
            minigameState.OnStateChange += OnMiniGameStateChange;
        }

        public void Disable()
        {
            fishingModel.OnStartFishing -= Fishing;
            fishingModel.OnTryHook -= Hook;
            minigameState.OnStateChange -= OnMiniGameStateChange;
        }

        public void Start() {; }

        void OnMiniGameStateChange(bool isMinigame)
        {
            if (!isMinigame && fishingModel.state == FishingModel.FishingState.biting)
            {
                EndBiting();
                fishingModel.HookResult(false);
                minigameState.IsMinigame = false;
            }
        }

        void EndBiting()
        {
            UnsubscribeUpdate();
            fishingModel.Biting(false);
            _biteTime = 0f;
            _waitBiteTime = 0f;
            CloseView();
        }

        void Hook()
        {
            if (_biteTime > 0f)
                fishingModel.HookResult(true);
            else
            {
                fishingModel.HookResult(false);
                minigameState.IsMinigame = false;
            }

            EndBiting();
        }

        void SubsctibeUpdate()
        {
            if (!_isSubscribed)
            {
                GameUpdateModel.OnUpdate += UpdateFishing;
                _isSubscribed = true;
            }
        }
        bool _isSubscribed = false;
        void UnsubscribeUpdate()
        {
            if (_isSubscribed)
            {
                GameUpdateModel.OnUpdate -= UpdateFishing;
                _isSubscribed = false;
            }
        }


        void Fishing(bool isFishing)
        {
            if (isFishing)
            {
                StartBitingWait();
                SubsctibeUpdate();
                OpenView();
                minigameState.IsMinigame = true;                
            }
        }

        void StartBitingWait()
        {
            _waitBiteTime = fishingModel.biteMinimalWaitTime + Random.value * (fishingModel.biteMaximalWaitTime - fishingModel.biteMinimalWaitTime);
            fishingModel.Biting(false);
        }

        float _waitBiteTime = 0f;
        float _biteTime = 0f;

        void UpdateFishing()
        {
            if (_waitBiteTime > 0f)
            {
                _waitBiteTime -= Time.deltaTime;
                if (_waitBiteTime < 0)
                {
                    _biteTime = fishingModel.biteTime;
                    fishingModel.Biting(true);
                }
            }
            else
            {
                _biteTime -= Time.deltaTime;
                if (_biteTime < 0f)
                {
                    StartBitingWait();
                }
            }
        }

        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<FishingBitingView>(ViewConfigID.FishingBiting);
                View.OnHide += OnHideHandler;
            }
        }

        private void CloseView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }

        
    }
}

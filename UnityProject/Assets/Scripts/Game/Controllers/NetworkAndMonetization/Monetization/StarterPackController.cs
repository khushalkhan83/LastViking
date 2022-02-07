using System;
using System.Collections;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine; // remove

namespace Game.Controllers
{
    public class StarterPackController : IStarterPackController, IController
    {
        [Inject] public StarterPackModel StarterPackModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public RealTimeModel RealTimeModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public CinematicModel CinematicModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private int waitForShowViewCoroutineID = -1;
        
        #region Debug
        private bool BlockPopupsOnStart => EditorGameSettings.Instance.BlockPopupsOnStart;

        #endregion

        void IController.Enable()
        {
            // StorageModel.TryProcessing(StarterPackModel._Data);
            // StarterPackModel.SetRemainOfferTime(StarterPackModel.OfferDuration);
        }

        void IController.Start()
        {
            // if (!StarterPackModel.IsPackAvailable) return;

            // if(BlockPopupsOnStart) return;

            // StarterPackModel.OnOfferEnded += OnOfferEnded;
            // StarterPackModel.OnPackBought += OnPackBought;

            // if (TutorialModel.IsComplete)
            // {
            //     MainLogic();
            // }
            // else
            // {
            //     TutorialModel.OnComplete += OnTutorialComplete;
            // }
        }

        void IController.Disable()
        {
            // GameUpdateModel.OnUpdate -= OnUpdate;
            // TutorialModel.OnComplete -= OnTutorialComplete;
            // RealTimeModel.OnTimeReady -= OnRealtimeReady;
            // RealTimeModel.OnTimeReady -= OnRealtimeReadyInit;
            // RealTimeModel.OnTimeError -= OnErrorRealtimeHandler;
            // StarterPackModel.OnOfferEnded -= OnOfferEnded;
            // GameTimeModel.OnChangeDay -= OnChangeDay;
            // ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.InventoryPlayer, OnHideInventory);
            // StarterPackModel.OnPackBought -= OnPackBought;
        }

        private void MainLogic()
        {
            if (StarterPackModel.CurrentOfferStatus == StarterPackModel.OfferStatus.Available)
            {
                StartStarterPack();
            }
            else if (StarterPackModel.CurrentOfferStatus == StarterPackModel.OfferStatus.WaitForAvailable)
            {
                WaitForInitStarterPack();
            }
        }

        private void OnTutorialComplete()
        {
            TutorialModel.OnComplete -= OnTutorialComplete;
            MainLogic();
        }

        private void WaitForInitStarterPack()
        {
            RealTimeModel.OnTimeReady += OnRealtimeReadyInit;
            RealTimeModel.DropTime();
        }

        private void OnPackBought()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            TutorialModel.OnComplete -= OnTutorialComplete;
            RealTimeModel.OnTimeReady -= OnRealtimeReady;
            RealTimeModel.OnTimeReady -= OnRealtimeReadyInit;
            StarterPackModel.OnOfferEnded -= OnOfferEnded;
            GameTimeModel.OnChangeDay -= OnChangeDay;
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.InventoryPlayer, OnHideInventory);
            StarterPackModel.OnPackBought -= OnPackBought;
        }

        private void StartUpdate() => GameUpdateModel.OnUpdate += OnUpdate;
        private void StopUpdate() => GameUpdateModel.OnUpdate -= OnUpdate;

        private void TrySubscribeChangeDay()
        {
            if (!StarterPackModel.HasRepeatedShown)
            {
                GameTimeModel.OnChangeDay += OnChangeDay;
            }
        }

        private void ShowBeforeEnd()
        {
            StarterPackModel.StartCoroutine(ShowBeforeEndCoroutine());
        }

        private IEnumerator ShowBeforeEndCoroutine()
        {
            yield return new WaitForSeconds(1f);

            // Dont start ShowBeforeEndCoroutine at the game start.
            if(StarterPackModel.RemainOfferTime < StarterPackModel.BeforeEndTimeSec + 60)
            {
                yield break;
            }

            while(StarterPackModel.RemainOfferTime > StarterPackModel.BeforeEndTimeSec)
            {
                yield return new WaitForSeconds(5f);
            }

            StartWaitForShowView();
        }

        private void UnsubscribeChangeDay() => GameTimeModel.OnChangeDay -= OnChangeDay;

        private void OnUpdate() => StarterPackModel.UpdateTime(Time.unscaledDeltaTime);

        private void OnRealtimeReadyInit()
        {
            RealTimeModel.OnTimeReady -= OnRealtimeReadyInit;
            InitStarterPack();
        }

        private void InitStarterPack()
        {
            RealTimeModel.OnTimeReady += OnRealtimeReady;
            RealTimeModel.OnTimeError += OnErrorRealtimeHandler;

            StartWaitForShowView(() => StarterPackModel.ShowStarterPackPopupFirstTime());

            TrySubscribeChangeDay();

            UpdateEndTime();

            UpdateRemainTime();

            StartUpdate();
        }

        private void StartStarterPack()
        {
            RealTimeModel.OnTimeReady += OnRealtimeReady;
            RealTimeModel.OnTimeError += OnErrorRealtimeHandler;
            if(StarterPackModel.NextSessionShown == false)
            {
               StartWaitForShowView(() => StarterPackModel.ShowStarterPackPopupNextSession());
            }
            TrySubscribeChangeDay();
            ShowBeforeEnd();
            UpdateRemainTime();
            StartUpdate();
        }

        private void OnRealtimeReady()
        {
            StopUpdate();
            StartUpdate();
            UpdateRemainTime();
        }

        private void OnErrorRealtimeHandler(string msg)
        {
            StopUpdate();
        }

        private void UpdateEndTime()
        {
            var offerTicks = GameTimeModel.GetTicks(StarterPackModel.OfferDuration);
            StarterPackModel.EndOfferTime = RealTimeModel.Now().Ticks + offerTicks;
        }

        private void UpdateRemainTime()
        {
            var remainTicks = StarterPackModel.EndOfferTime - RealTimeModel.Now().Ticks;
            var remainSeconds = GameTimeModel.GetSecondsTotal(remainTicks);
            StarterPackModel.SetRemainOfferTime(remainSeconds);
        }
        
        private void OnChangeDay()
        {
            if ((int)GameTimeModel.DayLast + 1 >= StarterPackModel.RepeatedOfferDay)
            {
                UnsubscribeChangeDay();
                PrepareForRepeatShow();
            }
        }

        private void PrepareForRepeatShow()
        {
            ViewsSystem.OnEndHide.AddListener(ViewConfigID.InventoryPlayer, OnHideInventory);
        }

        private void OnHideInventory()
        {
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.InventoryPlayer, OnHideInventory);
            RepeatShowStarterPackPopup();
        }
        
        private void RepeatShowStarterPackPopup()
        {
            StarterPackModel.ShowStarterPackPopupSecondTime();
            ViewsSystem.Show<StarterPackPopupView>(ViewConfigID.StarterPackPopupView);
        }

        private void StartWaitForShowView(Action onShowView = null)
        {
            CoroutineModel.BreakeCoroutine(waitForShowViewCoroutineID);
            waitForShowViewCoroutineID = CoroutineModel.InitCoroutine(WaitForShowView(onShowView));
        }
        
        private IEnumerator WaitForShowView(Action onShowView = null)
        {
            while(true)
            {
                yield return new WaitForSeconds(2f);
                if (!IsWindowOrPopupOpen() && !CinematicModel.CinematicStarted)
                {
                    if (StarterPackModel.CurrentOfferStatus != StarterPackModel.OfferStatus.Bought 
                        && StarterPackModel.CurrentOfferStatus != StarterPackModel.OfferStatus.Expired)
                    {
                        ViewsSystem.Show<StarterPackPopupView>(ViewConfigID.StarterPackPopupView);
                        if(onShowView != null) onShowView?.Invoke();
                    }
                    break;
                }
            }
        }

        private void OnOfferEnded()
        {
            StopUpdate();
        }

        private bool IsWindowOrPopupOpen() 
        {
            foreach (var views in ViewsSystem.ActiveViews.Values) {
                foreach (var view in views) {
                    if (ViewsSystem.Configs.TryGetValue(view, out var configData))
                    {
                        if (StarterPackModel.CanShowOverViews.Contains(configData.ViewID))
                        {
                            continue;
                        }

                        LayerID id = configData.LayerID;
                        if (id == LayerID.Window || id == LayerID.Popup)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}

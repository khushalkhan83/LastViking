using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Controllers;
using Core;
using Game.Models;
using Game.Views;
using Core.Views;
using UltimateSurvival;
using System;
using Game.Providers;

namespace Game.Controllers
{
    /* plase anybody split this class (and 100500 other) to different ones */

    public class TeasureHuntTimerController : IController, ITeasureHuntTimerController
    {
        [Inject] public TreasureHuntModel huntModel { get; private set; }
        //[Inject] public RealTimeModel realTImeModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        public void Disable()
        {
            huntModel.OnTimeForCheckWait -= CheckIsTimeToStart;
            huntModel.OnBeginDigMode -= StartDigModeHandler;
            huntModel.OnDebugOptionChanged -= OnDebugOptionChangedHandler;
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;

            CheckDisables();
            CloseDebugView();
        }

        public void Enable()
        {
            huntModel.OnTimeForCheckWait += CheckIsTimeToStart;
            huntModel.OnBeginDigMode += StartDigModeHandler;
            huntModel.OnDebugOptionChanged += OnDebugOptionChangedHandler;
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
            ChechEnables();

            if (huntModel.IsDebug)
            {
                OpenViewDebug();
            }
        }

        void CheckDisables()
        {
            if (huntModel.state == TreasureHuntState.wait)
            {
                huntModel.ResetAlarm();
            }
            if (huntModel.state == TreasureHuntState.digging)
            {
                huntModel.ResetAlarm();
            }
        }

        void ChechEnables()
        {
            if (huntModel.state == TreasureHuntState.wait)
            {
                CheckIsTimeToStart();
            }
            else if (huntModel.state == TreasureHuntState.digging)
            {
                CheckDiggingTimer();
            }
        }

        private void OnRevivalPrelimHandler() => UpdateViewsVisible();
        private void OnRevivalHandler() => UpdateViewsVisible();
        private void OnDebugOptionChangedHandler() => UpdateViewsVisible();

        private void UpdateViewsVisible()
        {
            if (huntModel.IsDebug)
            {
                OpenViewDebug();
            }
            else
            {
                CloseDebugView();
            }
        }

        bool isTimeReady => true; // realTImeModel.isReady
        DateTime nowDateTime => DateTime.Now; // realTImeModel.Now()

        void CheckDiggingTimer()
        {
            //turn off digging timer
            // if (isTimeReady)
            // {
            //     DateTime now = nowDateTime;
            //     if (now >= huntModel.endOfDiggingDate)
            //     {
            //         huntModel.DiggingTimeOut(now);
            //         huntModel.CreateWaitAlarm((float)huntModel.GetEndOfWaitDate(now).Subtract(now).TotalSeconds);
            //     }
            //     else
            //     {
            //         huntModel.CreateWaitAlarm((float)huntModel.endOfDiggingDate.Subtract(now).TotalSeconds);
            //     }
            // }
        }

        void CheckIsTimeToStart()
        {
            if (isTimeReady)
            {
                if (huntModel.state == TreasureHuntState.wait)
                {
                    if (IsEndOfWait())
                    {
                        huntModel.StartReciveBottleMode();
                    }
                    else
                    {
                        DateTime now = nowDateTime;
                        DateTime endWaitTim = huntModel.GetEndOfWaitDate(now);
                        float waitTime = (float)endWaitTim.Subtract(now).TotalSeconds;
                        huntModel.CreateWaitAlarm(waitTime);
                    }
                }
                else if (huntModel.state == TreasureHuntState.digging)
                {
                    CheckDiggingTimer();
                }
            }
        }

        bool IsEndOfWait()
        {
            if (!isTimeReady)
                return false;

            DateTime now = nowDateTime;
            DateTime endWaitTim = huntModel.GetEndOfWaitDate(now);
            return (now > endWaitTim);
        }

        public void Start()
        {
        }

        void StartDigModeHandler()
        {
            huntModel.SetDiggingTimeout(nowDateTime);
        }

        ////////////////
        ///

        public IView ViewDebug { get; private set; }
        private void OpenViewDebug()
        {
            if (ViewDebug == null)
            {
                ViewDebug = ViewsSystem.Show<TreasureHuntDebugView>(ViewConfigID.HuntDebug);
                ViewDebug.OnHide += OnHideDebugHandler;
                (ViewDebug as TreasureHuntDebugView).SetModel(huntModel);
            }
        }

        private void OnHideDebugHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            ViewDebug = null;
        }

        private void CloseDebugView()
        {
            if (ViewDebug != null)
            {
                ViewsSystem.Hide(ViewDebug);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            view = null;
        }
    }
}
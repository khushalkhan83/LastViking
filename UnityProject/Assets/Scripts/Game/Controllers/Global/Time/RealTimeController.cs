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

namespace Game.Controllers
{
    public class RealTimeController : IRealTimeController, IController
    {
        [Inject] public RealTimeModel realTimeModel { get; private set; }
        [Inject] public GameUpdateModel updateModel { get; private set; }
        [Inject] public NetworkModel networkModel { get; private set; }
        
        public void Enable()
        {
            realTimeModel.OnTimeReset += StartUpdate;
            networkModel.OnInternetConnectionStateChange += InternetStateChangeHandler;
            StartUpdate();
        }

        public void Disable()
        {
            realTimeModel.OnTimeReset -= StartUpdate;
            networkModel.OnInternetConnectionStateChange -= InternetStateChangeHandler;
        }

        public void Start() {
        }

        void InternetStateChangeHandler()
        {
            if (networkModel.IsHasConnection && !realTimeModel.isReady)
            {
                StartUpdate();
            }

            if (!networkModel.IsHasConnection && realTimeModel.isReady)
            {
                realTimeModel.DropTime();
            }
        }

        WWW www = null;
        void StartUpdate()
        {
            if (www == null)
            {
                www = new WWW(realTimeModel.URL);
                updateModel.OnUpdate += OnCheckWWW;
            }
        }


        void OnCheckWWW()
        {
            if (www.isDone)
            {
                updateModel.OnUpdate -= OnCheckWWW;
                WWW completedWWW = www;
                www = null;

                if (string.IsNullOrEmpty(completedWWW.error))
                {
                    string timeString = completedWWW.text;

                    try
                    {
                        RealTimeModel.TimeJSON tObj = JsonUtility.FromJson<RealTimeModel.TimeJSON>(timeString);
                        timeString = tObj.UTC;
                    }
                    catch(System.Exception e)
                    {
                        realTimeModel.UpdateTimeError(e.ToString());
                        return;
                    }

                    DateTime inteTime;
                    if (DateTime.TryParse(timeString, out inteTime))
                    {
                        realTimeModel.SetInternetTime(inteTime, DateTime.Now);
                    }
                    else
                    {
                        realTimeModel.UpdateTimeError("Cant parse: " + timeString);
                    }
                }
                else
                {
                    realTimeModel.UpdateTimeError(completedWWW.error);
                }
            }
        }
    }
}
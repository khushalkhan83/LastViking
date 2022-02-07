using Core.Views;
using Game.Models;
using Game.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureHuntDebugView : ViewBase
{
    [SerializeField]
    Button btnAddBottle,btnStartReciveMode,btnEndDigMode;

    [SerializeField]
    Text statusString;

    private void OnDisable()
    {
        UnSubscribe();
    }

    public void AddBottle()
    {
        huntModel.StartWaitActivateMode();
    }

    public void StartReciveMode()
    {
        huntModel.StartReciveBottleMode();
    }

    public void EndDigMode()
    {
        if (isTimeReady)
        {
            huntModel.DiggingTimeOut(nowDateTime);
        }
        else
        {
            InternetErrorView.InternetErrorMessage();
        }
    }

    TreasureHuntModel huntModel;
    //RealTimeModel realTimeModel;
    bool isTimeReady => true; // realTimeModel.isReady
    System.DateTime nowDateTime => System.DateTime.Now; // realTimeModel.Now()

    //public void SetModel(TreasureHuntModel model, RealTimeModel rtModel)
    public void SetModel(TreasureHuntModel model)
    {
        huntModel = model;
        //realTimeModel = rtModel;

        statusString.text = "Status: " + model.state.ToString();
        btnAddBottle.gameObject.SetActive(huntModel.state == TreasureHuntState.reciveBottle);
        btnStartReciveMode.gameObject.SetActive(huntModel.state == TreasureHuntState.wait);
        btnEndDigMode.gameObject.SetActive(huntModel.state == TreasureHuntState.digging);
        Subscribe();
    }

    void Refresh()
    {
        //SetModel(huntModel, realTimeModel);
        SetModel(huntModel);
    }

    bool isSubscribed = false;
    void Subscribe()
    {
        if (!isSubscribed)
        {
            isSubscribed = true;
            
            huntModel.OnBeginRecive += Refresh;
            huntModel.OnWaitActivate += Refresh;
            huntModel.OnBeginDigMode += Refresh;
            huntModel.OnDiggingTimeOut += Refresh;
        }
    }
    void UnSubscribe()
    {
        if (isSubscribed)
        {
            isSubscribed = false;

            huntModel.OnBeginRecive -= Refresh;
            huntModel.OnWaitActivate -= Refresh;
            huntModel.OnBeginDigMode -= Refresh;
            huntModel.OnDiggingTimeOut -= Refresh;
        }
    }
}

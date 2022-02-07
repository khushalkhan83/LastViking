using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Models;

public class TreasureHuntPlace : MonoBehaviour
{
    [SerializeField]
    TreasureHuntDiggingPlace[] digPoints;
    public TreasureHuntDiggingPlace[] dps => digPoints;

    [SerializeField]
    GroundDecal placeDecal;

    TreasureHuntModel huntModel;

    public void Hide()
    {
        for (int i = 0; i < digPoints.Length; i++)
        {
            digPoints[i].gameObject.SetActive(false);
        }
        Subscribe(false);
        placeDecal.gameObject.SetActive(false);
    }

    public void Show(TreasureHuntModel model)
    {
        huntModel = model;
        for (int i =0;i< digPoints.Length;i++)
        {
            digPoints[i].ShowDigged(i,huntModel);
        }
        Subscribe(true);
        //placeDecal.gameObject.SetActive(true);
    }

    bool _isSubscribed = false;
    void Subscribe(bool isSub)
    {
        if (isSub != _isSubscribed)
        {
            _isSubscribed = isSub;
            if (_isSubscribed)
            {
                huntModel.OnEnterNearZone += HuntModel_OnEnterNearZone;
            }
            else
            {
                huntModel.OnEnterNearZone -= HuntModel_OnEnterNearZone;
            }
        }
    }

    private void HuntModel_OnEnterNearZone(bool isNear, bool isShovel)
    {
        for (int i = 0; i < digPoints.Length; i++)
        {
            bool isDiged = huntModel.IsHoleUsed(i);
            digPoints[i].gameObject.SetActive(isDiged ||( isNear && isShovel));
        }

        placeDecal.gameObject.SetActive(isNear && isShovel);
    }

    public void SetVisibleOnlyPlace(TreasureHuntDiggingPlace target)
    {
        foreach(TreasureHuntDiggingPlace tp in digPoints)
        {
            tp.SetUndiggedVisible(tp == target);
        }
    }

    //
    public void BakeMesh()
    {
        if (placeDecal != null)
            placeDecal.ProjectMesh();
        foreach(TreasureHuntDiggingPlace dp in digPoints)
        {
            dp.BakeMesh();
        }
    }
}

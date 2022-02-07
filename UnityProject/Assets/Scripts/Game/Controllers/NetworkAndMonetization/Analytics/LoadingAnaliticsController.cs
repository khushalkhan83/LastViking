using System;
using Game.Models;
using UnityEngine;
using DevToDev;
using DevtodevAnalytics = DevToDev.Analytics;

namespace Game.Controllers
{
    public class LoadingAnaliticsController : MonoBehaviour
    {
        [SerializeField] private EnoughSpaceModel enoughSpaceModel;

        private EnoughSpaceModel EnoughSpaceModel => enoughSpaceModel;


        private void OnEnable()
        {
            enoughSpaceModel.OnShowNotEnoughSpaceCriticalView += OnShowNotEnoughSpaceCriticalView;
        }

        private void OnDisable()
        {
            enoughSpaceModel.OnShowNotEnoughSpaceCriticalView -= OnShowNotEnoughSpaceCriticalView;
        }

        private void OnShowNotEnoughSpaceCriticalView()
        {
            DevtodevAnalytics.CustomEvent(AnaliticEventID.NotEnoughStorageSpaceCritical.ToString());
        }
    }
}
using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class AttackSessionCompletedControllerData : IDataViewController
    {
        public string DescriptionID { get; }
        public float LifeTime { get; }

        public AttackSessionCompletedControllerData(string descriptionID, float lifeTime)
        {
            DescriptionID = descriptionID;
            LifeTime = lifeTime;
        }
    }

    public class AttackSessionCompletedController : ViewControllerBase<AttackSessionCompleteView, AttackSessionCompletedControllerData>
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        protected override void Show()
        {
            View.SetDescriptionText(Data.DescriptionID);// LocalizationModel.GetString(Data.DescriptionID));
            StartCoroutine(LifeTimeProcess());
        }

        private IEnumerator LifeTimeProcess()
        {
            yield return new WaitForSeconds(Data.LifeTime);
            NotificationContainerViewModel.EndCurrent();
        }

        protected override void Hide()
        {
            StopAllCoroutines();
        }
    }
}

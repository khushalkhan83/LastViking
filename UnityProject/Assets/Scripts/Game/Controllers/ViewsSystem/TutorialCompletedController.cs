using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialCompletedControllerData : IDataViewController
    {
        public LocalizationKeyID DescriptionID { get; }
        public float LifeTime { get; }

        public TutorialCompletedControllerData(LocalizationKeyID descriptionID, float lifeTime)
        {
            DescriptionID = descriptionID;
            LifeTime = lifeTime;
        }
    }

    public class TutorialCompletedController : ViewControllerBase<TutorialCompleteView, TutorialCompletedControllerData>
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        protected override void Show()
        {
            View.SetDescriptionText(LocalizationModel.GetString(Data.DescriptionID));
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

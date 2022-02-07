using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Controllers
{
    public class ObjectiveTimeDelayViewControllerData : IDataViewController
    {
        public string Description { get; }
        public float TargetTime { get; }
        public float LifeTime { get; }

        public ObjectiveTimeDelayViewControllerData(string description, float targetTime, float lifeTime)
        {
            Description = description;
            TargetTime = targetTime;
            LifeTime = lifeTime;
        }
    }

    public class ObjectiveTimeDelayViewController : ViewControllerBase<ObjectiveTimeDelayView, ObjectiveTimeDelayViewControllerData>
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }

        protected override void Show()
        {
            View.SetDescriptionText(Data.Description);
            GameUpdateModel.OnUpdate += OnUpdate;
            StartCoroutine(LifeTimeProcess());
        }

        private string TimeToString(float time) => System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss");

        private void OnUpdate()
        {
            var leftTime = Data.TargetTime - GameTimeModel.EnviroTimeOfDay;
            View.SetTimeText (TimeToString(60 * leftTime));
        }

        private IEnumerator LifeTimeProcess()
        {
            yield return new WaitForSeconds(Data.LifeTime);
            NotificationContainerViewModel.EndCurrent();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            StopAllCoroutines();
        }
    }
}
using Core;
using Core.Controllers;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public interface IDataViewController
    {

    }

    public class ObjectiveInfoViewControllerData : IDataViewController
    {
        public Sprite Icon { get; }
        public string Description { get; }
        public float LifeTime { get; }

        public ObjectiveInfoViewControllerData(Sprite icon, string description, float lifeTime)
        {
            Icon = icon;
            Description = description;
            LifeTime = lifeTime;
        }
    }

    public class ObjectiveInfoViewController : ViewControllerBase<ObjectiveInfoView, ObjectiveInfoViewControllerData>
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }

        protected override void Show()
        {
            View.SetObjecvtiveIcon(Data.Icon);
            View.SetDescriptionText(Data.Description);
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

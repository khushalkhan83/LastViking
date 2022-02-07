using Game.Views;
using Core.Controllers;
using ActivitiesLog.ViewControllerData;

namespace Game.Controllers
{
    public class ActivitiesLogViewController : ViewControllerBase<ActivitiesLogView,ActivitiesViewData>
    {
        protected override void Show() 
        {
            Data.OnDataChanged += PresentData;

            Data.Fetch();
            View.ShowContentAnimation();
            View.OnCloseClick += Data.CloseViewRequest;
        }

        protected override void Hide() 
        {
            Data.OnDataChanged -= PresentData;
            View.OnCloseClick -= Data.CloseViewRequest;
        }

        private void PresentData()
        {
            View.PrepareEnoughViews(Data.ViewDatas.Count);
            
            foreach (var enterence in View.ActivityLogEnterences)
            {
                enterence.gameObject.SetActive(false);
            }
            for (int i = 0; i < Data.ViewDatas.Count; i++)
            {
                var data = Data.ViewDatas[i];
                var enterence = View.ActivityLogEnterences[i];

                enterence.Desciption = data.description;
                enterence.Icon = data.icon;
                enterence.IconScale = data.iconScale;
                enterence.TextBacgroundColor = data.textBackgroundColor;
                enterence.IconBackgroundColor = data.iconBackgroundColor;
                enterence.gameObject.SetActive(true);
            }
        }
    }
}

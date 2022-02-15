using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ViewsStateController : IViewsStateController, IController
    {
        [Inject] public ViewsStateModel ViewsStateModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public ViewsMapper ViewsMapper { get; private set; }

        private List<ViewConfigID> WindowViewsIds  {get; set;}
        private List<ViewConfigID> PopupViewsIds  {get; set;}

        void IController.Enable()
        {
            WindowViewsIds = GetWindowsConfigs();
            PopupViewsIds = GetPopupsConfigs();

            foreach (var windowViewId in WindowViewsIds)
            {
                ViewsSystem.OnBeginShow.AddListener(windowViewId,OnBeginShowWindow);
                ViewsSystem.OnBeginHide.AddListener(windowViewId,OnBeginHideWindow);
            }

            foreach (var windowViewId in PopupViewsIds)
            {
                ViewsSystem.OnBeginShow.AddListener(windowViewId,OnBeginShowPopup);
                ViewsSystem.OnBeginHide.AddListener(windowViewId,OnBeginHidePopup);
            }

        }

        void IController.Start() 
        {
            
        }

        void IController.Disable() 
        {
            foreach (var windowViewId in WindowViewsIds)
            {
                ViewsSystem.OnBeginShow.RemoveListener(windowViewId,OnBeginShowWindow);
                ViewsSystem.OnBeginHide.RemoveListener(windowViewId,OnBeginHideWindow);
            }

            foreach (var windowViewId in PopupViewsIds)
            {
                ViewsSystem.OnBeginShow.RemoveListener(windowViewId,OnBeginShowPopup);
                ViewsSystem.OnBeginHide.RemoveListener(windowViewId,OnBeginHidePopup);
            }

        }

        private void OnBeginHideWindow()
        {
            if(ViewsStateModel.OpenedWindowsCount > 0) ViewsStateModel.SetOpenWindowsCount(ViewsStateModel.OpenedWindowsCount - 1);
        }
        private void OnBeginShowWindow()
        {
            ViewsStateModel.SetOpenWindowsCount(ViewsStateModel.OpenedWindowsCount + 1);
        }
        private void OnBeginHidePopup()
        {
            if(ViewsStateModel.OpenedPopupsCount > 0) ViewsStateModel.SetOpenPopupsCount(ViewsStateModel.OpenedPopupsCount - 1);
        }
        private void OnBeginShowPopup()
        {
            ViewsStateModel.SetOpenPopupsCount(ViewsStateModel.OpenedPopupsCount + 1);
        }


        private List<ViewConfigID> GetWindowsConfigs() => GetConfigsByLayerExeptIgnoredViews(LayerID.Window);
        private List<ViewConfigID> GetPopupsConfigs() => GetConfigsByLayerExeptIgnoredViews(LayerID.Popup);

        private List<ViewConfigID> GetConfigsByLayerExeptIgnoredViews(LayerID layerID)
        {
            List<ViewConfigID> answer = new List<ViewConfigID>();

            var ids = Helpers.EnumsHelper.GetValues<ViewConfigID>().ToList();
            ids.Remove(ViewConfigID.None);
            
            foreach (var id in ids)
            {
                var temp = ViewsMapper[id];
                if(temp.LayerID != layerID) continue;
                if(ViewsStateModel.CanShowOverViews.Contains(temp.ViewID)) continue;

                answer.Add(id);
            }

            return answer;
        }
    }
}

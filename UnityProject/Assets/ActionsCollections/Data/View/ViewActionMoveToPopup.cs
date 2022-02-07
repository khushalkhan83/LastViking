using ActionsCollections;
using Game.Views;
using UnityEngine;
using System.Linq;
using CustomeEditorTools;
using System.Collections.Generic;

namespace DebugActions
{
    public class ViewActionMoveToPopup : ActionBase
    {
        private ViewsSystem ViewsSystem => ViewsSystem.Instance; 

        private string _operationName;

        public ViewActionMoveToPopup(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            bool error = !ViewsSystem.ActiveViews.TryGetValue(ViewConfigID.DebugTime,out var views);

            if(error) return;

            var view = views.FirstOrDefault() as DebugTimeView;

            if(view == null) return;

            var popupsLayer = GameObjectsUtil.GetObjectsByFilter(ViewsSystem.gameObject,new List<string>() {"Popups"}).FirstOrDefault();

            if(popupsLayer == null) return;

            view.transform.SetParent(popupsLayer.transform);
            view.transform.SetAsLastSibling();
            view.transform.localPosition = view.transform.localPosition - new Vector3(1200,-100,0);
        }
    }
}
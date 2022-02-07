using System;

namespace Game.Views
{
    public class ViewConfigData
    {
        public LayerID LayerID { get; }
        public Type View { get; }
        public Type Controller { get; }
        public ViewID ViewID { get; }
        public ViewConfigID ViewConfigID { get; }

        public ViewConfigData(ViewConfigID viewConfigID, ViewID viewID, Type view, Type controller = default, LayerID layerID = default)
        {
            ViewID = viewID;
            ViewConfigID = viewConfigID;
            LayerID = layerID;
            View = view;
            Controller = controller;
        }
    }
}

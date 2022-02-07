using System;
using Core;
using Core.Controllers;
using Core.Views;
using Game.Data;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class SpesialMessagesController : ISpesialMessagesController, IController
    {
        [Inject] public SpesialMessagesModel SpesialMessagesModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem {get; private set; }

        void IController.Enable() 
        {
            SpesialMessagesModel.OnRecivedItem += OnRecivedItem;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            SpesialMessagesModel.OnRecivedItem -= OnRecivedItem;
        }

        private void OnRecivedItem(RecivedItemMessageData data)
        {
            var view = ViewsSystem.Show(ViewConfigID.RecivedQuestItemConfig,data);
            view.OnHide += OnHide;
        }

        private void OnHide(IView view)
        {
            view.OnHide -= OnHide;
            ViewsSystem.Hide(view);
        }
    }
}

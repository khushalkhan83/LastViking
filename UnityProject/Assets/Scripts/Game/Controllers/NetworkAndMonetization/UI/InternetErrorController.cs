using Core.Controllers;
using Core.Views;
using Core;
using Game.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Controllers
{
    public class InternetErrorController : IInternetErrorController, IController
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        public void Disable()
        {
            CloseView();
        }

        public void Enable()
        {
            OpenView();
        }

        public void Start()
        {
        }

        public IView View { get; private set; }
        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<InternetErrorView>(ViewConfigID.InternerError);
                View.OnHide += OnHideHandler;
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }

        private void CloseView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }
    }
}
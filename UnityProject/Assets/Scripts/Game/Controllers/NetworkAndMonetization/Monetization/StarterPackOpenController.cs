using System;
using System.Collections;
using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class StarterPackOpenController : IStarterPackOpenController, IController
    {
        [Inject] public StarterPackModel StarterPackModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        IView View { get; set; }

        void IController.Enable()
        {
        }

        void IController.Start()
        {
            // if (!StarterPackModel.IsPackAvailable) return;

            // PlayerDeathModel.OnRevival += OnRevivalHandler;
            // PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
            // StarterPackModel.OnOfferEnded += OnOfferEnded;
            // StarterPackModel.OnPackBought += OnPackBought;

            // if (StarterPackModel.CurrentOfferStatus == StarterPackModel.OfferStatus.Available)
            // {
            //     UpdateViewVisible();
            // }
            // else
            // {
            //     StarterPackModel.OnShowStarterPackPopupFirstTime += OnShowFirst;
            // } 
        }

        void IController.Disable()
        {
            // Unsubscribe();
            // HideView();
        }

        private void Unsubscribe()
        {
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;
            StarterPackModel.OnOfferEnded -= OnOfferEnded;
            StarterPackModel.OnPackBought -= OnPackBought;
        }

        private void OnShowFirst()
        {
            StarterPackModel.OnShowStarterPackPopupFirstTime -= OnShowFirst;
            UpdateViewVisible();
        }

        private void OnRevivalPrelimHandler() => UpdateViewVisible();
        private void OnRevivalHandler() => UpdateViewVisible();

        private bool IsCanShow => !PlayerHealthModel.IsDead && StarterPackModel.IsPackAvailable;

        private void OnPackBought()
        {
            Unsubscribe();
            HideView();
        }

        private void OnOfferEnded()
        {
            Unsubscribe();
            HideView();
        }

        private void UpdateViewVisible()
        {
            if (IsCanShow)
            {
                ShowView();
            }
            else
            {
                HideView();
            }
        }

        private void ShowView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<StarterPackIconView>(ViewConfigID.StarterPackIconView);
                View.OnHide += OnHideHandler;
            }
        }

        private void HideView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }
    }
}

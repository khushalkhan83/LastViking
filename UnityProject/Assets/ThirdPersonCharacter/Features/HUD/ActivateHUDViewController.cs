using System;
using Game.Models;
using UnityEngine;

namespace Game.ThirdPerson.HUD
{
    public class ActivateHUDViewController : MonoBehaviour
    {
        private HUDViewModel ViewModel;

        private CinematicModel CinematicModel => ModelsSystem.Instance._cinematicModel;
        private PlayerDeathModel PlayerDeathModel => ModelsSystem.Instance._playerDeathModel;
        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;

        private bool IsCanShow => !CinematicModel.CinematicStarted && !PlayerHealthModel.IsDead;

        private void Awake()
        {
            ViewModel = GetComponent<HUDViewModel>();
        }

        private void OnEnable()
        {
            CinematicModel.OnStartCinematic += UpdateView;
            CinematicModel.OnEndCinematic += UpdateView;
            PlayerHealthModel.OnChangeHealth += UpdateView;

            PlayerDeathModel.OnRevival += UpdateView;
            PlayerDeathModel.OnRevivalPrelim += UpdateView;

            UpdateView();
        }

        private void OnDisable()
        {
            CinematicModel.OnStartCinematic -= UpdateView;
            CinematicModel.OnEndCinematic -= UpdateView;
            PlayerHealthModel.OnChangeHealth -= UpdateView;

            PlayerDeathModel.OnRevival -= UpdateView;
            PlayerDeathModel.OnRevivalPrelim -= UpdateView;

            ViewModel.SetShow(false);
        }

        private void UpdateView()
        {
            ViewModel.SetShow(IsCanShow);
        }
    }
}
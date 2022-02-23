using System.Collections;
using System.Collections.Generic;
using Game.Models;
using Game.Views;
using UnityEngine;

public class RunViewController : MonoBehaviour
{
        [SerializeField] private PlayerRunModel PlayerRunModel;
        [SerializeField] private RunButtonView View;

        private void OnEnable()
        {
            View.OnPointerDown_ += OnPointerDownHandler;
            PlayerRunModel.OnChangeIsRunToggle += OnChangeRunHandler;

            UpdateIcon();
        }

        private void OnDisable()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            PlayerRunModel.OnChangeIsRunToggle -= OnChangeRunHandler;
        }

        private void OnChangeRunHandler()
        {
            UpdateIcon();
        }

        private void OnPointerDownHandler()
        {
            if (PlayerRunModel.IsRunToggle)
            {
                PlayerRunModel.RunStop();
                PlayerRunModel.RunTogglePassive();
            }
            else
            {
                PlayerRunModel.RunStart();
                PlayerRunModel.RunToggleActive();
            }
        }

        private void UpdateIcon()
        {
            if (PlayerRunModel.IsRunToggle)
            {
                View.SetIcon(View.IconActive);
            }
            else
            {
                View.SetIcon(View.IconDefault);
            }
        }
}

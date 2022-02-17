using Core;
using Core.Views;
using Game.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class LearnInputView : ViewBase
    {
        private LocalizationModel LocalizationModel = ModelsSystem.Instance._localizationModel;
        #region Data
#pragma warning disable 0649
        [SerializeField] private List<GameObject> steps;
        [SerializeField] private Button clickButton;
        [SerializeField] private Text JoystickText;
        [SerializeField] private Text AttackText;
        [SerializeField] private Text JumpText;
        [SerializeField] private Text RunText;
        [SerializeField] private Text SwipeText;
        [SerializeField] private Text TapText;
        [SerializeField] private Text TaskText;
#pragma warning restore 0649
        #endregion

        public List<GameObject> Steps => steps;

        public event Action OnClick;

        private void OnEnable()
        {
            UpdateLocalization();
            clickButton.onClick.AddListener(() => OnClick?.Invoke());
        }

        private void OnDisable()
        {
            clickButton.onClick.RemoveAllListeners();
        }

        void UpdateLocalization()
        {
            JoystickText.text = LocalizationModel.GetString(LocalizationKeyID.Tutorial_Move);
            AttackText.text = LocalizationModel.GetString(LocalizationKeyID.Tutorial_Attack);
            JumpText.text = LocalizationModel.GetString(LocalizationKeyID.Tutorial_Jump);
            RunText.text = LocalizationModel.GetString(LocalizationKeyID.Tutorial_Run);
            SwipeText.text = LocalizationModel.GetString(LocalizationKeyID.Tutorial_Swipe);
            TapText.text = LocalizationModel.GetString(LocalizationKeyID.Tutorial_Tap_Attack);
            TaskText.text = LocalizationModel.GetString(LocalizationKeyID.Tutorial_Missions);
        }

    }
}
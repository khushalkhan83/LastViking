using System.Collections;
using System.Collections.Generic;
using Game.Models;
using Game.Views;
using UnityEngine;

public class StaminaViewController : MonoBehaviour
{
    [SerializeField] private PlayerStaminaModel PlayerStaminaModel;
    [SerializeField] private StaminaBarView View;

        void OnEnable()
        {
            PlayerStaminaModel.OnChangeStamina += OnChangeStaminaHandler;

            View.SetAmount(PlayerStaminaModel.Stamina / PlayerStaminaModel.StaminaMax);
        }

        void OnDisable()
        {
            PlayerStaminaModel.OnChangeStamina -= OnChangeStaminaHandler;
        }

        private void OnChangeStaminaHandler() => View.SetAmount(PlayerStaminaModel.Stamina / PlayerStaminaModel.StaminaMax);
}

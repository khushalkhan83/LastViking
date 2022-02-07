using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerStaminaModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredFloat Stamina;
            public ObscuredFloat BoostRemainingTime;

            public void SetStamina(float stamina)
            {
                Stamina = stamina;
                ChangeData();
            }

            public void SetBoostRemainingTime(float boostRemainingTime)
            {
                BoostRemainingTime = boostRemainingTime;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private ObscuredFloat _staminaMax;
        [SerializeField] private ObscuredFloat _boostDuration;
        [SerializeField] private AnimationCurve _boostMultiplier;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public float Stamina
        {
            get => _data.Stamina;
            private set => _data.SetStamina(value);
        }

        public float BoostRemainingTime
        {
            get => _data.BoostRemainingTime;
            private set => _data.SetBoostRemainingTime(value);
        }

        public float StaminaMax => _staminaMax + EquipmentBonus;
        public AnimationCurve BoostMultiplier => _boostMultiplier;
        public float BoostDuration => _boostDuration;

        public float EquipmentBonus { get; private set; }
        public bool IsCantLoseStamina { get; private set; }

        public ObscuredBool IsHasStamina => Stamina > 0f;
        public ObscuredBool IsFullStamina => Stamina == StaminaMax;

        public event Action OnChangeStamina;
        public event Action OnBeginBoost;
        public event Action OnEndBoost;

        public StorageModel StorageModel => _storageModel;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public void SetEquipmentPercentageBonus(float percentageBonus)
        {
            EquipmentBonus = _staminaMax * (percentageBonus / 100f);
            if(Stamina > StaminaMax)
            {
                Stamina = StaminaMax;
            }
            OnChangeStamina?.Invoke();
        }

        public void CantLoseStamina(bool isCantLoseStamina)
        {
            IsCantLoseStamina = isCantLoseStamina;
        }

        public void AdjustStamina(float adjustment)
        {
            if (!IsCantLoseStamina)
            {
                Stamina = Mathf.Clamp(Stamina + adjustment, 0, StaminaMax) * StaminaMultiplier;

                OnChangeStamina?.Invoke();
            }
        }

        public bool IsBoost => BoostRemainingTime > 0;

        public float StaminaMultiplier => IsBoost ? BoostMultiplier.Evaluate(BoostRemainingTime) : 1;

        public void Boost()
        {
            BoostRemainingTime = BoostDuration;
            OnBeginBoost?.Invoke();
        }

        public void BoostProcessing(float deltaTime)
        {
            BoostRemainingTime -= deltaTime;

            if (BoostRemainingTime < 0)
            {
                BoostRemainingTime = 0;
            }

            if (BoostRemainingTime == 0)
            {
                OnEndBoost?.Invoke();
            }
        }

        internal void RefillStamina()
        {
            Stamina = StaminaMax;

            OnChangeStamina?.Invoke();
        }
    }
}

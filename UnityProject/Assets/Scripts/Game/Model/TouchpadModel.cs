using System;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public class TouchpadModel : MonoBehaviour
    {

        [Serializable]
        public class Data: DataBase, IImmortal
        {
            public float SensativityModificator = 0.5f;

            public void SetSensativityModificator(float value)
            {
                SensativityModificator = value;
                ChangeData();
            }
        }
        
        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private float _touchDeltaAttack;
        [SerializeField] private float _touchDeltaAttackContinuosly;

        [SerializeField] private float _angleVerticalLookDown;
        [SerializeField] private float _angleVerticalLookUp;

        [SerializeField] private float _sensivity;
        [SerializeField] private float _minimalSensivity;
        [SerializeField] private float _smooth;

        [Range(0, 15)]
        [SerializeField] private int _smoothAccuracy;

#pragma warning restore 0649
        #endregion
        
        public Data _Data => _data;
        public float TouchDeltaAttack => _touchDeltaAttack;
        public float TouchDeltaAttackContinuosly => _touchDeltaAttackContinuosly;
        public float Sensivity => _minimalSensivity +  (_sensivity - _minimalSensivity) * SensativityModificator;
        public float Smooth => _smooth;
        public float AngleVerticalLookDown => _angleVerticalLookDown;
        public float AngleVerticalLookUp => _angleVerticalLookUp;
        public int SmoothAccuracy => _smoothAccuracy;

        public event Action OnSensativityModificatorChanged;
        public float SensativityModificator
        {
            get => _data.SensativityModificator;
            private set => _data.SetSensativityModificator(value);
        }

        private Vector2 _axes;
        public Vector2 Axes { get => _blockRotaiton ? Vector2.zero : _axes; private set => _axes = value; }

        private bool _blockRotaiton;
        public void SetBlockRotation(bool value)
        {
            _blockRotaiton = value;
        }

        public event Action OnInteraction;

        public void SetAxes(Vector2 axes)
        {
            Axes = axes;
        }

        public void SetDefaultAxes()
        {
            SetAxes(Vector2.zero);
        }

        public void Interaction() => OnInteraction?.Invoke();


        public void SetSensativityModificator(float value)
        {
            SensativityModificator = value;
            OnSensativityModificatorChanged?.Invoke();
        }
    }
}

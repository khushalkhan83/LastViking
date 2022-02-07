using Game.Models;
using System;
using UnityEngine;

namespace Game.Controllers
{
    public class HealthChangerViewController : MonoBehaviour
    {
        [Serializable]
        public class Data
        {
            #region Data
#pragma warning disable 0649

            [Range(0.0f, 1.0f)]
            [SerializeField] private float _percent;

            [SerializeField] private GameObject _gameObject;

#pragma warning restore 0649
            #endregion

            public float Percent => _percent;

            public GameObject GameObject => _gameObject;
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data[] _levels;
        [SerializeField] private BuildingHealthModel _buildingHealthModel;
        [SerializeField] private Transform _container;

#pragma warning restore 0649
        #endregion

        public Data[] Levels => _levels;
        public BuildingHealthModel BuildingHealthModel => _buildingHealthModel;
        public Transform Container => _container;

        public GameObject ViewCurrent { get; private set; }
        public GameObject ViewRefCurrent { get; private set; }

        public float Percent => BuildingHealthModel.Health / BuildingHealthModel.HealthMax;

        private void OnEnable()
        {
            BuildingHealthModel.OnChangeHealth += OnChangeHealth;
        }

        private void OnDisable()
        {
            BuildingHealthModel.OnChangeHealth -= OnChangeHealth;
        }

        private void OnChangeHealth() => UpdateView();

        private void Start() => UpdateView();

        public void UpdateView() => SetView(Percent);

        public void SetView(float percent)
        {
            if (percent > 0)
            {
                if (TryGetViewRef(percent, out var levelRef))
                {
                    if (levelRef != ViewRefCurrent)
                    {
                        ViewRefCurrent = levelRef;

                        if (ViewCurrent)
                        {
                            Destroy(ViewCurrent);
                        }
                        ViewCurrent = Instantiate(levelRef, Container);
                    }
                }
            }
            else
            {
                if (ViewCurrent)
                {
                    Destroy(ViewCurrent);
                }
            }
        }

        private bool TryGetViewRef(float percent, out GameObject obj)
        {
            obj = null;

            var max = -1f;
            foreach (var level in Levels)
            {
                if (level.Percent > max && level.Percent <= percent)
                {
                    max = level.Percent;
                    obj = level.GameObject;
                }
            }

            return obj != null;
        }
    }
}

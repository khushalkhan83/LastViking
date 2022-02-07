using Core;
using Core.Storage;
using Game.Models;
using System;
using System.Collections.Generic;
using UltimateSurvival.Building;
using UnityEngine;

namespace UltimateSurvival
{
    [Serializable]
    public class PlayerEventHandler : EntityEventHandler
    {
        [Serializable]
        public class Data : DataBase
        {
            public float MovementSpeedFactor;
            public Vector3 SpawnPosition;
            public Vector3 Position;
            public Quaternion SpawnRotation;
            public Quaternion Rotation;

            public void SetPosition(Vector3 pos)
            {
                Position = pos;
                ChangeData();
            }
            public void SetRotation(Quaternion rot)
            {
                Rotation = rot;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private ImpactReceiver _impactReceiver;

#pragma warning restore 0649
        #endregion

        public Vector3 SpawnPosition
        {
            get
            {
                return _data.SpawnPosition;
            }
            protected set
            {
                _data.SpawnPosition = value;
                _data.ChangeData();
            }
        }

        public Quaternion SpawnRotation
        {
            get
            {
                return _data.SpawnRotation;
            }
            protected set
            {
                _data.SpawnRotation = value;
                _data.ChangeData();
            }
        }

        public Vector3 Position
        {
            get
            {
                return _data.Position;
            }
            protected set
            {
                _data.SetPosition(value);
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return _data.Rotation;
            }
            protected set
            {
                _data.SetRotation(value);
            }
        }

        public bool PlayerAttackBuilding {get;set;} = false;
        public bool PlayerShootSomething {get;set;} = false;
        public bool CanUseWeapon {get;set;} = false;

        public float MovementSpeedFactor
        {
            get => _data.MovementSpeedFactor;
            private set => _data.MovementSpeedFactor = value;
        }

        public void SetMovementSpeedFactor(float movementSpeedFactor)
        {
            MovementSpeedFactor = movementSpeedFactor;
        }

        public ChangedValue<Vector2> MovementInput = new ChangedValue<Vector2>(Vector2.zero);
        public ChangedValue<bool> ViewLocked = new ChangedValue<bool>(false);
        public ChangedValue<bool> InteractContinuously = new ChangedValue<bool>(false);
        public ChangedValue<Vector2> LookInput = new ChangedValue<Vector2>(Vector2.zero);
        public ChangedValue<Vector3> LookDirection = new ChangedValue<Vector3>(Vector3.zero);
        public ChangedValue<Vector3> LastSleepPosition = new ChangedValue<Vector3>(Vector3.zero);
        public ChangedValue<SavableItem> EquippedItem = new ChangedValue<SavableItem>(null);
        public ChangedValue<BuildingPiece> SelectedBuildable = new ChangedValue<BuildingPiece>(null);
        public ChangedValue<RaycastData> RaycastData = new ChangedValue<RaycastData>(null);

        public Attempt AttackOnce = new Attempt();
        public Attempt AttackContinuously = new Attempt();
        public Attempt PlaceObject = new Attempt();
        public Attempt<float> RotateObject = new Attempt<float>();
        public Attempt<SavableItem, bool> ChangeEquippedItem = new Attempt<SavableItem, bool>();

        public Activity SelectBuildable = new Activity();
        public Activity Walk = new Activity();
        public Activity Crouch = new Activity();
        public Activity Jump = new Activity();
        public Activity Aim = new Activity();

        [SerializeField] private HitZoneManager _hitZone;
        private HitZoneManager hitZone => _hitZone;

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;
        private PlayerRespawnPoints PlayerRespawnPoints => FindObjectOfType<PlayerRespawnPoints>();
        private InjectionModel InjectionModel => ModelsSystem.Instance._injectionModel;

        private bool _dataInited = false;

        // called at PreInitState
        internal void Init(bool defaultPosition)
        {
            if(_dataInited)
            {
                return;
            }
            _dataInited = true;

            InitLogic(defaultPosition);
        }

        private void InitLogic(bool defaultPosition = false)
        {
            if (StorageModel.TryProcessing(_data))
            {
                if (!defaultPosition && CheckPositionCorrectness(Position))
                {
                    transform.position = Position;
                    transform.rotation = Rotation;
                }
                else
                {
                    if(InjectionModel.SceneDependenciesInjected)
                    {
                        SetPlayerPosition();
                    }
                    else 
                        InjectionModel.OnSceneDependenciesInjected += SetPlayerPosition;
                }
            }
            else
            {
                SpawnPosition = transform.position;
                SpawnRotation = transform.rotation;
            }

            Health.SetForce(PlayerHealthModel.HealthCurrent);

            Health.OnChange += OnChangeHealth;
            PlayerHealthModel.OnChangeHealth += PlayerHealthModel_OnChangeHealth;
            StorageModel.OnPreSaveAll += OnPreSaveHandler;
        }

        #region MonoBehaviour
        private void OnEnable()
        {
            if(!_dataInited)
                InitLogic();
        }

        private void OnDisable()
        {
            Health.OnChange -= OnChangeHealth;
            PlayerHealthModel.OnChangeHealth -= PlayerHealthModel_OnChangeHealth;
            StorageModel.OnPreSaveAll -= OnPreSaveHandler;
            InjectionModel.OnSceneDependenciesInjected -= SetPlayerPosition;
        }
            
        #endregion


        private void SetPlayerPosition()
        {
            transform.position = PlayerRespawnPoints.InitPlayerPoint.position;
            transform.rotation = PlayerRespawnPoints.InitPlayerPoint.rotation;
        }

        private bool CheckPositionCorrectness(Vector3 pos) =>  (int) pos.x != 0 && (int) pos.y != 0 && (int) pos.z != 0;

        public void OnPreSaveHandler()
        {
            Position = transform.position;
            Rotation = transform.rotation;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        private void PlayerHealthModel_OnChangeHealth()
        {
            Health.OnChange -= OnChangeHealth;
            Health.SetForce(PlayerHealthModel.HealthCurrent);
            Health.OnChange += OnChangeHealth;
        }

        public void AddImpact(Vector3 dir, float force)
        {
            _impactReceiver.AddImpact(dir,force);
        }

        public void ResetImpact() => _impactReceiver.Reset();


        private void OnChangeHealth()
        {
            if (!PlayerHealthModel.IsCantRecieveDamage)
            {
                PlayerHealthModel.OnChangeHealth -= PlayerHealthModel_OnChangeHealth;
                PlayerHealthModel.SetHealth(Health.Value);
                PlayerHealthModel.OnChangeHealth += PlayerHealthModel_OnChangeHealth;
            }
            else
            {
                Health.SetSilence(Health.LastValue); //hack
            }
        }
    }
}

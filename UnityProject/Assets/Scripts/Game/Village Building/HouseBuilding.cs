using System;
using System.Collections;
using System.Collections.Generic;
using Core.Storage;
using Game.Audio;
using Game.Models;
using NaughtyAttributes;
using UltimateSurvival;
using UnityEngine;

namespace Game.VillageBuilding
{
    public class HouseBuilding : MonoBehaviour, IData
    {
        [Serializable]
        public class Data : DataBase
        {
            public long startBuildingTimeTicks;
            public int level = 0;
            public bool isBuildingProcess = false;
            public bool isTimerUpgrade = true;

            public void SetStartBuildingTimeTicks(long startBuildingTimeTicks)
            {
                this.startBuildingTimeTicks = startBuildingTimeTicks;
                ChangeData();
            }

            public void SetLevel(int level)
            {
                this.level = level;
                ChangeData();
            }

            public void SetIsBuildingProcess(bool isBuildingProcess)
            {
                this.isBuildingProcess = isBuildingProcess;
                ChangeData();
            }

            public void SetIsTimerUpgrade(bool isTimerUpgrade)
            {
                this.isTimerUpgrade = isTimerUpgrade;
                ChangeData();
            }
        }

        [SerializeField] private Data data = default;
        [SerializeField] private HouseBuildingType type = default;
        [SerializeField] private float buildingTimeSec = 60f;
        [SerializeField] private HouseViewAnimation buildingAniamtion = default;
        [SerializeField] private HouseViewAnimation[] levelViews = default;
        [SerializeField] private BuildingHealthModel buildingHealthModel = default;

        private WorldObjectModel _worldObjectModel;

        public event Action OnStateChanged;
        public event Action<HouseBuilding> OnStartBuildingProcess;
        public event Action<HouseBuilding> OnCompleteBuildingProcess;
        public event Action OnDataInitialize;

        private AudioSystem AudioSystem => AudioSystem.Instance;
        public long StartBuildingTimeTicks
        {
            get{return data.startBuildingTimeTicks;}
            private set{data.SetStartBuildingTimeTicks(value);}
        }
        public int Level
        {
            get{ return data.level; }
            private set{data.SetLevel(value);}
        }
        public bool IsBuildingProcess
        {
            get{return data.isBuildingProcess;}
            set{data.SetIsBuildingProcess(value);}
        }
        public bool IsTimerUpgrade
        {
            get{return data.isTimerUpgrade;}
            private set{data.SetIsTimerUpgrade(value);}
        }

        public HouseBuildingType Type => type;
        public long BuildingTimeTicks => (long)buildingTimeSec * TimeSpan.TicksPerSecond;
        public TimeSpan BuildingTimeRemaining => TimeSpan.FromTicks(BuildingTimeTicks - (GameTimeModel.RealTimeNowTick - StartBuildingTimeTicks));
        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return data;
            }
        }
        private WorldObjectModel WorldObjectModel
        {
            get
            {
                if (_worldObjectModel == null) _worldObjectModel = GetComponentInParent<WorldObjectModel>();
                return _worldObjectModel;
            }
        }

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private VillageBuildingModel VillageBuildingModel => ModelsSystem.Instance._villageBuildingModel;

        private void Awake() 
        {
            VillageBuildingModel.AddActiveHouseBuilding(this);
        }

        private void OnEnable() 
        {
            if(WorldObjectModel == null)
            {
                Init();
            }
            GameUpdateModel.OnUpdate += UpdateBuildingProcess;
        }

        private void OnDisable() 
        {
            GameUpdateModel.OnUpdate -= UpdateBuildingProcess;
        }

        private void OnDestroy() 
        {
            VillageBuildingModel.RemoveActiveHouseBuilding(this);
        }

        public void UUIDInitialize() 
        {
            if(WorldObjectModel != null)
            {
                Init();
            }
        }

        private void Init()
        {
            StorageModel.TryProcessing(data);
            if(data.isBuildingProcess) ResumeBuildingProcess();
            UpdateLevelView();
        }

        private void UpdateLevelView(bool animate = false)
        {
            for(int i = 0; i< levelViews.Length; i++)
            {
                if(i == Level)
                {
                    levelViews[i].gameObject.SetActive(true);
                    if(animate) 
                        levelViews[i].PlayerShowAnimantion();
                    else
                        levelViews[i].SetShowedPosition();
                }
                else
                {
                    levelViews[i].gameObject.SetActive(false);
                }
            }
        }

        public DestroyableBuilding GetCurrentLevelDestroyableBuilding()
        {
            if(Level > 0 && Level < levelViews.Length)
                return levelViews[Level].GetComponent<DestroyableBuilding>();
            else
                return null;
        }

        public void StartBuildingProcess(bool isTimerUpgrade = true)
        {
            if(!IsBuildingProcess)
            {
                IsBuildingProcess = true;
                IsTimerUpgrade = isTimerUpgrade;
                StartBuildingTimeTicks = GameTimeModel.RealTimeNowTick;
                buildingAniamtion.gameObject.SetActive(true);
                buildingAniamtion.PlayerShowAnimantion();
                OnStateChanged?.Invoke();
                OnStartBuildingProcess?.Invoke(this);
            }
        }

        private void ResumeBuildingProcess()
        {
            buildingAniamtion.gameObject.SetActive(true);
            buildingAniamtion.PlayerShowAnimantion();
        }

        public void SkipBuildingTimeSec(float skipTimeSec)
        {
            StartBuildingTimeTicks += TimeSpan.TicksPerSecond * (long)skipTimeSec;
        }

        public void CompleteBuildingProcess()
        {
            if(IsBuildingProcess)
            {
                Level++;
                IsBuildingProcess = false;
                UpdateLevelView(true);
                buildingAniamtion.gameObject.SetActive(true);
                buildingAniamtion.PlayerHideAnimation(() => buildingAniamtion.gameObject.SetActive(false));
                if(buildingHealthModel != null) buildingHealthModel.SetHealth(buildingHealthModel.HealthMax);
                OnStateChanged?.Invoke();
                OnCompleteBuildingProcess?.Invoke(this);
                AudioSystem.PlayOnce(AudioID.CraftEnd);
            }
        }

        public void BreakBuildingProcess()
        {
            if(IsBuildingProcess)
            {
                IsBuildingProcess = false;
                UpdateLevelView(true);
                buildingAniamtion.gameObject.SetActive(true);
                buildingAniamtion.PlayerHideAnimation(() => buildingAniamtion.gameObject.SetActive(false));
                OnStateChanged?.Invoke();
            }
        }

        private void UpdateBuildingProcess()
        {
            if(IsBuildingProcess && IsTimerUpgrade)
            {
                if((GameTimeModel.RealTimeNowTick - StartBuildingTimeTicks) > BuildingTimeTicks)
                {
                    CompleteBuildingProcess();
                }
            }
        }
    }
}

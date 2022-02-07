using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class StatisticsModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredUInt KilledAll;
            public ObscuredUInt KilledAnimal;
            public ObscuredUInt KilledZombie;
            public ObscuredUInt KilledSkeleton;
            public ObscuredUInt CraftedItems;
            public ObscuredUInt BarrelsDestroyed;
            public ObscuredUInt GatheredResources;
            public ObscuredFloat TimeGameSession;
            public ObscuredULong StartAliveTimeTicks;

            public void SetKilledAll(uint killedAll)
            {
                KilledAll = killedAll;
                ChangeData();
            }

            public void SetKilledAnimal(uint killedAnimal)
            {
                KilledAnimal = killedAnimal;
                ChangeData();
            }

            public void SetKilledZombie(uint killedZombie)
            {
                KilledZombie = killedZombie;
            }

            public void SetKilledSkeleton(uint killedSkeleton)
            {
                KilledSkeleton = killedSkeleton;
                ChangeData();
            }

            public void SetCraftedItems(uint craftedItems)
            {
                CraftedItems = craftedItems;
                ChangeData();
            }

            public void SetBarrelsDestoyed(uint barrelsDestroyed)
            {
                BarrelsDestroyed = barrelsDestroyed;
                ChangeData();
            }

            public void SetGatheredResources(uint gatheredResources)
            {
                GatheredResources = gatheredResources;
                ChangeData();
            }

            public void SetTimeGameSession(float timeGameSession)
            {
                TimeGameSession = timeGameSession;
                ChangeData();
            }

            public void SetStartAliveTimeTicks(ulong startAliveTimeTicks)
            {
                StartAliveTimeTicks = startAliveTimeTicks;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

#pragma warning restore 0649
        #endregion

        public StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public uint KilledAll
        {
            get => _data.KilledAll;
            protected set => _data.SetKilledAll(value);
        }

        public uint KilledAnimal
        {
            get => _data.KilledAnimal;
            protected set => _data.SetKilledAnimal(value);
        }

        public uint KilledZombie
        {
            get => _data.KilledZombie;
            protected set => _data.SetKilledZombie(value);
        }

        public uint KilledSkeleton
        {
            get => _data.KilledSkeleton;
            protected set => _data.SetKilledSkeleton(value);
        }

        public uint CraftedItems
        {
            get => _data.CraftedItems;
            protected set => _data.SetCraftedItems(value);
        }

        public uint BarrelsDestroyed
        {
            get => _data.BarrelsDestroyed;
            protected set => _data.SetBarrelsDestoyed(value);
        }

        public uint GatheredResources
        {
            get => _data.GatheredResources;
            protected set => _data.SetGatheredResources(value);
        }

        public float TimeGameSession
        {
            get => _data.TimeGameSession;
            protected set => _data.SetTimeGameSession(value);
        }

        public ulong StartAliveTimeTicks
        {
            get => _data.StartAliveTimeTicks;
            protected set => _data.SetStartAliveTimeTicks(value);
        }

        private void OnEnable() => StorageModel.TryProcessing(_data);

        public void Kill() => ++KilledAll;
        public void KillAnimal() => ++KilledAnimal;
        public void KillZombie() => ++KilledZombie;
        public void KillSkeleton() => ++KilledSkeleton;
        public void CraftItem() => ++CraftedItems;
        public void BarrelDestroy() => ++BarrelsDestroyed;
        public void GatherResource(uint count) => GatheredResources += count;
        public void SetStartAliveTimeTicks(ulong ticks) => StartAliveTimeTicks = ticks;
        public void AddSesionTime(float seconds) => TimeGameSession += seconds;

        public void ResetSessionData()
        {
            KilledAll = 0;
            KilledAnimal = 0;
            KilledSkeleton = 0;
            KilledZombie = 0;
            CraftedItems = 0;
            BarrelsDestroyed = 0;
            GatheredResources = 0;
            TimeGameSession = 0;
        }
    }
}

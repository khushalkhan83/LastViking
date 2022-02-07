using CodeStage.AntiCheat.ObscuredTypes;
using Core;
using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class PlayerConsumeModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public ObscuredFloat RemainingTime { set; get; }

        public bool IsInProgress => RemainingTime > 0;
        public bool IsCanConsume => RemainingTime == 0;
        public float RemainingTimeNormalized => RemainingTime / StartTime;

        public ObscuredFloat StartTime { get; private set; }

        public event Action OnStartConsumeProcess;
        public event Action OnEndConsumeProcess;

        private UniqueAction<ItemsContainer> OnStartConsumeAction { get; } = new UniqueAction<ItemsContainer>();
        private UniqueAction<ItemsContainer> OnUpdateConsumeAction { get; } = new UniqueAction<ItemsContainer>();
        private UniqueAction<ItemsContainer> OnEndConsumeAction { get; } = new UniqueAction<ItemsContainer>();

        public IUniqueEvent<ItemsContainer> OnStartConsume => OnStartConsumeAction;
        public IUniqueEvent<ItemsContainer> OnUpdateConsume => OnUpdateConsumeAction;
        public IUniqueEvent<ItemsContainer> OnEndConsume => OnEndConsumeAction;

        public StorageModel StorageModel => _storageModel;

        public ItemsContainer Container { get; private set; }
        public ObscuredInt CellId { get; private set; }
        public SavableItem Item { get; private set; }

        public void StartConsume(ObscuredFloat timeConsume, ItemsContainer container, ObscuredInt cellId)
        {
            if (timeConsume <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (cellId < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            StartTime = timeConsume;
            RemainingTime = timeConsume;

            Container = container ?? throw new ArgumentNullException();
            CellId = cellId;
            Item = new SavableItem(container.GetCell(cellId).Item);

            OnStartConsumeAction.Invoke(container);
            OnStartConsumeProcess?.Invoke();
        }

        public void ConsumeProcess()
        {
            RemainingTime -= Time.deltaTime;
            OnUpdateConsumeAction.Invoke(Container);

            if (RemainingTime < 0)
            {
                RemainingTime = 0;
                EndConsume();
            }
        }

        private void EndConsume()
        {
            OnEndConsumeAction.Invoke(Container);
            OnEndConsumeProcess?.Invoke();

            //handlers need this data in call moment
            Item = null;
            Container = null;
            CellId = -1;
        }
    }
}

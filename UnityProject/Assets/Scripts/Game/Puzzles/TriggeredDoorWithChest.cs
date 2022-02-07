using System;
using System.Collections;
using System.Collections.Generic;
using Core.Storage;
using DG.Tweening;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Puzzles
{
    public class TriggeredDoorWithChest : TriggeredMechanismBase
    {
        [Serializable]
        public class Data : DataBase
        {
            public bool activated = false;
            public uint chestWorldObjectId = 0;

            public void SetActivated(bool value)
            {
                activated = value;
                ChangeData();
            }

            public void SetChestWorldObjectId(uint id)
            {
                chestWorldObjectId = id;
                ChangeData();
            }
        }

        [SerializeField] private Data data = default;
        [SerializeField] private string puzzleName = "Puzzle";        
        [SerializeField] private Transform doorTransform = default;
        [SerializeField] private Vector3 doorOpenedPosition = default;
        [SerializeField] private Vector3 doorClosedPosition = default;
        [SerializeField] private float animationTime = 0.5f;
        [SerializeField] private bool dropChest = true;
        [SerializeField] private Transform chestDropPoint = default;
        [SerializeField] private CellSpawnSettings[] _cellsSettings = default;

        public event Action OnActivated;

        public string PuzzleName => puzzleName;
        public LootObject DroppedChest {get; private set;}

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private DropContainerModel DropContainerModel => ModelsSystem.Instance._dropContainerModel;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private PuzzlesModel PuzzlesModel => ModelsSystem.Instance._puzzlesModel;

        private void Awake() 
        {
            PuzzlesModel.AddActivePuzzle(this); 
        }

        protected override void OnEnable() 
        {
            StorageModel.TryProcessing(data);
            PuzzlesModel.AddActivePuzzle(this);
            base.OnEnable();
        }

        private void OnDestroy() 
        {
            PuzzlesModel.RemoveActivePuzzle(this);
        }

        public override bool Activated 
        { 
            get => data.activated; 
            protected set => data.SetActivated(value);
        }

        public uint ChestWorldObjectId 
        { 
            get => data.chestWorldObjectId; 
            protected set => data.SetChestWorldObjectId(value);
        }

        protected override void UpdateMechanismState()
        {
            if(!Activated)
            {
                Activated = CheckIsActive();
                if(Activated)
                { 
                    DropChest();
                    PuzzlesModel.PuzzleActivated(puzzleName);
                    OnActivated?.Invoke();
                }
            }
            UpdateMechanismView();
        }

        protected virtual void DropChest()
        {
            if(dropChest)
            {
                List<SavableItem> dropItems = new List<SavableItem>();
                foreach(var cellsSetting in _cellsSettings)
                {
                    dropItems.Add(cellsSetting.GenerateItem(ItemsDB.ItemDatabase));
                }
                DroppedChest = DropContainerModel.DropContainer(chestDropPoint.transform.position, 0.1f, dropItems)[0];
                ChestWorldObjectId = DroppedChest.GetComponentInParent<WorldObjectModel>().ID;
            }
        }

        protected override void UpdateMechanismView()
        {
            DOTween.Kill(doorTransform);
            Vector3 endValue = Activated? doorOpenedPosition : doorClosedPosition;
            doorTransform.DOLocalMove(endValue, animationTime);
        }
    }
}

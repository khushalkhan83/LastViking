using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public abstract class TutorialStep_MineResource : TutorialStepBase
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public TokensModel TokensModel { get; private set; }
        [Inject] public MinebleElementsModel MinebleElementsModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }

        private const string k_exeptionInTokenPosition = "toeken_stone";
        private readonly string requiaredResource = "Stone";
        private readonly int resourceCount;
        private readonly int tokenConfigId = 12;
        private string tockenID = "toeken_stone";
        private OutLineMinableObjectID outLineMinableObjectID;
        private readonly Sprite messageIcon;
        private string notificationMessage = "Mine stones to build base";

        public TutorialStep_MineResource(TutorialEvent StepStartedEvent, string requiaredResource, int resourceCount, int tokenConfigId, string tockenID, OutLineMinableObjectID outLineMinableObjectID, Sprite messageIcon, string notificationMessage): base(StepStartedEvent)
        {
            this.requiaredResource = requiaredResource;
            this.resourceCount = resourceCount;
            this.tokenConfigId = tokenConfigId;
            this.tockenID = tockenID;
            this.outLineMinableObjectID = outLineMinableObjectID;
            this.messageIcon = messageIcon;
            this.notificationMessage = notificationMessage;
        }

        private MinebleElement currentTokenTarget = null;

        private bool HasEnoughItems() => PlayerHasItems(requiaredResource, resourceCount);

        public override void OnStart()
        {
            GameUpdateModel.OnUpdate += UpdateToken;
            InventoryModel.ItemsContainer.OnChangeCell += OnChangeInventoryCell;
            HotBarModel.ItemsContainer.OnChangeCell += OnChangeInventoryCell;

            ProcessState();

            currentTokenTarget = null;
        }
        public override void OnEnd()
        {
            GameUpdateModel.OnUpdate -= UpdateToken;
            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeInventoryCell;
            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeInventoryCell;
            TokensModel.HideToken(tockenID);
            ShowTaskFill(false);
        }

        private void OnChangeInventoryCell(CellModel cell) => CheckConditions();

        private void ProcessState()
        {
            if (HasEnoughItems())
            {
                CheckConditions();
            }
            else
            {
                ShowTaskMessage(true,notificationMessage,messageIcon);
                ShowTaskFill(true);
                UpdateTaskFill();
            }
        }

        private bool PlayerHasItems(string itemName, int count)
        {
            var itemData = ItemsDB.GetItem(itemName);
            return InventoryOperationsModel.PlayerHasItems(itemData, count);
        }

        private void UpdateToken()
        {
            if (MinebleElementsModel.OutlineObjects.TryGetValue(outLineMinableObjectID, out List<MinebleElement> elements))
            {
                MinebleElement closestTarget = null;
                float closestSqrDistance = float.MaxValue;
                foreach (var element in elements)
                {
                    if (element.MinableFractures.FirstOrDefault()?.ResourceValue <= 0)
                        continue;

                    float sqrDistance = (PlayerEventHandler.transform.position - element.transform.position).sqrMagnitude;
                    if (sqrDistance < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistance;
                        closestTarget = element;
                    }
                }

                if (currentTokenTarget != closestTarget)
                {
                    currentTokenTarget = closestTarget;
                    TokensModel.HideToken(tockenID);
                    if (currentTokenTarget != null)
                    {
                        Vector3 shiftPosition = tockenID.Equals(k_exeptionInTokenPosition) ? Vector3.up : Vector3.up * 2f;
                        TokensModel.ShowToken(tockenID, tokenConfigId, currentTokenTarget.transform.position + shiftPosition);
                    }
                }
            }
        }

        private void UpdateTaskFill()
        {
            SetTaskFillValue(InventoryOperationsModel.GetItemCount(requiaredResource),resourceCount);
        }

        private void CheckConditions()
        {
            UpdateTaskFill();
            bool nextStep = HasEnoughItems();

            if(nextStep) TutorialNextStep();
        }
    }
}
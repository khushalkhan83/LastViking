using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using Game.Views.Cell;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Controllers
{
    public class HotBarViewController : ViewControllerBase<HotBarView>
    {
        [Inject] public PlayerBleedingDamagerModel PlayerBleedingDamagerModel { get; private set; }
        [Inject] public PlayerPoisonDamagerModel PlayerPoisonDamagerModel { get; private set; }
        [Inject] public PlayerDrinkProcessModel PlayerDrinkProcessModel { get; private set; }
        [Inject] public PlayerHealProcessModel PlayerHealProcessModel { get; private set; }
        [Inject] public PlayerEatProcessModel PlayerEatProcessModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public RepairViewModel RepairViewModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public InventoryButtonViewModel HotBarViewModel { get; private set; }

        protected AddStatsEffectView AddStatsEffectView { get; private set; }

        protected bool IsPlayerInWater => PlayerMovementModel.MovementID == PlayerMovementID.Water;

        protected override void Show()
        {
            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCellHandler;

            HotBarModel.OnChangeEquipCell += OnChangeEquipCellHandler;

            PlayerConsumeModel.OnStartConsume.AddListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            RepairingItemsModel.OnUpdateRepairing += OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount += OnChangeCountRepairingHandler;

            PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;

            var cellsDataHotBar = CollectCellsData();

            for (int cellId = 0; cellId < cellsDataHotBar.Length; cellId++)
            {
                View.Cells[cellId].SetData(cellsDataHotBar[cellId]);
                View.Cells[cellId].OnPointDown_ += OnPointDownHandler;
            }

            View.Cells[HotBarModel.EquipCellId].Selection();
            HotBarModel.SetCanPlayAudio(false);
        }

        protected override void Hide()
        {
            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCellHandler;

            HotBarModel.OnChangeEquipCell -= OnChangeEquipCellHandler;

            PlayerConsumeModel.OnStartConsume.RemoveListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            RepairingItemsModel.OnUpdateRepairing -= OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount -= OnChangeCountRepairingHandler;

            PlayerMovementModel.OnChangeMovementID -= OnChangePlayerMovement;

            foreach (var cellView in View.Cells)
            {
                cellView.OnPointDown_ -= OnPointDownHandler;
            }

            HideAddEffect();
            HotBarModel.SetCanPlayAudio(false);
        }

        private void OnUpdateConsumePlayerHandler() => UpdateCells(HotBarModel.ItemsContainer.Cells, View.Cells);

        private void OnEndConsumePlayerHandler() => UpdateCells(HotBarModel.ItemsContainer.Cells, View.Cells);

        private void OnStartConsumePlayerHandler()
        {
            UpdateCells(HotBarModel.ItemsContainer.Cells, View.Cells);
            if (PlayerConsumeModel.Container == HotBarModel.ItemsContainer)
            {
                ConsumeItem(PlayerConsumeModel.Item, PlayerConsumeModel.CellId);
            }
        }
        
        private void ConsumeItem(SavableItem item, int cellId)
        {
            var statInfos = new List<AddStatsEffectView.StatInfo>();

            if (item.TryGetProperty("Health Change", out var property))
            {
                statInfos.Add(new AddStatsEffectView.StatInfo() { StatID = AddStatsEffectView.StatID.Health, Count = property.Int.Current });
            }

            if (item.TryGetProperty("Hunger Change", out property))
            {
                statInfos.Add(new AddStatsEffectView.StatInfo() { StatID = AddStatsEffectView.StatID.Food, Count = property.Int.Current });
            }

            if (item.TryGetProperty("Thirst Change", out property))
            {
                statInfos.Add(new AddStatsEffectView.StatInfo() { StatID = AddStatsEffectView.StatID.Water, Count = property.Int.Current });
            }

            if (item.TryGetProperty("Consume Sound", out property))
            {
                AudioSystem.PlayOnce(property.AudioID);
            }

            if (AddStatsEffectView != null)
            {
                HideAddEffect();
            }

            ShowAddeffect(cellId, statInfos);
        }

        private void OnUpdateRepairingHandler() => UpdateCells(HotBarModel.ItemsContainer.Cells, View.Cells);

        private void OnChangeCountRepairingHandler() => UpdateCells(HotBarModel.ItemsContainer.Cells, View.Cells);

        private void OnEndAllAddEffectsHandler() => HideAddEffect();

        private void ShowAddeffect(int cellId, List<AddStatsEffectView.StatInfo> statInfos)
        {
            AddStatsEffectView = ViewsSystem.Show<AddStatsEffectView>(ViewConfigID.AddStatsEffect, View.transform);
            AddStatsEffectView.OnEndAll += OnEndAllAddEffectsHandler;
            AddStatsEffectView.SetPosition(View.Cells[cellId].transform.position);
            AddStatsEffectView.StartEffect(statInfos.ToArray());
        }

        private void HideAddEffect()
        {
            if (AddStatsEffectView != null)
            {
                AddStatsEffectView.EndEffect();
                AddStatsEffectView.OnEndAll -= OnEndAllAddEffectsHandler;
                ViewsSystem.Hide(AddStatsEffectView);
                AddStatsEffectView = null;
            }
        }

        private void UpdateCells(IEnumerable<CellModel> cells, HotBarCellView[] cellViews)
        {
            foreach (var cell in cells)
            {
                UpdateCell(cell, cellViews[cell.Id]);
            }
        }

        private void UpdateCell(CellModel cellModel, HotBarCellView cellView)
        {
            cellView.SetData(GetCellData(cellModel));
        }

        private void OnChangeEquipCellHandler()
        {
            HotBarModel.SetCanPlayAudio(true);
            View.Cells[HotBarModel.EquipCellIdLast].Deselection();
            View.Cells[HotBarModel.EquipCellId].Selection();
        }

        private void OnPointDownHandler(HotBarCellView cellView, PointerEventData eventData)
        {
            var cell = HotBarModel.ItemsContainer.GetCell(cellView.Id);
            var item = cell.Item;
            var data = GetCellData(cell);

            if (cell.IsHasItem && !data.IsDisable && item.IsCanConsume())
            {
                if (PlayerConsumeModel.IsCanConsume)
                {
                    if (item.TryGetProperty("PoisonDamage", out var propertyPoisonDamage) && item.TryGetProperty("PoisonTime", out var propertyPoisonTime))
                    {
                        PlayerPoisonDamagerModel.AddPoison(propertyPoisonTime.Float.Current, propertyPoisonDamage.Float.Current);
                    }

                    if (item.HasProperty("Antidote"))
                    {
                        PlayerPoisonDamagerModel.AddAntidote();
                    }

                    if (item.TryGetProperty("Health Change", out var property))
                    {
                        item.TryGetProperty("ConsumeTime", out var timeProperty);
                        PlayerHealProcessModel.AddHeal(timeProperty.Float.Current, property.Int.Current, property.Int.Current > PlayerHealthModel.HealthMax);
                    }

                    if (item.TryGetProperty("Hunger Change", out property))
                    {
                        item.TryGetProperty("ConsumeTime", out var timeProperty);
                        PlayerEatProcessModel.AddEat(timeProperty.Float.Current, property.Int.Current, property.Int.Current > PlayerFoodModel.FoodMax);
                    }

                    if (item.TryGetProperty("Thirst Change", out property))
                    {
                        item.TryGetProperty("ConsumeTime", out var timeProperty);
                        PlayerDrinkProcessModel.AddDrink(timeProperty.Float.Current, property.Int.Current, property.Int.Current > PlayerWaterModel.WaterMax);
                    }

                    if (item.TryGetProperty("ConsumeTime", out property))
                    {
                        PlayerConsumeModel.StartConsume(property.Float.Current, HotBarModel.ItemsContainer, cell.Id);
                    }

                    if (item.HasProperty("StopBleeding"))
                    {
                        PlayerBleedingDamagerModel.SetBleeding(false);
                    }

                    var isNeedRemove = true;

                    if (item.TryGetProperty("Sips", out property))
                    {
                        --property.Int.Current;
                        isNeedRemove = property.Int.Current == 0;
                    }

                    if (isNeedRemove)
                    {
                        HotBarModel.ItemsContainer.RemoveItemsFromCell(cellView.Id, 1);
                    }
                }
            }
            else
            {
                if(cell.IsHasItem)
                {
                    HotBarModel.Equp(cellView.Id);
                }
            }
        }

        private void OnChangePlayerMovement() => UpdateCells(HotBarModel.ItemsContainer.Cells, View.Cells);

        private void OnChangeCellHandler(CellModel cell)
        {
            View.Cells[cell.Id].SetData(GetCellData(cell));

            if (cell.Id == HotBarModel.EquipCellId)
            {
                View.Cells[HotBarModel.EquipCellId].Selection();
            }
        }

        private HotBarCellData[] CollectCellsData()
        {
            var result = new HotBarCellData[HotBarModel.ItemsContainer.CountCells];

            var i = 0;
            foreach (var cell in HotBarModel.ItemsContainer.Cells)
            {
                result[i] = GetCellData(cell);
                ++i;
            }

            return result;
        }

        private HotBarCellData GetCellData(CellModel cell)
        {
            var icon = (Sprite)null;
            var count = (int?)null;
            var durability = (float?)null;
            var sips = (float?)null;
            var progressConsumable = (float?)null;
            var isDisable = false;
            var isVisibleBorder = HotBarModel.EquipCellId == cell.Id;
            var progressRepairing = (float?)null;

            var isConsumable = PlayerConsumeModel.IsInProgress
                   && PlayerConsumeModel.Container == HotBarModel.ItemsContainer
                   && cell.Id == PlayerConsumeModel.CellId;

            SavableItem item;
            if (isConsumable)
            {
                item = PlayerConsumeModel.Item;
            }
            else
            {
                item = cell.Item;
            }

            if (cell.IsHasItem || isConsumable)
            {
                icon = item.ItemData.Icon;

                if (item.ItemData.StackSize > 1)
                {
                    count = item.Count;
                }

                if (item.TryGetProperty("Durability", out var durabilityProperty))
                {
                    durability = durabilityProperty.Float.Ratio;
                }

                if (item.TryGetProperty("Sips", out var sipsProperty))
                {
                    sips = sipsProperty.Int.Ratio;
                }

                if (isConsumable)
                {
                    progressConsumable = PlayerConsumeModel.RemainingTimeNormalized;
                }

                if (RepairingItemsModel.TryFindRepairItemInfo(item, out var info))
                {
                    progressRepairing = info.RemainingTime / info.AllTime;
                }

                var consuming = (item.IsCanConsume() && PlayerConsumeModel.IsInProgress);
                var isPlayerInWater = PlayerMovementModel.MovementID == PlayerMovementID.Water;
                isDisable = consuming || (!item.IsCanConsume() && isPlayerInWater) || item.IsBroken();
            }

            return new HotBarCellData
            {
                Id = cell.Id,
                Icon = icon,
                Count = count,
                Durability = durability,
                ProgressConsumable = progressConsumable,
                ProgressRepairing = progressRepairing,
                IsDisable = isDisable,
                Sips = sips,
                IsVisibleBorder = isVisibleBorder,
            };
        }
    }
}

using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Controllers
{
    public class ApplyItemController : IApplyItemController, IController
    {
        [Inject] public ApplyItemModel ApplyItemModel { get; private set; }
        [Inject] public PlayerPoisonDamagerModel PlayerPoisonDamagerModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public PlayerHealProcessModel PlayerHealProcessModel { get; private set; }
        [Inject] public PlayerEatProcessModel PlayerEatProcessModel { get; private set; }
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public PlayerDrinkProcessModel PlayerDrinkProcessModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public PlayerBleedingDamagerModel PlayerBleedingDamagerModel { get; private set; }

        void IController.Enable()
        {
            ApplyItemModel.OnApplyItem += OnApplyItem;
        }

        void IController.Start() { }

        void IController.Disable()
        {
            ApplyItemModel.OnApplyItem -= OnApplyItem;
        }

        private void OnApplyItem(ItemsContainer container, CellModel cell)
        {
            var item = cell.Item;

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
                PlayerConsumeModel.StartConsume(property.Float.Current, container, cell.Id);
            }

            if (item.TryGetProperty("Consume Sound", out property))
            {
                AudioSystem.PlayOnce(property.AudioID);
            }

            if (item.HasProperty("StopBleeding"))
            {
                PlayerBleedingDamagerModel.SetBleeding(false);
            }

            var isNeedRemove = true;

            if (item.TryGetProperty("Sips", out property))
            {
                --property.Int.Current;
                item.SetProperty(property);
                isNeedRemove = property.Int.Current <= 0;
            }

            if (isNeedRemove)
            {
                container.RemoveItemsFromCell(cell.Id, 1);
            }
        }
    }
}

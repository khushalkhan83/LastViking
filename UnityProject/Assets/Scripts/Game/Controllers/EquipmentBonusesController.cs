using System.Collections.Generic;
using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;

namespace Game.Controllers
{
    public class EquipmentBonusesController : IEquipmentBonusesController, IController
    {
        [Inject] public InventoryEquipmentModel InventoryEquipmentModel { get; set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; set; }
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; set; }
        [Inject] public PlayerMovementControllerGround MovementControllerGround { get; private set; }
        [Inject] public PlayerHealthRegenerationModel PlayerHealthRegenerationModel { get; private set; }
        [Inject] public EquipmentModel EquipmentModel { get; private set; }

        void IController.Enable() 
        {
            InventoryEquipmentModel.OnChangeEquipItem += UpdateEquipmentBonuses;
            UpdateEquipmentBonuses();
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            InventoryEquipmentModel.OnChangeEquipItem -= UpdateEquipmentBonuses;
        }

        private void UpdateEquipmentBonuses()
        {
            var equipedItems = InventoryEquipmentModel.GetNotBrokenEquipedItems();

            float healthBonus = 0;
            float foodBonus = 0;
            float waterBonus = 0;
            float staminaPercentageBonus = 0;
            float speedPercentageBonus = 0;
            float regenHealthBonus = 0;

            EquipmentModel.EquipmentSetItemCounts.Clear();

            foreach(var item in equipedItems)
            {
                if(item.TryGetProperty("EquipmentSet", out var setProp))
                {
                    EquipmentSet equipmentSet = setProp.EquipmentSet;
                    if(equipmentSet != EquipmentSet.None)
                    {
                        EquipmentModel.EquipmentSetItemCounts.TryGetValue(equipmentSet, out int count);
                        count++;
                        EquipmentModel.EquipmentSetItemCounts[equipmentSet] = count;
                    }
                }

                if(item.TryGetProperty("HealthBonus", out var healthProp))
                {
                    healthBonus += healthProp.Int.Current;
                }

                if(item.TryGetProperty("FoodBonus", out var hungerProp))
                {
                    foodBonus += hungerProp.Int.Current;
                }

                if(item.TryGetProperty("WaterBonus", out var thirstProp))
                {
                    waterBonus += thirstProp.Int.Current;
                }

                if(item.TryGetProperty("StaminaPercentageBonus", out var staminaProp))
                {
                    staminaPercentageBonus += staminaProp.Int.Current;
                }

                if(item.TryGetProperty("SpeedPercentageBonus", out var speedProp))
                {
                    speedPercentageBonus += speedProp.Int.Current;
                }

                if(item.TryGetProperty("RegenHealthBonus", out var regenHealthProp))
                {
                    regenHealthBonus += regenHealthProp.Int.Current;
                }
            }

            UpdateSetBonuses();

            PlayerHealthModel.SetEquipmentBonus(healthBonus);
            PlayerFoodModel.SetEquipmentBonus(foodBonus);
            PlayerWaterModel.SetEquipmentBonus(waterBonus);
            PlayerStaminaModel.SetEquipmentPercentageBonus(staminaPercentageBonus);
            PlayerStaminaModel.SetEquipmentPercentageBonus(staminaPercentageBonus);
            MovementControllerGround.SetEquipmentPercentageBonus(speedPercentageBonus);
            PlayerHealthRegenerationModel.SetRegenValue(regenHealthBonus);

            void UpdateSetBonuses()
            {
                foreach(var key in EquipmentModel.EquipmentSetItemCounts.Keys)
                {
                    int setCount = EquipmentModel.EquipmentSetItemCounts[key];
                    var setInfo = EquipmentModel.EquipmentSetInfoProvider[key];
                    if(setCount >= setInfo.FullSetCount)
                    {
                        switch(setInfo.AbilityID)
                        {
                            case AbilityID.IncreaseHealth:
                                healthBonus += setInfo.BonusProperty.Int.Current;
                                break;
                        }
                    }
                }
            }
        }

    }
}

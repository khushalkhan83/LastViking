using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class InfoPanelFoodViewController : ViewControllerBase<InfoPanelFoodView>
    {
        [Inject] public InfoPanelFoodViewModel InfoPanelFoodViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public AbilityProvider AbilityProvider { get; private set; }

        protected override void Show()
        {
            InfoPanelFoodViewModel.FoodItem.TryGetProperty("FirstAbilityID", out var firstAbilityID);
            InfoPanelFoodViewModel.FoodItem.TryGetProperty("SecondAbilityID", out var secondAbilityID);

            View.SetFoodItemName(LocalizationModel.GetString(InfoPanelFoodViewModel.TitleTextID));

            if (firstAbilityID != null)
            {
                var firstAbility = AbilityProvider[firstAbilityID.AbilityID];
                if (firstAbility.Sprite != null)
                {
                    View.SetVisibleFisrtAbiltity(true);
                    View.SetFirstAbilityIcon(firstAbility.Sprite);
                }
                else
                {
                    View.SetVisibleFisrtAbiltity(false);
                }

                if (firstAbility.LocalizationKeyID != LocalizationKeyID.None)
                {
                    View.SetFoodItemDescription(LocalizationModel.GetString(firstAbility.LocalizationKeyID));
                }
                else
                {
                    View.SetFoodItemDescription(null);
                }
            }
            else
            {
                View.SetVisibleFisrtAbiltity(false);
                View.SetFoodItemDescription(null);
            }

            if (secondAbilityID != null)
            {
                var secondAbility = AbilityProvider[secondAbilityID.AbilityID];
                if (secondAbility.Sprite != null)
                {
                    View.SetVisibleSecondAbility(true);
                    View.SetSecondAbilityIcon(secondAbility.Sprite);
                }
                else
                {
                    View.SetVisibleSecondAbility(false);
                }

                if (secondAbility.LocalizationKeyID != LocalizationKeyID.None)
                {
                    View.SetFoodItemDescriptionSecond(LocalizationModel.GetString(secondAbility.LocalizationKeyID));
                }
                else
                {
                    View.SetFoodItemDescriptionSecond(null);
                }
            }
            else
            {
                View.SetVisibleSecondAbility(false);
                View.SetFoodItemDescriptionSecond(null);
            }

            if (firstAbilityID == null && secondAbilityID == null)
            {
                View.SetDecorActive(true);
            }
            else
            {
                View.SetDecorActive(false);
            }

            InfoPanelFoodViewModel.SetHealth(0);
            InfoPanelFoodViewModel.SetHunger(0);
            InfoPanelFoodViewModel.SetThirst(0);
            View.SetHealthValue(string.Empty);
            View.SetHungerValue(string.Empty);
            View.SetThirstValue(string.Empty);

            var isHasHeath = InfoPanelFoodViewModel.FoodItem.IsHasProperty("Health Change");
            var isHasHunger = InfoPanelFoodViewModel.FoodItem.IsHasProperty("Hunger Change");
            var isHasThirst = InfoPanelFoodViewModel.FoodItem.IsHasProperty("Thirst Change");

            if (isHasHeath)
            {
                var propertyHealth = InfoPanelFoodViewModel.FoodItem.GetProperty("Health Change");
                InfoPanelFoodViewModel.SetHealth(propertyHealth.Int.Current);
            }
            if (isHasHunger)
            {
                var propertyHunger = InfoPanelFoodViewModel.FoodItem.GetProperty("Hunger Change");
                InfoPanelFoodViewModel.SetHunger(propertyHunger.Int.Current);
            }
            if (isHasThirst)
            {
                var propertyThirst = InfoPanelFoodViewModel.FoodItem.GetProperty("Thirst Change");
                InfoPanelFoodViewModel.SetThirst(propertyThirst.Int.Current);
            }

            if (InfoPanelFoodViewModel.HealthValue > 0)
            {
                View.SetHealthValue($"+{InfoPanelFoodViewModel.HealthValue}");
                View.SetColorsGreen(View.HealthValue, View.HealthIcon);
            }
            else if (InfoPanelFoodViewModel.HealthValue < 0)
            {
                View.SetHealthValue(InfoPanelFoodViewModel.HealthValue.ToString());
                View.SetColorsRed(View.HealthValue, View.HealthIcon);
            }
            else
            {
                View.SetColorsGrey(View.HealthValue, View.HealthIcon);
            }

            if (InfoPanelFoodViewModel.HungerValue > 0)
            {
                View.SetHungerValue($"+{InfoPanelFoodViewModel.HungerValue}");
                View.SetColorsGreen(View.HungerValue, View.HungerIcon);
            }
            else if (InfoPanelFoodViewModel.HungerValue < 0)
            {
                View.SetHungerValue(InfoPanelFoodViewModel.HungerValue.ToString());
                View.SetColorsRed(View.HungerValue, View.HungerIcon);
            }
            else
            {
                View.SetColorsGrey(View.HungerValue, View.HungerIcon);
            }

            if (InfoPanelFoodViewModel.ThirstValue > 0)
            {
                View.SetThirstValue($"+{InfoPanelFoodViewModel.ThirstValue}");
                View.SetColorsGreen(View.ThirstValue, View.ThirstIcon);
            }
            else if (InfoPanelFoodViewModel.ThirstValue < 0)
            {

                View.SetThirstValue(InfoPanelFoodViewModel.ThirstValue.ToString());
                View.SetColorsRed(View.ThirstValue, View.ThirstIcon);
            }
            else
            {
                View.SetColorsGrey(View.ThirstValue, View.ThirstIcon);
            }
        }

        protected override void Hide()
        {

        }
    }
}

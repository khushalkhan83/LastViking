using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class InfoPanelMedicineViewController : ViewControllerBase<InfoPanelMedicineView>
    {
        [Inject] public InfoPanelMedicineViewModel InfoPanelMedicineViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public AbilityProvider AbilityProvider { get; private set; }

        protected override void Show()
        {
            InfoPanelMedicineViewModel.Health.TryGetProperty("FirstAbilityID", out var firstAbilityID);
            InfoPanelMedicineViewModel.Health.TryGetProperty("SecondAbilityID", out var secondAbilityID);

            View.SetMedicineItemName(LocalizationModel.GetString(InfoPanelMedicineViewModel.TitleTextID));

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
                    View.SetMedicineItemDescription(LocalizationModel.GetString(firstAbility.LocalizationKeyID));
                }
                else
                {
                    View.SetMedicineItemDescription(null);
                }
            }
            else
            {
                View.SetVisibleFisrtAbiltity(false);
                View.SetMedicineItemDescription(null);
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
                    View.SetMedicineItemDescriptionSecond(LocalizationModel.GetString(secondAbility.LocalizationKeyID));
                }
                else
                {
                    View.SetMedicineItemDescriptionSecond(null);
                }
            }
            else
            {
                View.SetVisibleSecondAbility(false);
                View.SetMedicineItemDescriptionSecond(null);
            }

            if (firstAbilityID == null && secondAbilityID == null)
            {
                View.SetDecorActive(true);
            }
            else
            {
                View.SetDecorActive(false);
            }

            var healthAddon = InfoPanelMedicineViewModel.Health.GetProperty("Health Change");

            View.SetHealthAddon($"+{ healthAddon.Int.Current}");
        }

        protected override void Hide()
        {

        }
    }
}

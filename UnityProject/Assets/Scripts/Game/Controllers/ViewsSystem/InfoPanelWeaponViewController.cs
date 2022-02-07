using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class InfoPanelWeaponViewController : ViewControllerBase<InfoPanelWeaponView>
    {
        [Inject] public InfoPanelWeaponViewModel InfoPanelWeaponViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public AbilityProvider AbilityProvider { get; private set; }

        protected override void Show()
        {
            InfoPanelWeaponViewModel.Weapon.TryGetProperty("FirstAbilityID", out var firstAbilityID);
            InfoPanelWeaponViewModel.Weapon.TryGetProperty("SecondAbilityID", out var secondAbilityID);

            View.SetWeaponItemName(LocalizationModel.GetString(InfoPanelWeaponViewModel.TitleTextID));

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
                    View.SetWeaponItemDescription(LocalizationModel.GetString(firstAbility.LocalizationKeyID));
                }
                else
                {
                    View.SetWeaponItemDescription(null);
                }
            }
            else
            {
                View.SetVisibleFisrtAbiltity(false);
                View.SetWeaponItemDescription(null);
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
                    View.SetWeaponItemDescriptionSecond(LocalizationModel.GetString(secondAbility.LocalizationKeyID));
                }
                else
                {
                    View.SetWeaponItemDescriptionSecond(null);
                }
            }
            else
            {
                View.SetVisibleSecondAbility(false);
                View.SetWeaponItemDescriptionSecond(null);
            }

            if (firstAbilityID == null && secondAbilityID == null)
            {
                View.SetDecorActive(true);
            }
            else
            {
                View.SetDecorActive(false);
            }

            InfoPanelWeaponViewModel.SetDamage(0);
            InfoPanelWeaponViewModel.SetDurability(0);

            UpdateDamageInfo();
            UpdateDurabilityInfo();
        }

        private void UpdateDamageInfo()
        {
            if (InfoPanelWeaponViewModel.Weapon.TryGetProperty("Damage", out var propertyDamage))
            {
                View.SetFillAmountWeaponDamage(GetAmountFill(GetGroup(propertyDamage.Int.Current, View.DamageGroupRanges)));
                InfoPanelWeaponViewModel.SetDamage(propertyDamage.Int.Current);
            }
            else
            {
                View.SetFillAmountWeaponDamage(0);
                InfoPanelWeaponViewModel.SetDamage(0);
            }
        }

        private void UpdateDurabilityInfo()
        {
            if (InfoPanelWeaponViewModel.Weapon.TryGetProperty("Durability", out var propertyDurability))
            {
                View.SetFillAmountWeaponDurability(GetAmountFill(GetGroup(propertyDurability.Float.Current, View.DurabilityGroupRanges)));
                InfoPanelWeaponViewModel.SetDurability(propertyDurability.Float.Current);
            }
            else
            {
                View.SetFillAmountWeaponDurability(0);
                InfoPanelWeaponViewModel.SetDurability(0);
            }
        }

        protected override void Hide()
        {

        }

        private int GetGroup(int value, int[] ranges)
        {
            if (value < 0)
            {
                return 0;
            }

            var last = 0;
            var count = ranges.Length;

            for (int i = 0; i < count; i++)
            {
                if (last <= value && value < ranges[i])
                {
                    return i + 1;
                }
                last = ranges[i];
            }

            return count;
        }

        private int GetGroup(float value, float[] ranges)
        {
            if (value < 0)
            {
                return 0;
            }

            var last = 0f;
            var count = ranges.Length;

            for (int i = 0; i < count; i++)
            {
                if (last <= value && value < ranges[i])
                {
                    return i + 1;
                }
                last = ranges[i];
            }

            return count;
        }

        private float GetAmountFill(int index) => View.AmountFill[index];
    }
}

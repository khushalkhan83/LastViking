using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class InfoPanelToolViewController : ViewControllerBase<InfoPanelToolView>
    {
        [Inject] public InfoPanelToolViewModel InfoPanelToolViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public AbilityProvider AbilityProvider { get; private set; }

        protected override void Show()
        {
            InfoPanelToolViewModel.Tool.TryGetProperty("FirstAbilityID", out var firstAbilityID);
            InfoPanelToolViewModel.Tool.TryGetProperty("SecondAbilityID", out var secondAbilityID);

            View.SetToolItemName(LocalizationModel.GetString(InfoPanelToolViewModel.TitleTextID));

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
                    View.SetToolItemDescription(LocalizationModel.GetString(firstAbility.LocalizationKeyID));
                }
                else
                {
                    View.SetToolItemDescription(null);
                }
            }
            else
            {
                View.SetVisibleFisrtAbiltity(false);
                View.SetToolItemDescription(null);
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
                    View.SetToolItemDescriptionSecond(LocalizationModel.GetString(secondAbility.LocalizationKeyID));
                }
                else
                {
                    View.SetToolItemDescriptionSecond(null);
                }
            }
            else
            {
                View.SetVisibleSecondAbility(false);
                View.SetToolItemDescriptionSecond(null);
            }

            if (firstAbilityID == null && secondAbilityID == null)
            {
                View.SetDecorActive(true);
            }
            else
            {
                View.SetDecorActive(false);
            }

            InfoPanelToolViewModel.SetMiningValue(0);
            InfoPanelToolViewModel.SetDurability(0);

            UpdateDamageInfo();
            UpdateDurabilityInfo();
        }

        private void UpdateDamageInfo()
        {
            if (InfoPanelToolViewModel.Tool.TryGetProperty("MiningWood", out var propertyMiningWood))
            {
                View.SetItemIconWood();
                View.SetFillAmountToolGather(GetAmountFill(GetGroup(propertyMiningWood.Int.Current, View.DamageGroupRanges)));
                InfoPanelToolViewModel.SetMiningValue(propertyMiningWood.Int.Current);
            }
            else if (InfoPanelToolViewModel.Tool.TryGetProperty("MiningStone", out var propertyMiningStone))
            {
                View.SetItemIconStone();
                View.SetFillAmountToolGather(GetAmountFill(GetGroup(propertyMiningStone.Int.Current, View.DamageGroupRanges)));
                InfoPanelToolViewModel.SetMiningValue(propertyMiningStone.Int.Current);
            }
            else
            {
                View.SetFillAmountToolGather(GetAmountFill(GetGroup(-1, View.DamageGroupRanges)));
                InfoPanelToolViewModel.SetMiningValue(0);
            }
        }

        private void UpdateDurabilityInfo()
        {
            if (InfoPanelToolViewModel.Tool.TryGetProperty("Durability", out var propertyDurability))
            {
                View.SetFillAmountToolDurability(GetAmountFill(GetGroup(propertyDurability.Float.Current, View.DurabilityGroupRanges)));
                InfoPanelToolViewModel.SetDurability(propertyDurability.Float.Current);
            }
            else
            {
                View.SetFillAmountToolDurability(0);
                InfoPanelToolViewModel.SetDurability(0);
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

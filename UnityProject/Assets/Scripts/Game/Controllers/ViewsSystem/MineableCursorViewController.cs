using Core;
using Core.Controllers;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class MineableCursorViewController : ViewControllerBase<MineableCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        protected override void Show()
        {
            var MineableObject = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<MineableObject>();

            switch (MineableObject.RequiredToolPurpose)
            {
                case FPTool.ToolPurpose.CutWood:
                    View.SetWoodToolIcon();
                    View.SetActiveToolIconHolder(false);

                    View.SetActiveHookMiniIcon(true);
                    View.SetActiveSecondMiniIcon(true);
                    View.SetSecondMiniIconAsAxe();
                    break;
                case FPTool.ToolPurpose.BreakRocks:
                    View.SetStoneToolIcon();
                    View.SetActiveToolIconHolder(false);

                    View.SetActiveHookMiniIcon(true);
                    View.SetActiveSecondMiniIcon(true);
                    View.SetSecondMiniIconAsPick();
                    break;
                case FPTool.ToolPurpose.Dig:
                    View.SetShovelToolIcon();
                    View.SetActiveToolIconHolder(false);

                    View.SetActiveHookMiniIcon(false);
                    View.SetActiveSecondMiniIcon(true);
                    View.SetSecondMiniIconAsShovel();
                    break;
                default:
                    break;
            }
        }

        protected override void Hide()
        {
        }
    }
}

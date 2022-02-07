using Core;
using Game.Models;
using UnityEngine;

namespace Game.Objectives.Actions.Controllers
{
    public class OutLineMineableStoneActionDataController : BaseActionController<OutLineMineableStoneActionData>
    {
        [Inject] public MinebleElementsModel MinebleElementsModel { get; private set; }
        [Inject] public OutLineCoconutModel OutLineCoconutModel { get; private set; }
        [Inject] public OutLineBlueBarrelModel OutLineBlueBarrelModel { get; private set; }
        [Inject] public OutLineBagPickupModel OutLineBagPickupModel { get; private set; }
        [Inject] public OutLineBanana OutLineBananaModel { get; private set; }

        protected override void Action(OutLineMineableStoneActionData actionData)
        {
            switch (actionData.OutLineTarget)
            {
                case OutLineTarget.Coconuts:
                    if (actionData.IsSelection)
                    {
                        OutLineCoconutModel.Select();
                    }
                    else
                    {
                        OutLineCoconutModel.Deselect();
                    }
                    break;
                case OutLineTarget.BlueBarrels:
                    if (actionData.IsSelection)
                    {
                        OutLineBlueBarrelModel.Select();
                    }
                    else
                    {
                        OutLineBlueBarrelModel.Deselect();
                    }
                    break;

                case OutLineTarget.Banana:
                    Debug.Log("Banana Action! "+ actionData.IsSelection);
                    if (actionData.IsSelection)
                    {
                        OutLineBananaModel.Select();
                    }
                    else
                    {
                        OutLineBananaModel.Deselect();
                    }
                    break;
                case OutLineTarget.BagPickups:
                    if (actionData.IsSelection)
                    {
                        OutLineBagPickupModel.Select();
                    }
                    else
                    {
                        OutLineBagPickupModel.Deselect();
                    }
                    break;
            }
        }
    }
}

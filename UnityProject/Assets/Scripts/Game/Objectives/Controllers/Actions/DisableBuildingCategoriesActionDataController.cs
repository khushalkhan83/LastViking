using System.Collections;
using System.Collections.Generic;
using Core;
using Game.Models;
using UnityEngine;
using System.Linq;

namespace Game.Objectives.Actions.Controllers
{
    public class DisableBuildingCategoriesActionDataController : BaseActionController<DisableBuildingCategoriesActionData>
    {
        [Inject] public BuildingHotBarModel BuildingHotBarModel { get; private set; }

        protected override void Action(DisableBuildingCategoriesActionData actionData)
        {
            BuildingHotBarModel.SetDisabledCategories(actionData.Categories);
        }
    }
}

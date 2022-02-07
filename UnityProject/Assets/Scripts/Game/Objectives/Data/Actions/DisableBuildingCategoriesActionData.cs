using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Game.Objectives.Actions
{
    public class DisableBuildingCategoriesActionData : ActionBaseData
    {
        [SerializeField] public List<ConstructionCategoryID> _categories = default;

        public List<ConstructionCategoryID> Categories => _categories;

        public override ActionID ActionID => ActionID. DisableBuildingCategories;
    }
}

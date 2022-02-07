using System;
using System.Collections;
using Game.Models;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class DockConstruction : MonoBehaviour
    {
        private ConstructionDockModel ConstructionDockModel => ModelsSystem.Instance._constructionDockModel;
        
        private void OnEnable()
        {
            ConstructionDockModel.NeedBuildDock = true;
        }

        private void OnDisable()
        {
            ConstructionDockModel.NeedBuildDock = false;
        }
    }
}
using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Game.Controllers.PlayerScenesController;

namespace DebugActions
{
    // public class SceneActionSetSceneUnloadType : ActionBase
    // {
    //     private readonly string _operationName;
    //     private readonly UnloadType _unloadType;

    //     private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

    //     public SceneActionSetSceneUnloadType(string name, UnloadType unloadType)
    //     {
    //         _operationName = name;
    //         _unloadType = unloadType;
    //     }
    //     public override string OperationName => _operationName + ": " + _unloadType.ToString();

    //     public override void DoAction()
    //     {
    //         PlayerScenesModel.UnloadType = _unloadType;
    //     }
    // }
}
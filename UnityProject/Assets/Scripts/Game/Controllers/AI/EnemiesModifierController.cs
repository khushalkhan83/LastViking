using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class EnemiesModifierController : IEnemiesModifierController, IController
    {
        [Inject] public EnemiesModifierModel Model { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }

        void IController.Enable()
        {
            try
            {
                Model.ApplyPresetPerDay((int)GameTimeModel.Days);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
        }
    }
}
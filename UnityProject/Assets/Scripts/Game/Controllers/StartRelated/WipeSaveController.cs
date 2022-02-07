using System;
using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine.SceneManagement;

namespace Game.Controllers
{
    public class WipeSaveController : IWipeSaveController, IController
    {
        [Inject] public WipeSaveModel WipeSaveModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        
        void IController.Enable() 
        {
            WipeSaveModel.Init();
            TutorialModel.Init();

            if(!WipeSaveModel.TryPerformAction) return;

            if(TutorialModel.IsComplete)
            {
                WipeSave();
                return;
            }

            WipeSaveModel.TryPerformAction = false;
        }


        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
        }


        private void WipeSave()
        { 
            StorageModel.ClearAll();
            SceneManager.LoadScene("LoadingScene");
        }
    }
}

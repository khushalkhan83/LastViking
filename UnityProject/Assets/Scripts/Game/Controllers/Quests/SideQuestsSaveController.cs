using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Helpers;

namespace Game.Controllers
{
    public class SideQuestsSaveController : ISideQuestsSaveController, IController
    {
        [Inject] public SideQuestsSaveModel SideQuestsSaveModel { get; private set; }
        [Inject] public SideQuestsModel SideQuestsModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public SideQuestsProvider SideQuestsProvider { get; private set; }
        void IController.Enable() 
        {
            bool debug = SideQuestsSaveModel.DebugMode && ApplicationHelper.IsEditorApplication();
            if(!debug)
            {
                bool dataExist = StorageModel.TryProcessing(SideQuestsSaveModel._Data);
                if(!dataExist)
                    SideQuestsSaveModel.SetDefaultProgress();

                SideQuestsModel.SetProgress(SideQuestsSaveModel.GetProgress());
            }
            

            SideQuestsModel.OnDataChanged += OnDataChanged;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            SideQuestsModel.OnDataChanged -= OnDataChanged;
        }

        private void OnDataChanged()
        {
            SideQuestsSaveModel.SaveProgress(SideQuestsModel.GetProgress());
        }
    }
}

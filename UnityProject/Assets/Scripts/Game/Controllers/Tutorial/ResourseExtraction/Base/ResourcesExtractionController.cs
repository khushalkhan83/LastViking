using Core;
using Core.Controllers;
using Game.Models;
using static UltimateSurvival.FPTool;

namespace Game.Controllers
{
    public abstract class ResourcesExtractionController : IController
    {
        [Inject] public ResourcesExtractionModel Model {get; private set;}
        [Inject] public TutorialResourcesModel TutorialResourcesModel {get; private set;}

        protected abstract ToolPurpose toolPurpose {get;}
        protected abstract int modifier {get;}

        void IController.Enable() 
        {
            Model.SetModifier(toolPurpose,modifier);
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            Model.RemoveModifier(toolPurpose);
        }
    }
}
using Core;
using Game.Models;

namespace Game.Controllers
{
    public class TutorialOutlineMineableTreeController : TutorialOutlineMineableControllerBase, ITutorialOutlineMineableTreeController
    {
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }

        protected override OutLineMinableObjectID outLineMinableObjectID => OutLineMinableObjectID.Tree;

        public override void Enable()
        {
            base.Enable();
            WorldObjectsModel.OnAdd.AddListener(WorldObjectID.food_coconut, OnAddCoconutHandler);
        }

        public override void Disable()
        {
            base.Disable();
            WorldObjectsModel.OnAdd.RemoveListener(WorldObjectID.food_coconut, OnAddCoconutHandler);
        }

        private void OnAddCoconutHandler(WorldObjectModel worldObjectModel)
        {
            DeselectAll();
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

    }
}

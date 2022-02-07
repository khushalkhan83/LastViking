using Core;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class TokenMineableTreeController : TokenMineableControllerBase, ITokenMineableTreeController
    {
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }

        protected override int TokenConfigId => 11;
        protected override string TockenID => "toeken_tree";
        protected override OutLineMinableObjectID outLineMinableObjectID => OutLineMinableObjectID.Tree;
        protected override Vector3 ShiftTokenPosition => Vector3.up * 1.5f;

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
            GameUpdateModel.OnUpdate -= OnUpdate;
            Hide();
        }

    }
}

using Core.StateMachine;
using Game.Models;
using UnityEngine;

namespace Game.AI.States.Effects.Zombie
{
    public class OnDeathEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649 

        [SerializeField] private WorldObjectModel _worldObjectModel;
#pragma warning restore 0649
        #endregion

        private WorldObjectModel WorldObjectModel => _worldObjectModel;

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public override void Apply()
        {
            WorldObjectModel.Delete();
        }
    }
}

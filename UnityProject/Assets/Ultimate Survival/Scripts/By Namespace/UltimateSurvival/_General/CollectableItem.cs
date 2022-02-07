using Game.Interactables;
using Game.Models;
using UnityEngine;
using UltimateSurvival;

namespace Game.Collectables
{
    public class CollectableItem : ItemsRelatedInteractableBase
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private string collectableID;
        
        #pragma warning restore 0649
        #endregion

        private MedallionsModel MedallionsModel => ModelsSystem.Instance._medallionsModel;

        private bool IsCollected => MedallionsModel.IsCollected(collectableID);


        #region MonoBehaviour
        private void Start()
        {
            if (IsCollected)
            {
                Deactivate();
            }
        }
        #endregion

        #region ItemsRelatedInteractableBase

        public override SavableItem[] RequiredItems => null;
        public override bool CanUse()
        {
            return !IsCollected;
        }

        public override void Use()
        {
            Collect();
            Deactivate();
        }

        #endregion

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void Collect()
        {
            MedallionsModel.SetCollected(collectableID);
        }
    }
}

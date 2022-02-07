using Game.Models;
using Game.StateMachine.Events;
using UnityEngine;

namespace Game.AI.Events.Animals
{
    public class OnTargetDeath : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Hit _hit;
        [SerializeField] private AnimalID _animalID;

#pragma warning restore 0649
        #endregion

        public Hit Hit => _hit;
        public AnimalID AnimalID => _animalID;
        public AnimalsModel AnimalsModel => ModelsSystem.Instance._animalsModel;

        private void OnEnable()
        {
            Hit.OnKillTarget += OnKillTarget;
        }

        private void OnDisable()
        {
            Hit.OnKillTarget -= OnKillTarget;
        }

        private void OnKillTarget(Target target) => AnimalsModel.AnimalKillTarget(AnimalID, target);
    }
}

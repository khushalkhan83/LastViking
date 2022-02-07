using Game.AI;
using UnityEngine;

namespace Game.Models
{
    public delegate void AnimalKillTarget(AnimalID animalID, Target target);
    public delegate void TargetKillAnimal(Target target, AnimalID animalID);

    public class AnimalsModel : MonoBehaviour
    {
        public event AnimalKillTarget OnAnimalKillTarget;
        public event TargetKillAnimal OnTargetKillAnimal;

        public void AnimalKillTarget(AnimalID animalID, Target target) => OnAnimalKillTarget?.Invoke(animalID, target);
        public void TargetKillAnimal(Target target, AnimalID animalID) => OnTargetKillAnimal?.Invoke(target, animalID);
    }
}

using Core;
using Game.AI;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class AnimalKillConditionController : BaseConditionController<AnimalKillConditionData, CountConditionDataModel>
    {
        [Inject] public AnimalsModel AnimalsModel { get; private set; }

        protected override void Subscribe()
        {
            AnimalsModel.OnTargetKillAnimal += OnTargetKillAnimalHandler;
        }

        protected override void Unsubscribe()
        {
            AnimalsModel.OnTargetKillAnimal -= OnTargetKillAnimalHandler;
        }

        private void OnTargetKillAnimalHandler(Target target, AnimalID animalID) => EventProcessing
            (
                data => data.AnimalID == animalID
                , model => model.Progress(1)
                , (data, model) => data.Value <= model.Value
            );
    }
}

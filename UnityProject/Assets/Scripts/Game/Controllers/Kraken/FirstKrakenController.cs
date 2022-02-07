using Core;
using Core.Controllers;
using Game.AI;
using Game.Models;

namespace Game.Controllers
{
    public class FirstKrakenController : IFirstKrakenController, IController
    {
        [Inject] public FirstKrakenModel FirstKrakenModel {get; private set;}
        [Inject] public StorageModel StorageModel {get; private set;}
        [Inject] public AnimalsModel AnimalsModel {get; private set;}
        
        void IController.Start() {}
        void IController.Enable() 
        {
            TryProcessData();
            AnimalsModel.OnTargetKillAnimal += OnTargetKillAnimalHandler;
            FirstKrakenModel.OnKrakenPreActivate += OnKrakenPreActivate;
        }


        void IController.Disable() 
        {
            AnimalsModel.OnTargetKillAnimal -= OnTargetKillAnimalHandler;
            FirstKrakenModel.OnKrakenPreActivate -= OnKrakenPreActivate;
        }

        private void OnTargetKillAnimalHandler(Target target, AnimalID animalID)
        {
            if (target && target.ID == TargetID.Player)
            {
                if(animalID == AnimalID.Kraken)
                    FirstKrakenModel.SetKrakenKilled();
            }
        }

        private void TryProcessData()
        {
            StorageModel.TryProcessing(FirstKrakenModel.FirstKrakenData);
        }

        private void OnKrakenPreActivate()
        {
            FirstKrakenModel.SetKrakenHealth(FirstKrakenModel.HealthOnStart);
            FirstKrakenModel.SetKrakenMaxHealth(FirstKrakenModel.MaxHealth);
        }
    }
}

using Core;
using Core.Controllers;
using Game.AI;
using Game.Models.AI;
using Game.Models;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Game.VillageBuilding;

namespace Game.Controllers
{
    public class TargetsController : ITargetsController, IController
    {
        [Inject] public TargetsModel TargetsModel {get; private set;}
        [Inject] public WorldObjectsModel WorldObjectsModel {get; private set;}
        [Inject] public PlayerScenesModel PlayerScenesModel {get; private set;}
        [Inject] public VillageBuildingModel VillageBuildingModel {get; private set;}

        private IEnumerable defenseIds;
        private IEnumerable DefenseIds
        {
            get => defenseIds ?? (defenseIds = Helpers.EnumsHelper.GetValues<WorldObjectID>().ToList().Where(x => x.ToString().Contains("defense")));
        }

        private HouseBuilding Townhall => VillageBuildingModel.Townhall;

        void IController.Enable() 
        {
            AddExistingDefenceTargets();
            UpdateBuildingTargets();
            Sub();
           
        }
        void IController.Start() 
        {

        }

        void IController.Disable() 
        {
            UnSub();
        }

        private void AddExistingDefenceTargets()
        {
            foreach(WorldObjectID defenseId in DefenseIds)
            {
                if(WorldObjectsModel.SaveableObjectModels.TryGetValue(defenseId, out var worldObjects))
                {
                    foreach(var model in  worldObjects)
                    {
                        TargetsModel.DefenceTargets.Add(model.GetComponentInChildren<Target>());
                    }
                }
            }
        }

        private void Sub()
        {
            foreach (WorldObjectID defenseId in DefenseIds)
            {
                WorldObjectsModel.OnAdd.AddListener(defenseId, OnAddDefense);
                WorldObjectsModel.OnRemove.AddListener(defenseId, OnRemoveDefense);
            }
            TargetsModel.OnTownHallPartTargets += GetTownHallPartTargets;
            TargetsModel.GetHasTargetsForAttack += GetHasTargetsForAttack;

            PlayerScenesModel.OnEnvironmentChange += OnEnvironmentChange;
            VillageBuildingModel.OnHouseStateChanged += OnHouseStateChanged;
        }

        private void UnSub()
        {
            foreach (WorldObjectID defenseId in DefenseIds)
            {
                WorldObjectsModel.OnAdd.RemoveListener(defenseId, OnAddDefense);
                WorldObjectsModel.OnRemove.RemoveListener(defenseId, OnRemoveDefense);
            }
            TargetsModel.OnTownHallPartTargets -= GetTownHallPartTargets;
            TargetsModel.GetHasTargetsForAttack -= GetHasTargetsForAttack;

            PlayerScenesModel.OnEnvironmentChange -= OnEnvironmentChange;
            VillageBuildingModel.OnHouseStateChanged -= OnHouseStateChanged;
        }
        private void OnAddDefense(WorldObjectModel model)
        {
            TargetsModel.DefenceTargets.Add(model.GetComponentInChildren<Target>());
        }
        private void OnRemoveDefense(WorldObjectModel model)
        {
            TargetsModel.DefenceTargets.Remove(model.GetComponentInChildren<Target>());
        }

        private void OnEnvironmentChange()
        {
            TargetsModel.DefenceTargets.Clear();
        }

        private void OnHouseStateChanged()
        {
            UpdateBuildingTargets();
        }

        private void UpdateBuildingTargets()
        {
            TargetsModel.Walls.Clear();
            TargetsModel.Towers.Clear();
            foreach(var building in VillageBuildingModel.ActiveBuildings)
            {
                if(building.Level >= 1)
                {
                    if(building.Type == HouseBuildingType.Fence || building.Type == HouseBuildingType.Gate)
                    {
                        DestroyableBuilding destroyableBuilding = building.GetCurrentLevelDestroyableBuilding();
                        if(destroyableBuilding != null)
                            TargetsModel.Walls.Add(destroyableBuilding);
                    }
                    else if(building.Type == HouseBuildingType.Tower)
                    {
                        DestroyableBuilding destroyableBuilding = building.GetCurrentLevelDestroyableBuilding();
                        if(destroyableBuilding != null)
                            TargetsModel.Towers.Add(destroyableBuilding);
                    }
                }
            }
        }

        private List<Target> GetTownHallPartTargets()
        {
            List<Target> answer = new List<Target>();

            if(Townhall != null)
            {
                var targets = Townhall.GetComponentsInChildren<Target>();
                answer.AddRange(targets);
            }

            return answer;
        }

        private bool GetHasTargetsForAttack()
        {
            bool answer = TargetsModel.DefenceTargets.Count > 0 || TargetsModel.Walls.Count > 0 || TargetsModel.Towers.Count > 0 || TargetsModel.TownHallPartTargets.Count > 0;
            return answer;
        }
    }
}

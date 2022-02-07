using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using UltimateSurvival;
using UltimateSurvival.Building;
using Extensions;
using EasyBuildSystem.Runtimes.Internal.Builder;

namespace Game.Controllers
{
    public class ReplaceBuildingController : IReplaceBuildingController, IController
    {
        [Inject] public ReplaceBuildingModel ReplaceBuildingModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public BuildingProcessModel BuildingProcessModel { get; private set; }


        private PlayerBuilder playerBuilder;
        private int itemIdCurrent;


        void IController.Enable() 
        {
            playerBuilder = PlayerEventHandler.gameObject.GetComponent<PlayerBuilder>();

            ReplaceBuildingModel.OnReplaceBuilding += OnReplaceBuilding;
            ReplaceBuildingModel.OnReplaceConstruction += OnReplaceConstruction;
            ReplaceBuildingModel.OnPlaceReplacedBuilding += OnPlaceReplacedBuilding;
            ReplaceBuildingModel.OnPlaceReplacedConstruction += OnPlaceReplacedConstruction;
            ReplaceBuildingModel.OnReplaceBuildingActiveChanged += OnReplaceBuildingActiveChanged;
            ReplaceBuildingModel.OnReplaceConstructionActiveChanged += OnReplaceConstructionActiveChanged;
            PlayerHealthModel.OnDeath += OnDeath;
            PlayerScenesModel.OnPreEnvironmentChange += OnPreEnvironmentChange;
            PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;
            HotBarModel.OnChangeEquipItem += OnChangeEquipItem;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            ReplaceBuildingModel.OnReplaceBuilding -= OnReplaceBuilding;
            ReplaceBuildingModel.OnReplaceConstruction -= OnReplaceConstruction;
            ReplaceBuildingModel.OnPlaceReplacedBuilding -= OnPlaceReplacedBuilding;
            ReplaceBuildingModel.OnPlaceReplacedConstruction -= OnPlaceReplacedConstruction;
            ReplaceBuildingModel.OnReplaceBuildingActiveChanged -= OnReplaceBuildingActiveChanged;
            ReplaceBuildingModel.OnReplaceConstructionActiveChanged -= OnReplaceConstructionActiveChanged;
            PlayerHealthModel.OnDeath -= OnDeath;
            PlayerScenesModel.OnPreEnvironmentChange -= OnPreEnvironmentChange;
            PlayerMovementModel.OnChangeMovementID -= OnChangePlayerMovement;
            HotBarModel.OnChangeEquipItem -= OnChangeEquipItem;
        }

        private void OnReplaceBuilding()
        {
            ReplaceBuildingModel.SetReplaceBuildingActive(true);
            BuildingModeModel.BuildingActive = false;
            BuildingModeModel.HideSwitchButton = true;
            HidePreview();
            ShowPreview();
        }

        private void OnReplaceConstruction()
        {
            ReplaceBuildingModel.SetReplaceConstructionActive(true);
            BuildingModeModel.BuildingActive = false;
            BuildingModeModel.HideSwitchButton = true;
            ShowConstrcutionPreview();
        }

        private void OnPlaceReplacedBuilding()
        {
            if(playerBuilder.BuildingHelpers.IsPreviewExists && playerBuilder.BuildingHelpers.PlacementAllowed)
            {
                if(ReplaceBuildingModel.Building != null)
                {
                    ReplaceBuildingModel.Building.transform.position = playerBuilder.BuildingHelpers.CurrentPreview.transform.position;
                    ReplaceBuildingModel.Building.transform.rotation = playerBuilder.BuildingHelpers.CurrentPreview.transform.rotation;

                    var worldObjectModel = ReplaceBuildingModel.Building.GetComponent<WorldObjectModel>();
                    if(worldObjectModel != null)
                    {
                        worldObjectModel.SetPosition(worldObjectModel.transform.position);
                        worldObjectModel.SetRotation(worldObjectModel.transform.rotation);
                    }
                }
            }
            ReplaceBuildingModel.SetReplaceBuildingActive(false);
        }

        private void OnPlaceReplacedConstruction()
        {
            if(BuilderBehaviour.Instance.PreviewExists() && BuilderBehaviour.Instance.AllowPlacement)
            {
                if(ReplaceBuildingModel.ConstructionpPartBehaviour != null)
                {
                    ReplaceBuildingModel.ConstructionpPartBehaviour.transform.position = BuilderBehaviour.Instance.CurrentPreview.transform.position;
                    ReplaceBuildingModel.ConstructionpPartBehaviour.transform.rotation = BuilderBehaviour.Instance.CurrentPreview.transform.rotation;
                }

                var worldObjectModel = ReplaceBuildingModel.ConstructionpPartBehaviour.GetComponent<WorldObjectModel>();
                if(worldObjectModel != null)
                {
                    worldObjectModel.SetPosition(worldObjectModel.transform.position);
                    worldObjectModel.SetRotation(worldObjectModel.transform.rotation);
                }
            }
            ReplaceBuildingModel.SetReplaceConstructionActive(false);
        }

        private void OnReplaceBuildingActiveChanged()
        {
            if(!ReplaceBuildingModel.BuildingReplaceActive)
            {
                HidePreview();
                BuildingModeModel.HideSwitchButton = false;
            }
        }

        private void OnReplaceConstructionActiveChanged()
        {
            if(!ReplaceBuildingModel.ConstructionReplaceActive)
            {
                HideConstrcutionPreview();
                {
                    BuildingModeModel.HideSwitchButton = false;
                }
            }
        }

        private void OnDeath() =>  ReplaceBuildingModel.SetReplaceBuildingActive(false);

        private void OnPreEnvironmentChange() => ReplaceBuildingModel.SetReplaceBuildingActive(false);

        private void OnChangePlayerMovement()
        {
            if(PlayerMovementModel.MovementID == PlayerMovementID.Water)
            {
                ReplaceBuildingModel.SetReplaceBuildingActive(false);
            }
        }

        private void OnChangeEquipItem()
        {
            if(IsPlaceableItemEquiped())
            {
                ReplaceBuildingModel.SetReplaceBuildingActive(false);
            }
        }

        private bool IsPlaceableItemEquiped()
        {  
            var item = HotBarModel.EquipCell.Item;
            return item != null && item.Count > 0 && item.HasProperty("Is Placeable");
        }

        private void ShowPreview()
        {
            if(ReplaceBuildingModel.Building != null)
            {
                var itemData = ItemsDB.GetItem(ReplaceBuildingModel.Building.ItemID);
                if(itemData != null)
                {
                    ReplaceBuildingModel.Building.CheckNull()?.gameObject.SetActive(false);
                    playerBuilder.HidePreview();
                    playerBuilder.ShowPreview(itemData);
                }
            }
        }

        private void HidePreview()
        {
            ReplaceBuildingModel.Building.CheckNull()?.gameObject.SetActive(true);
            playerBuilder.HidePreview();
        }

        private void ShowConstrcutionPreview()
        {
            if(ReplaceBuildingModel.ConstructionpPartBehaviour != null)
            {
                ReplaceBuildingModel.ConstructionpPartBehaviour.gameObject.SetActive(false);
                BuildingProcessModel.SelectBuildItem(ReplaceBuildingModel.ConstructionpPartBehaviour.Id);
                BuildingProcessModel.StartBuild();
            }
        }

        private void HideConstrcutionPreview()
        {
            ReplaceBuildingModel.ConstructionpPartBehaviour.CheckNull()?.gameObject.SetActive(true);
            BuildingModeModel.BuildingActive = false;
            BuildingModeModel.BuildingActive = true;
        }

    }
}

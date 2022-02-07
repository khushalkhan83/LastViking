using System.Collections;
using System.Collections.Generic;
using Game.Models;
using Game.Views;
using Game.VillageBuilding;
using UnityEngine;

namespace Game.Controllers
{
    public class BuildingTokenController : MonoBehaviour
    {

        [SerializeField] private HouseBuilding houseBuilding;
        [SerializeField] private Transform pivot = default;

        [Range(0f, 100f)]
        [SerializeField] private float maxDistance = default;

        [Range(0f, 20)]
        [SerializeField] private float baseScale = default;


        private Transform _camera;
        private float sqrDistance;
        private Vector3 position;
        private Vector3 up;
        private Sprite icon;
        private float sqrMaxDistance;


        private VillageBuildingModel VillageBuildingModel => ModelsSystem.Instance._villageBuildingModel;
        private ViewsSystem ViewsSystem => ViewsSystem.Instance;
        private InventoryModel InventoryModel => ModelsSystem.Instance._inventoryModel;
        private HotBarModel HotBarModel => ModelsSystem.Instance._hotBarModel;
        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;

        private Transform Camera => _camera ?? (_camera = GameObject.FindGameObjectWithTag("MainCamera").transform);

        private BuildingTokenView view;


        private void OnEnable()
        {
            icon = VillageBuildingModel.GetHouseBuildingInfo(houseBuilding.Type).icon;
            sqrMaxDistance = maxDistance * maxDistance;
            UpdateTokenView();

            VillageBuildingModel.OnHouseStateChanged += UpdateTokenLockedColor;
            VillageBuildingModel.OnCitizensCountChanged += UpdateTokenLockedColor;
            InventoryModel.ItemsContainer.OnChangeCell += OnChangeInventoryCell;
            HotBarModel.ItemsContainer.OnChangeCell += OnChangeInventoryCell;
        }

        private void OnDisable() 
        {
            VillageBuildingModel.OnHouseStateChanged -= UpdateTokenLockedColor;
            VillageBuildingModel.OnCitizensCountChanged -= UpdateTokenLockedColor;
            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeInventoryCell;
            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeInventoryCell;
            HideView();
        }

        private void UpdateTokenView()
        {
            if(Camera == null)
                return;
            sqrDistance = (Camera.position - transform.position).sqrMagnitude;
            if (sqrDistance > sqrMaxDistance || !TutorialModel.IsComplete)
            {
                HideView();
            }
            else
            {
                ShowView();

                position = view.transform.position + Camera.rotation * Vector3.forward;
                up = Camera.rotation * Vector3.up;

                view.SetLocalPosition(Vector3.zero);
                view.SetScale(Vector3.one * baseScale);
                view.LookAt(position, up);
            }
        }

        private void UpdateTokenLockedColor()
        {
            if(view != null)
            {
                bool isUnlocked = VillageBuildingModel.CanUpgradeBuilding(houseBuilding);
                view.SetIsUnlockedColor(isUnlocked);
            }
        }

        private void OnChangeInventoryCell(CellModel cellModel) => UpdateTokenLockedColor();

        private void ShowView()
        {
            if (view == null)
            {
                view = ViewsSystem.Show<BuildingTokenView>(ViewConfigID.BuildingTokenConfig, pivot);
                view.SetIcon(icon);
                view.PlayBouncingAnimation();
                UpdateTokenLockedColor();
            }
        }

        private void HideView()
        {
            if (view != null)
            {
                view.StopBouncingAnimation();
                ViewsSystem.Hide(view);
                view = null;
            }
        }

        private void LateUpdate()
        {
            UpdateTokenView();
        }
    }
}

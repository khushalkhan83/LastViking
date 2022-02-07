using Game.Models;
using Game.Providers;
using UnityEngine;

namespace UltimateSurvival.Building
{
    /// <summary>
    /// Used for showing a buildable preview, rotating, snapping and placing.
    /// </summary>
    public class PlayerBuilder : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private BuildingHelpers m_BuildingHelpers;

        [SerializeField] private AudioSource m_AudioSource;

#pragma warning restore 0649
        #endregion

        public BuildingHelpers BuildingHelpers => m_BuildingHelpers;

        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private InventoryModel InventoryModel => ModelsSystem.Instance._inventoryModel;
        private HotBarModel HotBarModel => ModelsSystem.Instance._hotBarModel;
        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;
        private PlayerMovementModel PlayerMovementModel => ModelsSystem.Instance._playerMovementModel;
        private PrefabsProvider PrefabsProvider => ModelsSystem.Instance._prefabsProvider;
        private BuildingModeModel BuildingModeModel => ModelsSystem.Instance._buildingModel;

        private float m_NextTimeCanPlayAudio;

        public static bool close;
        private bool isMove;
        public int ItemIdCurrent { get; private set; }

        public GameObject house;
        private bool changePlayerTag;

        private void Start()
        {
            close = false;
            changePlayerTag = false;

            m_BuildingHelpers.Initialize(transform, PlayerEventHandler, m_AudioSource);

            PlayerEventHandler.PlaceObject.SetTryer(Try_Place);
            PlayerEventHandler.PlaceObject.AddListener(Place);

        }

        private void OnEnable()
        {
            HotBarModel.OnChangeEquipItem += OnChangedEquippedItemHandler;
            GameUpdateModel.OnUpdate += OnUpdate;
            PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;
            BuildingModeModel.BuildingEnabled += OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled += OnBuildingDisabled;
            PlayerEventHandler.RotateObject.SetTryer(Try_RotateObject);
        }

        private void OnDisable()
        {
            HotBarModel.OnChangeEquipItem -= OnChangedEquippedItemHandler;
            GameUpdateModel.OnUpdate -= OnUpdate;
            PlayerMovementModel.OnChangeMovementID -= OnChangePlayerMovement;
            BuildingModeModel.BuildingEnabled -= OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled -= OnBuildingDisabled;
        }

        private void OnChangedEquippedItemHandler() => UpdatePreviewVisible();
        private void OnBuildingEnabled() => HidePreview();
        private void OnBuildingDisabled() => UpdatePreviewVisible();

        private void UpdatePreviewVisible()
        {
            if (m_BuildingHelpers.IsPreviewExists && HotBarModel.EquipCell.IsHasItem && HotBarModel.EquipCell.Item.Id != ItemIdCurrent)
            {
                PlayerHealthModel.OnDeath -= OnDeathHandler;
                HidePreview();
            }

            if (HotBarModel.EquipCell.IsHasItem && HotBarModel.EquipCell.Item.TryGetProperty("Is Placeable", out var placeable))
            {
                if (!m_BuildingHelpers.IsPreviewExists)
                {
                    PlayerHealthModel.OnDeath += OnDeathHandler;
                    ShowPreview(HotBarModel.EquipCell.Item.ItemData);
                }
            }
            else if(m_BuildingHelpers.IsPreviewExists)
            {
                HidePreview();
            }
        }

        private void OnChangePlayerMovement()
        {
            switch (PlayerMovementModel.MovementID)
            {
                case PlayerMovementID.Ground:
                    OnChangedEquippedItemHandler();
                    break;
                case PlayerMovementID.Water:
                    HidePreview();
                    break;
                default:
                    break;
            }
        }

        public void ShowPreview(ItemData item)
        {
            var id = item.GetProperty("Is Placeable").PrefabID;
            if (id != PrefabID.None)
            {
                m_BuildingHelpers.SpawnPreview(PrefabsProvider[id]);
                ItemIdCurrent = item.Id;
            }
        }

        public void HidePreview()
        {
            m_BuildingHelpers.ClearPreview();
            ItemIdCurrent = -1;
        }

        private void OnDeathHandler()
        {
            PlayerHealthModel.OnDeath -= OnDeathHandler;
            HidePreview();
        }

        private bool Try_RotateObject(float rotationSign)
        {
            if (PlayerEventHandler.ViewLocked.Is(false) && m_BuildingHelpers.IsPreviewExists)
            {
                m_BuildingHelpers.RotationOffset = rotationSign;
                return true;
            }

            return false;
        }

        private bool Try_Place()
        {
            return m_BuildingHelpers.IsPreviewExists &&
                HotBarModel.EquipCell.IsHasItem &&
                HotBarModel.EquipCell.Item.HasProperty("Is Placeable") &&
                m_BuildingHelpers.PlacementAllowed;
        }

        private void Place()
        {
            m_BuildingHelpers.PlacePiece();
            m_BuildingHelpers.CurrentPreviewPiece.BuildAudio.Play(ItemSelectionMethod.RandomlyButExcludeLast, m_AudioSource);
        }

        private void OnUpdate()
        {
            m_BuildingHelpers.HasSocket = false;

            if (m_BuildingHelpers.IsPreviewExists)
                m_BuildingHelpers.LookForSnaps();

            if (m_BuildingHelpers.IsPreviewExists)
                m_BuildingHelpers.ManagePreview();

            if (!changePlayerTag && house == null)
            {
                changePlayerTag = true;
                gameObject.tag = "Target";
            }
        }

        public bool CanPlace()
        {
            return m_BuildingHelpers.PlacementAllowed;
        }
    }
}

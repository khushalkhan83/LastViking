using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using Game.Providers;
using UnityEngine;

namespace Game.Controllers
{
    public class ShelterController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject _default;
        [SerializeField] private ShelterLevelView[] _levels;
        [SerializeField] private ShelterView _shelterView;
        [ObscuredID(typeof(ShelterModelID))]
        [SerializeField] private ObscuredInt _shelterModelID;

#pragma warning restore 0649
        #endregion

        private ShelterView ShelterView => _shelterView;
        private ShelterModelID ShelterModelID => (ShelterModelID)(int)_shelterModelID;
        private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;
        private ShelterModelsProvider ShelterModelsProvider => ModelsSystem.Instance._shelterModelsProvider;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private ConstructionDockModel ConstructionDockModel => ModelsSystem.Instance._constructionDockModel;
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;

        public ShelterModel ShelterModel => ShelterModelsProvider[ShelterModelID];

        public ShelterLevelView ViewLevelCurrent { get; private set; }
        public GameObject ViewObjctCurrent { get; private set; }

        public ShelterLevelView GetViewByLevel(int level) => _levels[level];

        private void Start()
        {
            SetupShelterView();

            if (SheltersModel.IsBuyed(ShelterModelID))
            {
                SetCorePosition();
            }
        }

        private void OnEnable()
        {
            ShelterModel.OnBuy += OnBuyShelterHandler;
            ShelterModel.OnPreUpgrade += OnPreUpgradeShelterHandler;
            ShelterModel.OnDeath += OnShelterDeathHandler;
            ConstructionDockModel.OnDockBuilded += OnDockBuilded;
            QuestsModel.OnActivateQuest += OnActivateQuest;
        }

        private void OnDisable()
        {
            ShelterModel.OnBuy -= OnBuyShelterHandler;
            ShelterModel.OnPreUpgrade -= OnPreUpgradeShelterHandler;
            ShelterModel.OnDeath -= OnShelterDeathHandler;
            ConstructionDockModel.OnDockBuilded -= OnDockBuilded;
            QuestsModel.OnActivateQuest -= OnActivateQuest;
        }

        private void SetupShelterView()
        {
            if(ConstructionDockModel.DockBuilded)
            {
                if (SheltersModel.IsBuyed(ShelterModelID))
                {
                    SetView(ShelterModel.Level);
                }
                else
                {
                    SetViewDefault();
                }
            }
            else if (ViewObjctCurrent)
            {
                Destroy(ViewObjctCurrent);
            }
        }

        private void OnActivateQuest()
        {
            var chapter = QuestsModel.GetChapter();
            if(ShelterModel.Level != chapter)
            {
                ShelterModel.Temp_SetLevel(chapter);
                SetupShelterView();
            }
        }

        private void OnPreUpgradeShelterHandler()
        {
            UpdateShelterLevel();
        }

        private void OnBuyShelterHandler()
        {
            SetView(ShelterModel.Level);
            SetCorePosition();
        }

        private void OnShelterDeathHandler()
        {
            SetView(ShelterModel.Level);
        }

        private void UpdateShelterLevel()
        {
            var health = ViewObjctCurrent.GetComponent<IHealth>();
            var healthPercent = health.Health / health.HealthMax;

            SetView(ShelterModel.Level);
            SetCorePosition();

            health = ViewObjctCurrent.GetComponent<IHealth>();
            health.SetHealth(health.HealthMax * healthPercent);
        }

        private void SetCorePosition()
        {
            ShelterModel.SetLevelData(ShelterView.Container.position + GetViewByLevel(ShelterModel.Level).ContainerTarget.position);
        }

        private void SetViewDefault()
        {
            if (ViewObjctCurrent)
            {
                Destroy(ViewObjctCurrent);
            }
            ViewObjctCurrent = Instantiate(_default, ShelterView.Container);
            ViewLevelCurrent = null;
        }

        private void OnDockBuilded()
        {
            SetupShelterView();
        }

        private void SetView(int level)
        {
            if (ViewObjctCurrent)
            {
                Destroy(ViewObjctCurrent);
            }
            ViewLevelCurrent = Instantiate(GetViewByLevel(level), ShelterView.Container);
            ViewObjctCurrent = ViewLevelCurrent.gameObject;
        }
    }
}

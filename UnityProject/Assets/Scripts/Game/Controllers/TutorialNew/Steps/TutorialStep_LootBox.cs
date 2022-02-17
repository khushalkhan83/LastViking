using Core;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_LootBox : TutorialStepBase
    {
        [Inject] public InventoryLootViewModel ViewModel { get; private set; }
        [Inject] public LootGroupModel LootGroupModel { get; private set; }
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel { get; private set; }
        private TutorialLootBoxSpawner TutorialLootBoxSpawner => TutorialLootBoxSpawner.Instance;

        public TutorialStep_LootBox(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        private ITutorialStep stepTapHint;

        public override void OnStart()
        {
            ViewModel.OnShowChanged += UpdateHilight;
            TutorialLootBoxSpawner.OnLootBoxCollected += CheckConditions;

            if(TutorialLootBoxSpawner.Inited)
            {
                TutorialLootBoxSpawner.TrySpawnLootBox();
            }
            else
            {
                TutorialLootBoxSpawner.OnInit += OnSpawnerInit;
            }

            InitState();
            ProcessState();
        }

        public override void OnEnd()
        {
            ViewModel.OnShowChanged -= UpdateHilight;
            TutorialLootBoxSpawner.OnLootBoxCollected -= CheckConditions;  
            TutorialLootBoxSpawner.OnInit -= OnSpawnerInit;

            UpdateHilight(forceDisableHilight: true);
            stepTapHint.Exit();
        }

        private void InitState()
        {
            ShowTaskMessage(true, LocalizationModel.GetString(LocalizationKeyID.Tutorial_Get_Items), TutorialLootBoxSpawner.LootBoxIcon);
            stepTapHint = new TutorialStep_TapHint(null,ShowTapCondition, null);
            InjectionSystem.Inject(stepTapHint);
            stepTapHint.Enter();
        }

        private void ProcessState()
        {
            UpdateHilight();
            CheckConditions();
        }

        private void OnSpawnerInit()
        {
            TutorialLootBoxSpawner.TrySpawnLootBox();
            ProcessState();
        }

        private bool IsTargetSelected()
        {
            var activeLoot = LootGroupModel.ActiveLoot;
            if(activeLoot == null) return false;

            bool asnwer = TutorialLootBoxSpawner.IsSpawnedLootObject(activeLoot);
            return asnwer;
        }

        private void UpdateHilight() => UpdateHilight(false);

        private void UpdateHilight(bool forceDisableHilight = false)
        {
            bool show = forceDisableHilight ? false : ViewModel.IsShow && IsTargetSelected();
            TutorialSimpleDarkViewModel.SetShow(show);

            if(show)
            {
                TutorialSimpleDarkViewModel.PlayAnimation();
            }

            ViewModel.SetTakeAllButtonHilight(show);
        }

        private void CheckConditions()
        {
            bool nextStep = TutorialLootBoxSpawner.IsLootBoxCollected;

            if(nextStep) TutorialNextStep();
        }

        private bool ShowTapCondition(GameObject raycastGameObject)
        {
            return raycastGameObject != null && IsLootObject();

            bool IsLootObject()
            {
                return raycastGameObject.TryGetComponent<LootObject>(out var lootObject);
            }
        }
    }
}

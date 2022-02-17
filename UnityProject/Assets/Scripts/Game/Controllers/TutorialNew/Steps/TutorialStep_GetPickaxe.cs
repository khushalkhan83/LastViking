using Core;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_GetPickaxe : TutorialStepBase
    {
        [Inject] public ItemsDB ItemsDB { get; private set; }

        private ITutorialStep stepTakeItem;
        private ITutorialStep stepTapHint;

        private const string pickaxeItemId = "tool_pickaxe_stone";
        private const int tokenConfigId = 13;
        private Sprite icon;

        private readonly TutorialEvent approchedBarrel;
        private readonly TutorialEvent approachedPickaxe;
        private readonly TutorialEvent tookPickaxe;

        public TutorialStep_GetPickaxe(TutorialEvent StepStartedEvent, TutorialEvent approchedBarrel,TutorialEvent approachedPickaxe, TutorialEvent tookPickaxe) : base(StepStartedEvent)
        {
            this.approchedBarrel = approchedBarrel;
            this.approachedPickaxe = approachedPickaxe;
            this.tookPickaxe = tookPickaxe;
        }

        public override void OnStart()
        {
            Init();
            
            stepTakeItem.Enter();
            stepTapHint.Enter();
            ShowNotification();
        }
        public override void OnEnd()
        {
            stepTakeItem.Exit();
            stepTapHint.Exit();
            stepTakeItem = null;
        }

        private void Init()
        {
            stepTakeItem = new TutorialStep_TakeItem(null,CheckConditions,pickaxeItemId,tokenConfigId);
            InjectionSystem.Inject(stepTakeItem);

            stepTapHint = new TutorialStep_TapHint(null,ShowTapCondition, null);
            InjectionSystem.Inject(stepTapHint);

            icon = ItemsDB.ItemDatabase.GetItemByName(pickaxeItemId).Icon;
        }

        private void CheckConditions()
        {
            bool nextStep = true;

            TryEquipPickaxe();
            tookPickaxe.Fire();

            if(nextStep) TutorialNextStep();
        }

        private void TryEquipPickaxe()
        {
            InventoryOperationsModel.TryEquipItem(pickaxeItemId);
        }

        private void ShowNotification()
        {
            ShowTaskMessage(true, LocalizationModel.GetString(LocalizationKeyID.Tutorial_Task01_Start), icon);
        }

        private bool ShowTapCondition(GameObject raycastGameObject)
        {
            return raycastGameObject != null && (IsTutorialBarrel() ||  IsPickaxePickup());

            bool IsTutorialBarrel()
            {
                bool answer = raycastGameObject.TryGetComponent<DropItemTutorialHandler>(out var handler);
                if(answer == true) approchedBarrel.Fire();
                return answer;
            }

            bool IsPickaxePickup()
            {
                if(raycastGameObject.TryGetComponent<ItemPickup>(out var itemPickup))
                {
                    var answer = itemPickup.ItemToAdd.Name.Equals(pickaxeItemId);
                    if(answer == true) approachedPickaxe.Fire();
                    return answer;
                }
                return false;
            }
        }

    }
}

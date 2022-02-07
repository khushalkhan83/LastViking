using System.Collections;
using System.Collections.Generic;
using Core.Storage;
using Game.Models;
using Game.QuestSystem.Map.Extra;
using Game.Views;
using Game.VillageBuilding;
using UnityEngine;
using System.Linq;
using ItemConfig = Game.Models.InventoryOperationsModel.ItemConfig;
using UltimateSurvival;
using System;
using Game.Controllers;
using Core.Views;

public class SimpleTutorialController : MonoBehaviour
{
    [Serializable]
    public class Data : DataBase
    {
        public int tutorialStep = 0;
        public bool isComplete = false;

        public void SetTutorialStep(int step)
        {
            this.tutorialStep = step;
            ChangeData();
        }

        public void SetIsComplete(bool isComplete)
        {
            this.isComplete = isComplete;
            ChangeData();
        }
    }

    [SerializeField] public Data data = default;
    [SerializeField] public HouseBuilding townhall = default;
    [SerializeField] public Sprite townhallIcon = default;
    [SerializeField] public Sprite stoneIcon = default;
    [SerializeField] public Sprite woodIcon = default;

    public HouseBuildingInfo houseBuildingInfo;
    public HouseLevelInfo levelOneInfo;
    public int stonePrice;
    public int woodPrice;
    public int tokenConfigId;
    public string tockenID;
    public  OutLineMinableObjectID outLineMinableObjectID;
    private MinebleElement currentTokenTarget = null;
    private IView view;

    #region Prototype stuff

    public GameObject firePlace;
    public GameObject rabbit;
    public GameObject woolf;
        
    #endregion


    private VillageBuildingModel VillageBuildingModel => ModelsSystem.Instance._villageBuildingModel;
    public HouseBuilding Townhall => townhall;
    private ViewsSystem ViewsSystem => ViewsSystem.Instance;
    private InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;
    private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
    private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
    private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
    private MinebleElementsModel MinebleElementsModel => ModelsSystem.Instance._minebleElementsModel;
    private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
    private TokensModel TokensModel => ModelsSystem.Instance._tokensModel;
    private InventoryModel InventoryModel => ModelsSystem.Instance._inventoryModel;
    private HotBarModel HotBarModel => ModelsSystem.Instance._hotBarModel;
    private QuestNotificationsModel QuestNotificationsModel => ModelsSystem.Instance._questNotificationsModel;
    private NotificationContainerViewModel NotificationContainerViewModel => ModelsSystem.Instance._notificationContainerViewModel;



    public int TutorialStep
    {
        get{return data.tutorialStep;}
        private set{ data.SetTutorialStep(value);}
    }
    public bool IsComplete
    {
        get{return data.isComplete;}
        private set{data.SetIsComplete(true);}
    }
    

    private void OnEnable() 
    {
        // StorageModel.TryProcessing(data);

        // if(IsComplete)
        //     return;

        // InitTutorial();
    }

    private void OnDisable() 
    {
        
    }

    private void InitTutorial()
    {
        ShowNotificationView();
        houseBuildingInfo = VillageBuildingModel.GetHouseBuildingInfo(HouseBuildingType.TownHall);
        levelOneInfo = houseBuildingInfo.GetLevelInfo(1);
        stonePrice = levelOneInfo.requiredItems.FirstOrDefault(r => r.Name.Equals("Stone")).Amount;
        woodPrice = levelOneInfo.requiredItems.FirstOrDefault(r => r.Name.Equals("Wood")).Amount;

        if(TutorialStep == 0) InitStep0();
        else if(TutorialStep == 1) InitStep1();
        else if(TutorialStep == 2) InitStep2();
        else if(TutorialStep == 3) InitStep3();

        Townhall.OnCompleteBuildingProcess += OnCompleteBuildingProcess;
    }


#region Step0

    private void InitStep0()
    {
        QuestNotificationsModel.Show("Builid Towhall", townhallIcon);
        Townhall.GetComponent<TokenTarget>().enabled = true;
        ViewsSystem.OnBeginShow.AddListener(ViewConfigID.HouseBuildingConfig, OnOpenHouseBuildingViewStep);
    }

    private void OnOpenHouseBuildingViewStep()
    {
        ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.HouseBuildingConfig, OnOpenHouseBuildingViewStep);
        CompleteStep0();
    }

    private void CompleteStep0()
    {
        Townhall.GetComponent<TokenTarget>().enabled = false;
        TutorialStep = 1;
        InitStep1();
    }

#endregion


#region Step1

    private void InitStep1()
    {
        if(PlayerHasItems("Stone", stonePrice))
        {
            CompleteStep1();
        }
        else
        {
            QuestNotificationsModel.Show("Mine stone to build Townhall", stoneIcon);
            tokenConfigId = 12;
            tockenID = "toeken_stone";
            outLineMinableObjectID = OutLineMinableObjectID.Stone;
            GameUpdateModel.OnUpdate += UpdateToken;
            InventoryModel.ItemsContainer.OnAddItems += OnAddItemToPlayerStep1;
            HotBarModel.ItemsContainer.OnAddItems += OnAddItemToPlayerStep1;
        }
    }

    private void OnAddItemToPlayerStep1(int itemId, int count)
    {
        if(PlayerHasItems("Stone", stonePrice))
        {
            InventoryModel.ItemsContainer.OnAddItems -= OnAddItemToPlayerStep1;
            HotBarModel.ItemsContainer.OnAddItems -= OnAddItemToPlayerStep1;
            CompleteStep1();
        }
    }
    

    private void CompleteStep1()
    {
        GameUpdateModel.OnUpdate -= UpdateToken;
        TokensModel.HideToken(tockenID);
        TutorialStep = 2;
        InitStep2();
    }

#endregion


#region Step2

    private void InitStep2()
    {
        if(PlayerHasItems("Wood", woodPrice))
        {
            CompleteStep2();
        }
        else
        {
            QuestNotificationsModel.Show("Mine wood to build Townhall", woodIcon);
            tokenConfigId = 11;
            tockenID = "toeken_tree";
            outLineMinableObjectID = OutLineMinableObjectID.Tree;
            GameUpdateModel.OnUpdate += UpdateToken;
            InventoryModel.ItemsContainer.OnAddItems += OnAddItemToPlayerStep2;
            HotBarModel.ItemsContainer.OnAddItems += OnAddItemToPlayerStep2;
        }
    }

    private void OnAddItemToPlayerStep2(int itemId, int count)
    {
        if(PlayerHasItems("Wood", woodPrice))
        {
            InventoryModel.ItemsContainer.OnAddItems -= OnAddItemToPlayerStep2;
            HotBarModel.ItemsContainer.OnAddItems -= OnAddItemToPlayerStep2;
            CompleteStep2();
        }
    }


    private void CompleteStep2()
    {
        GameUpdateModel.OnUpdate -= UpdateToken;
        TokensModel.HideToken(tockenID);
        TutorialStep = 3;
        InitStep3();
    }

#endregion


#region Step3

    private void InitStep3()
    {
        QuestNotificationsModel.Show("Not you have resources for Townhall. Build it!", townhallIcon);
        Townhall.GetComponent<TokenTarget>().enabled = true;
    }

#endregion

    private void OnCompleteBuildingProcess(HouseBuilding houseBuilding) => CompleteTutorial();

    public void CompleteTutorial()
    {
        IsComplete = true;
        QuestNotificationsModel.Hide();
        HideNotifictionView();
        NotificationContainerViewModel.Show(PriorityID.Tutorial, ViewConfigID.TutorialCompleted, new TutorialCompletedControllerData(LocalizationKeyID.Tutorial_Completed, 5));
        Townhall.GetComponent<TokenTarget>().enabled = false;
        ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.HouseBuildingConfig, OnOpenHouseBuildingViewStep);
        InventoryModel.ItemsContainer.OnAddItems -= OnAddItemToPlayerStep1;
        HotBarModel.ItemsContainer.OnAddItems -= OnAddItemToPlayerStep1;
        InventoryModel.ItemsContainer.OnAddItems -= OnAddItemToPlayerStep2;
        HotBarModel.ItemsContainer.OnAddItems -= OnAddItemToPlayerStep2;
        GameUpdateModel.OnUpdate -= UpdateToken;
        Townhall.OnCompleteBuildingProcess -= OnCompleteBuildingProcess;
    }

    private void UpdateToken()
    {
        if(MinebleElementsModel.OutlineObjects.TryGetValue(outLineMinableObjectID, out List<MinebleElement> elements))
        {
            MinebleElement closestTarget = null;
            float closestSqrDistance = float.MaxValue;
            foreach (var element in elements)
            {
                if(element.MinableFractures.FirstOrDefault()?.ResourceValue <= 0)
                    continue;

                float sqrDistance = (PlayerEventHandler.transform.position - element.transform.position).sqrMagnitude;
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestTarget = element;
                }
            }

            if (currentTokenTarget != closestTarget)
            {
                currentTokenTarget = closestTarget;
                TokensModel.HideToken(tockenID);
                if (currentTokenTarget != null)
                {
                    Vector3 shiftPosition = tockenID.Equals("toeken_stone") ? Vector3.up : Vector3.up * 2f;
                    TokensModel.ShowToken(tockenID, tokenConfigId, currentTokenTarget.transform.position + shiftPosition);
                }
            }
        }
    }

    private bool PlayerHasItems(string itemName, int count)
    {
        var itemData = ItemsDB.GetItem(itemName);
        return InventoryOperationsModel.PlayerHasItems(itemData, count);
    }

    private void ShowNotificationView()
    {
        if(view == null)
        {
            view = ViewsSystem.Show(ViewConfigID.QuestObjective);
        }
    }

    private void HideNotifictionView()
    {
        if(view != null)
        {
            ViewsSystem.Hide(view);
            view = null;
        }
    }

}

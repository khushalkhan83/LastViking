using UnityEngine;
using Core.Controllers;
using Core;
using Game.Models;
using Game.Views;
using Core.Views;
using UltimateSurvival;
using System;
using Game.Controllers;
using Game.Providers;

public class TreasureHuntDigController : IController, ITreasureHuntDigController
{
    [Inject] public TreasureHuntModel huntModel { get; private set; }
    [Inject] public PlayerEventHandler playerEventHandler { get; private set; }
    [Inject] public GameUpdateModel gameUpdateModel { get; private set; }
    [Inject] public TokensModel tokensModel { get; private set; }
    [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
    [Inject] public HotBarModel HotBarModel { get; private set; }
    [Inject] public ViewsSystem ViewsSystem { get; private set; }
    [Inject] public FPManager FPManager { get; private set; }
    [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
    [Inject(true)] public TreasureHuntPlacesProvider TreasureHuntPlaces { get; private set; }

    public void Disable()
    {
        huntModel.OnBeginDigMode -= BeginDigMode;
        huntModel.OnShowDigPlace -= HuntModel_OnShowDigPlace;
        huntModel.OnDiggingTimeOut -= HideMode;
        huntModel.OnTryDigPlace -= HuntModel_OnTryDigPlace;
        FPManager.onChangeEquipedItem -= FPManager_onChangeEquipedItem;
        playerEventHandler.RaycastData.OnChange -= RaycastData_OnChange;
        gameUpdateModel.OnUpdate -= GameUpdateModel_OnUpdate;
        PlayerDeathModel.OnRevival -= OnPlayerRevival;
        PlayerDeathModel.OnRevivalPrelim -= OnPlayerRevival;
        TryHideAllPlaces();
        huntModel.SetActive(false); // hack: should be in other place
    }

    public void Enable()
    {
        huntModel.OnBeginDigMode += BeginDigMode;
        huntModel.OnShowDigPlace += HuntModel_OnShowDigPlace;
        huntModel.OnDiggingTimeOut += HideMode;
        huntModel.OnTryDigPlace += HuntModel_OnTryDigPlace;
        FPManager.onChangeEquipedItem += FPManager_onChangeEquipedItem;
        playerEventHandler.RaycastData.OnChange += RaycastData_OnChange;
        PlayerDeathModel.OnRevival += OnPlayerRevival;
        PlayerDeathModel.OnRevivalPrelim += OnPlayerRevival;
        gameUpdateModel.OnUpdate += GameUpdateModel_OnUpdate;

        if (huntModel.state == TreasureHuntState.digging)
        {
            LoadDigMode();
        }
        else
        {
            HideMode();
        }
        huntModel.SetActive(true); // hack: should be in other place
    }

    private void OnPlayerRevival()
    {
        if (nowMode == DigMode.outside)
        {
            tokensModel.ShowToken(kToken, 0, treasureZonePosition);
        }
        if (nowMode == DigMode.inside)
        {
            if (!IsHovelInHand())
                OpenView();
        }
    }

    private void TryHideAllPlaces()
    {
        if(TreasureHuntPlaces == null) return;
        for (int i = 0; i < TreasureHuntPlaces.Count; i++)
        {
            TreasureHuntPlaces[i].Hide();
        }
    }

    private void RaycastData_OnChange()
    {
        if (huntModel.state == TreasureHuntState.digging)
        {
            if (playerEventHandler.RaycastData.LastValue != null)
            {
                if (playerEventHandler.RaycastData.LastValue.HitInfo.collider != null)
                {
                    if (FPManager.currentWeapon is FPMelee)
                    {
                        if (playerEventHandler.RaycastData.LastValue.HitInfo.distance <= (FPManager.currentWeapon as FPMelee).MaxReach)
                        {
                            TreasureHuntDiggingPlace selectedPlace = playerEventHandler.RaycastData.LastValue.HitInfo.collider.GetComponent<TreasureHuntDiggingPlace>();

                            SetSelectedDigHole(selectedPlace);
                            return;
                        }
                    }
                }
            }
            SetSelectedDigHole(null);
        }
    }

    void SetSelectedDigHole(TreasureHuntDiggingPlace place)
    {
        var currentDiggingZone = TreasureHuntPlaces[huntModel.currentPlace];
        currentDiggingZone.SetVisibleOnlyPlace(place);
    }

    bool isTimeReady => true; //realTImeModel.isReady
    DateTime nowDateTime => DateTime.Now; // realTImeModel.Now()

    private void HuntModel_OnTryDigPlace(int indx)
    {
        if (huntModel.state == TreasureHuntState.digging)
        {
            if (isTimeReady)
            {
                if (!huntModel.IsHoleUsed(indx))
                {
                    if (IsFirstDig()) {
                        huntModel.OnStartDigging();
                    }
                    huntModel.SetPlaceDigged(indx);
                    HuntModel_OnShowDigPlace(huntModel.currentPlace); // [refactor logic]
                    if (huntModel.currentHole == indx)
                    {
                        DateTime now = nowDateTime;
                        huntModel.ReciveTreasure(now);
                        huntModel.DiggingTimeOut(now);
                        huntModel.CreateWaitAlarm((float)huntModel.GetEndOfWaitDate(now).Subtract(now).TotalSeconds);
                    }
                }
            }
            else
            {
                InternetErrorView.InternetErrorMessage();
            }
        }
    }

    private bool IsFirstDig() {
        for (int i = 0; i < TreasureHuntPlaces.Count; i++) {
            if (huntModel.IsHoleUsed(i)) {
                return false;
            }
        }
        return true;
    }

    private void FPManager_onChangeEquipedItem(FPObject obj)
    {
        if (huntModel.state == TreasureHuntState.digging)
        {
            if (IsInsideZone())
            {
                if (FPManager.ItemCurrent.TryGetProperty("PlayerWeaponID", out var weaponId)
                        && weaponId.PlayerWeaponID == PlayerWeaponID.tool_shovel)
                {
                    CloseView();
                    huntModel.EntedNearZone(true, true);
                }
                else
                {
                    OpenView();
                    huntModel.EntedNearZone(true, false);
                }
            }
            else
            {
                CloseView();
                huntModel.EntedNearZone(false, false);
            }
        }
    }

    private void HuntModel_OnShowDigPlace(int indx)
    {
        for (int i = 0;i< TreasureHuntPlaces.Count; i++)
        {
            if (i == indx)
            {
                TreasureHuntPlaces[i].Show(huntModel);
            }
            else
            {
                TreasureHuntPlaces[i].Hide();
            }
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        
    }
    

    void LoadDigMode()
    {
        int tpIndex = huntModel.currentPlace;
        int holeIndex = huntModel.currentHole;

        huntModel.SetDiggingPlaces(tpIndex, holeIndex);
    }

    void HideMode()
    {
        huntModel.SetDiggingPlaces(-1, -1);
        tokensModel.HideToken(kToken);
        gameUpdateModel.OnUpdate -= GameUpdateModel_OnUpdate;
        nowMode = DigMode.notInited;
        CloseView();
    }

    void BeginDigMode()
    {
        nowMode = DigMode.notInited;
        int tpIndex = UnityEngine.Random.Range(0, TreasureHuntPlaces.Count);

        TreasureHuntPlace targPlace = TreasureHuntPlaces[tpIndex];
        int holeIndex = UnityEngine.Random.Range(0, targPlace.dps.Length);

        huntModel.SetDiggingPlaces(tpIndex, holeIndex);
        gameUpdateModel.OnUpdate += GameUpdateModel_OnUpdate;
    }

    public Vector3 treasureZonePosition
    {
        get
        {
            if (huntModel.currentPlace < 0)
            {
                return Vector3.zero;
            }

            return TreasureHuntPlaces[huntModel.currentPlace].transform.position;
        }
    }

    bool IsInsideZone()
    {
        return Vector3.SqrMagnitude(playerEventHandler.Position - treasureZonePosition) <= 225;
    }

    enum DigMode { notInited,outside, inside }
    const string kToken = "treasurePlace";

    DigMode nowMode = DigMode.notInited;
    private void GameUpdateModel_OnUpdate()
    {
        if (!IsInsideZone())
        {
            // OutOfDistance
            if (nowMode != DigMode.outside)
            {
                nowMode = DigMode.outside;
                tokensModel.ShowToken(kToken, 6, treasureZonePosition);
                huntModel.EntedNearZone(false,false);
                CloseView();
            }
        }
        else
        {
            // Near
            if (nowMode != DigMode.inside)
            {
                if (nowMode!=DigMode.notInited)
                {
                    tokensModel.HideToken(kToken);
                }

                nowMode = DigMode.inside;
                bool isShovel = IsHovelInHand();
                huntModel.EntedNearZone(true, isShovel);

                if (!isShovel)
                    OpenView();
                else
                    CloseView();
            }
        }
    }

    bool IsHovelInHand()
    {
            if
            (
                PlayerMovementModel.MovementID == PlayerMovementID.Ground
                && huntModel.state == TreasureHuntState.digging
                && HotBarModel.EquipCell.IsHasItem
                && HotBarModel.EquipCell.Item.TryGetProperty("PlayerWeaponID", out var weaponId2)
                && weaponId2.PlayerWeaponID == PlayerWeaponID.tool_shovel
                && HotBarModel.EquipCell.Item.TryGetProperty("Durability", out var durab)
                && durab.Float.Current > 0
            )
        {
            return true;
        }
            else
        {
            return false;
        }
    }


    public IView View { get; private set; }
    private void OpenView()
    {
        if (View == null)
        {
            ResourceMessagesView resourceMessagesView = null;
            if (ViewsSystem.ActiveViews.TryGetValue(ViewConfigID.ResourceMessages, out var views)) {
                foreach (var view in views) {
                    resourceMessagesView = view as ResourceMessagesView;
                }
            }

            if (resourceMessagesView != null)
            {
                View = ViewsSystem.Show<TreasureHuntShovelIndicatorView>(ViewConfigID.TreasureShovel, resourceMessagesView.ContainerContent);
                View.OnHide += OnHideHandler;
            }
        }
    }

    private void OnHideHandler(IView view)
    {
        view.OnHide -= OnHideHandler;
        View = null;
    }

    private void CloseView()
    {
        if (View != null)
        {
            ViewsSystem.Hide(View);
        }
    }
}

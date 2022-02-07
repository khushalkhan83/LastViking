using System;
using System.Collections;
using System.Collections.Generic;
using AddressablesRelated;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class WeaponAssetsLoadingController : IWeaponAssetsLoadingController, IController
    {
        [Inject] public WeaponAssetsLoadingModel WeaponAssetsLoadingModel { get; set; }
        [Inject] public HotBarModel HotBarModel { get; set; }
        [Inject] public PlayerWeaponProvider PlayerWeaponProvider { get; set; }


        private AddressablePool WeaponPool => WeaponAssetsLoadingModel.WeaponPool;
        private Dictionary<int, PlayerWeaponID> weaponIDsByCellIdInHotBar = new Dictionary<int, PlayerWeaponID>();
        private bool memoryCheckScheduled;


        void IController.Enable()
        {
            // WeaponPool.Refresh();
            // InitWeaponCounter();
            // HotBarModel.ItemsContainer.OnChangeCell += OnChangeHotBarCellHandler;
            // ManageMemoryOnStartup();
        }

        void IController.Start() { }

        void IController.Disable()
        {
            // HotBarModel.ItemsContainer.OnChangeCell -= OnChangeHotBarCellHandler;
        }

        private Dictionary<PlayerWeaponID,int> loadWeaponCounter = new Dictionary<PlayerWeaponID,int>();

        private Dictionary<int,PlayerWeaponID> weaponsScheduledToUnloadByCellIds = new Dictionary<int, PlayerWeaponID>();

        private Dictionary<int,PlayerWeaponID> oldHotBar = new Dictionary<int,PlayerWeaponID>();
        private Dictionary<int,PlayerWeaponID> newHotBar = new Dictionary<int,PlayerWeaponID>();

        private void InitWeaponCounter()
        {
            loadWeaponCounter = new Dictionary<PlayerWeaponID,int>();

            var weaponIDs = Enum.GetValues(typeof(PlayerWeaponID));

            foreach (PlayerWeaponID id in weaponIDs)
            {
                if(id == PlayerWeaponID.None) continue;
                loadWeaponCounter.Add(id,0);
            }
        }

        private void InitHotBarVariables()
        {
            var hotbarItems = HotBarModel.ItemsContainer;
            var cellsCount = hotbarItems.CountCells;
            for (int i = 0; i < cellsCount; i++)
            {
                oldHotBar.Add(i,PlayerWeaponID.None);
                newHotBar.Add(i,PlayerWeaponID.None);
            }
        }

        private void ManageMemoryOnStartup()
        {
            InitHotBarVariables();
            ManageLoadingAndUnloading();
        }

        private void OnChangeHotBarCellHandler(CellModel cell) => ScheduleUnloadUnusedMemory();
        private void ScheduleUnloadUnusedMemory()
        {
            if (memoryCheckScheduled) return;

            WeaponAssetsLoadingModel.StartCoroutine(DoActionAfterOneFrame(ManageLoadingAndUnloading));
            memoryCheckScheduled = true;
        }

        private void ManageLoadingAndUnloading()
        {
            InitWeaponCounter();

            SetHotBarWeapons(newHotBar);

            var hotbarItems = HotBarModel.ItemsContainer;
            var cellsCount = hotbarItems.CountCells;

            PlayerWeaponID oldIDInCell = PlayerWeaponID.None;
            PlayerWeaponID newIDInCell = PlayerWeaponID.None;

            for (int i = 0; i < cellsCount; i++)
            {
                oldIDInCell = oldHotBar[i];
                newIDInCell = newHotBar[i];

                if(oldIDInCell == PlayerWeaponID.None && newIDInCell != PlayerWeaponID.None)
                {
                    loadWeaponCounter[newIDInCell]++;
                }
                else if (oldIDInCell != PlayerWeaponID.None && newIDInCell == PlayerWeaponID.None)
                {
                    loadWeaponCounter[oldIDInCell]--;
                }
            }

            // load and unload
            foreach (var keyValue in loadWeaponCounter)
            {
                if(keyValue.Value < 0 && !newHotBar.ContainsValue(keyValue.Key))
                {
                    WeaponPool.UnloadAsset(PlayerWeaponProvider[keyValue.Key]);
                }
                // we dont check contains value here because WeaponPool.LoadAsset not loading asset multiple times
                else if (keyValue.Value > 0)
                {
                    WeaponPool.LoadAsset(PlayerWeaponProvider[keyValue.Key]);
                }
            }
            
            memoryCheckScheduled = false;

            SetHotBarWeapons(oldHotBar);
        }

        private void SetHotBarWeapons(Dictionary<int, PlayerWeaponID> output)
        {
            var hotbarItems = HotBarModel.ItemsContainer;
            var cellsCount = hotbarItems.CountCells;
            for (int i = 0; i < cellsCount; i++)
            {
                var item = hotbarItems.GetCell(i).Item;
                TryGetPlayerWeaponID(item, out var weaponID);
                output[i] = weaponID;
            }
        }

        IEnumerator DoActionAfterOneFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }


        #region Help functions
        private bool TryGetPlayerWeaponID(SavableItem item, out PlayerWeaponID weaponID)
        {
            weaponID = PlayerWeaponID.None;

            if(item == null) return false;
            bool error = !item.TryGetProperty("PlayerWeaponID", out var result);
            if (error) return false;
            if (result == null) return false;


            weaponID = result.PlayerWeaponID;

            return true;
        }
        #endregion
    }
}

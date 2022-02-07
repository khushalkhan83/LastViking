using Core.Controllers;
using Game.Models;
using UnityEngine;
using UltimateSurvival;

namespace Game.Controllers
{
    public class SheltersController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649


#pragma warning restore 0649
        #endregion

        private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
        private CoinObjectsModel CoinObjectsModel => ModelsSystem.Instance._coinObjectsModel;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        private ShelterAttackModeModel ShelterAttackModeModel => ModelsSystem.Instance._shelterAttackModeModel;

        private const int timeInHoursToSpawnCoins = 7;

        public void OnEnable()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
            ShelterAttackModeModel.OnAttackModeFinish += OnAttackModeFinish;
        }

        public void Start() {}

        public void OnDisable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            ShelterAttackModeModel.OnAttackModeFinish -= OnAttackModeFinish;
        }

        #region Event Handlers
            
        private void OnUpdate()
        {
            if (SheltersModel.ShelterActive != ShelterModelID.None && SheltersModel.ShelterModel.SpawnConins)
            {
                if (GameTimeModel.EnviroHours >= timeInHoursToSpawnCoins)
                {
                    if (GameTimeModel.EnviroDays > GameTimeModel.GetDays(SheltersModel.TimeLastGetCoins))
                    {
                        SpawnCoinsAndSetNextSpawnTime();
                    }
                }
            }
        }

        private void OnAttackModeFinish() 
        {
            DropRewardItem();
        }

        #endregion

        #region Methods

        private void SpawnCoinsAndSetNextSpawnTime()
        {
            SpawnCoins();
            SetNextTimeSpawnTime();
        }

        private void SpawnCoins()
        {
            if (SheltersModel.ShelterActive == ShelterModelID.None) return;

            ShelterModel targetShelter = SheltersModel.ShelterModel;
            Vector3 shelterCointsPosition = targetShelter.CoinsDropPosition.transform.position;

            CoinObjectsModel.SpawnAtPosition(SheltersModel.ShelterModel.CoinsCurrent, shelterCointsPosition, shelterCointsPosition + Vector3.up, 2f, "Shelter");
        }
        private void SetNextTimeSpawnTime()
        {
            // var nextDayNum = GameTimeModel.EnviroDays + 1;
            var nextDayNum = GameTimeModel.EnviroDays;
            var nextDayInTicks = GameTimeModel.DayDurationTicks * nextDayNum;
            var nextSpawnTime = nextDayInTicks + GameTimeModel.GetHourTicks(timeInHoursToSpawnCoins);

            SheltersModel.SetTimeLastGetCoins(nextSpawnTime);
        }

        private void DropRewardItem()
        {
            var item = new SavableItem(ItemsDB.GetItem("medical_bottle_middle"));
            var playerPos = GameObject.Find("Player").transform;
            var spawnPos = playerPos.position + playerPos.forward * 2f;
            var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, spawnPos, Quaternion.identity).GetComponentInChildren<ItemPickup>();
            itemPickup.SetItemToAdd(item);
        }

        #endregion
    }
}

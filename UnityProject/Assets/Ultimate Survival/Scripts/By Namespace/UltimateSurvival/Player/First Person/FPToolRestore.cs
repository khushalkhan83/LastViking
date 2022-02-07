using Extensions;
using Game.Audio;
using Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    public enum RestorableID
    {
        None,
        Wood,
        Stone,
        Metal
    }

    public class FPToolRestore : FPMelee
    {

        #region Data
#pragma warning disable 0649

        private float _healValue;

        [SerializeField] private float _healPercent;

        [SerializeField] private Cost[] _costs;

#pragma warning restore 0649
        #endregion

        [Serializable]
        public class Cost
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private RestorableID _id;

            [SerializeField] private ItemCost[] _oneHealCost;

            public RestorableID ID => _id;
            public ItemCost[] OneHealCost => _oneHealCost;

#pragma warning restore 0649
            #endregion
        }

        [Serializable]
        public class ItemCost
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private string _name;
            [SerializeField] private int _count;

#pragma warning restore 0649
            #endregion

            public string Name { get { return _name; } }
            public int Count { get { return _count; } }
        }
        public ActionsLogModel ActionsLogModel => ModelsSystem.Instance._actionsLogModel;

        public IEnumerable<Cost> Costs => _costs;
        public ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;

        public InventoryModel InventoryModel => ModelsSystem.Instance._inventoryModel;
        private InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;

        public AudioID AudioIDOnHit;
        public bool isDecreaseDurability;

        protected override void On_Hit()
        {
            IsCanHitBuilding = true;
            IsCanHitDamageble = true;
            isDecreaseDurability = false;

            var gameObject = Player.RaycastData.Value?.GameObject;

            if (gameObject == null)
                return;

            var damageableBuilding = gameObject.GetComponent<DamageReceiver>();
            var restorable = gameObject.GetComponent<RestorableObject>();

            if (restorable)
            {
                var audioIdentifier = gameObject.GetComponent<AudioIdentifier>();
                var maxHealth = restorable.Health.HealthMax;
                var health = restorable.Health.Health;
                
                if (maxHealth > health)
                {
                    var cost = Costs.First(x => x.ID == restorable.ID);

                    if (IsHasItems(cost.OneHealCost))
                    {
                        AudioIDOnHit = restorable.AudioIDRestorable;
                        ExtractItems(cost.OneHealCost);
                        _healValue = (maxHealth / 100) * _healPercent;
                        restorable.Restore(_healValue);
                        IsCanHitBuilding = false;
                        IsCanHitDamageble = false;
                        isDecreaseDurability = true;
                    }
                    else
                    {
                        AudioIDOnHit = audioIdentifier.AudioID[UnityEngine.Random.Range(0, audioIdentifier.AudioID.Length)];
                        LeftItems(cost.OneHealCost);
                        IsCanHitBuilding = false;
                        IsCanHitDamageble = false;
                    }
                }
                else
                {
                    IsCanHitBuilding = false;
                    IsCanHitDamageble = false;
                    AudioIDOnHit = audioIdentifier.AudioID[UnityEngine.Random.Range(0, audioIdentifier.AudioID.Length)];
                }
            }

            base.On_Hit();
            OnDecreaseDurability();
        }

        protected override void OnDecreaseDurability()
        {
            if (isDecreaseDurability)
            {
                base.OnDecreaseDurability();
            }
        }

        protected override void OnHitSound()
        {
            var restorable = Player.RaycastData.Value?.GameObject.CheckNull()?.GetComponent<RestorableObject>();

            if (restorable)
            {
                AudioSystem.PlayOnce(AudioIDOnHit, Player.RaycastData.Value.HitInfo.point);
            }
        }

        private bool IsHasItems(IEnumerable<ItemCost> items)
        {
            return items.All(item => InventoryModel.ItemsContainer.IsHasItems(ItemsDB.GetItem(item.Name).Id, item.Count));
        }

        private void ExtractItems(IEnumerable<ItemCost> items)
        {
            InventoryOperationsModel.RemoveItemsFromPlayer(items);
        }

        private void LeftItems(IEnumerable<ItemCost> items)
        {
            foreach (var item in items)
            {
                var itemData = ItemsDB.GetItem(item.Name);
                ActionsLogModel.SendMessage(new MessageInventoryAttentionData(itemData));
            }
        }
    }
}

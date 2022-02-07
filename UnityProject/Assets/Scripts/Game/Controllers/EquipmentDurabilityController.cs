using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class EquipmentDurabilityController : IEquipmentDurabilityController, IController
    {
        [Inject] public EquipmentModel EquipmentModel { get; private set; }
        [Inject] public InventoryEquipmentModel InventoryEquipmentModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }

        private PlayerDamageReceiver playerDamageReceiver;

        void IController.Enable() 
        {
            playerDamageReceiver = PlayerEventHandler.GetComponent<PlayerDamageReceiver>();

            playerDamageReceiver.OnTakeDamage += OnTakeDamage;
            PlayerDeathModel.OnRevival += OnDeath;
        }

        void IController.Start() 
        {

        }

        void IController.Disable() 
        {
            playerDamageReceiver.OnTakeDamage -= OnTakeDamage;
            PlayerDeathModel.OnRevival -= OnDeath;
        }

        private void OnTakeDamage(float value, GameObject from = null) => EquipmentHitDamge();

        private void EquipmentHitDamge()
        {
            var equipedItems = InventoryEquipmentModel.GetNotBrokenEquipedItems();
            if(equipedItems != null && equipedItems.Count > 0)
            {
                var damageItem = equipedItems[Random.Range(0, equipedItems.Count)];
                if(damageItem.TryGetProperty("Durability", out var durability))
                {
                    durability.Float.Current --;
                    damageItem.SetProperty(durability);

                    if (durability.Float.Current <= 0)
                    {
                        AudioSystem.Instance.PlayOnce(AudioID.Broken, PlayerEventHandler.transform.position);
                    }
                }
            }
        }

        private void OnDeath() => EquipmentDeathDamge();

        private void EquipmentDeathDamge()
        {
            var equipedItems = InventoryEquipmentModel.GetNotBrokenEquipedItems();
            foreach (var item in equipedItems)
            {
                if (item.TryGetProperty("Durability", out var durability))
                {
                    durability.Float.Current -= durability.Float.Default * EquipmentModel.DeathDurabilityRelativeDamage;
                    item.SetProperty(durability);

                    if (durability.Float.Current <= 0)
                    {
                        AudioSystem.Instance.PlayOnce(AudioID.Broken, PlayerEventHandler.transform.position);
                    }
                }
            }
        }

    }
}

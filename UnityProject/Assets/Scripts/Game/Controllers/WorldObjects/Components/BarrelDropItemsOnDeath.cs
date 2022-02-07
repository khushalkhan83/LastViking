using System;
using System.Collections.Generic;
using Game.Audio;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using Core;

namespace Game.Controllers
{
    public class BarrelDropItemsOnDeath : MonoBehaviour, IOutlineTarget
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private TreasureID _treasureID;

        [SerializeField] private GameObject _rootObject;

        [SerializeField] private GameObject _replacementObject;

        [SerializeField] private AudioID _destroySound;

        [SerializeField] private GameObject _deathObjectSpawn;

#pragma warning restore 0649
        #endregion

        public TreasureID TreasureID => _treasureID;

        public TreasureLootModel TreasureLootModel => ModelsSystem.Instance._treasureLootModel;
  
        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public GameObject DeathObjectSpawn => _deathObjectSpawn;

        public GameObject RootObject => _rootObject;
        public GameObject ReplacementObject => _replacementObject;
        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        private AudioSystem AudioSystem => AudioSystem.Instance;
        private BlueprintObjectsModel BlueprintObjectsModel => ModelsSystem.Instance._blueprintObjectsModel;
        private CoinObjectsModel CoinObjectsModel => ModelsSystem.Instance._coinObjectsModel;
        private DropItemModel DropItemModel => ModelsSystem.Instance._dropItemModel;

        private void OnEnable()
        {
            Health.OnDeath += OnDeathHandler;
        }

        private void OnDisable()
        {
            Health.OnDeath -= OnDeathHandler;
        }

        private void OnDeathHandler()
        {
            var audioIdentifire = gameObject.GetComponent<AudioIdentifier>();

            if (ReplacementObject)
            {
                AudioSystem.PlayOnce(_destroySound);
                Instantiate(ReplacementObject, RootObject.transform.position, RootObject.transform.rotation);
            }

            List<SavableItem> dropItems = TreasureLootModel.GetLootItems(TreasureID);
            foreach (var item in dropItems)
            {
                DropItem(item);
            }
        }

        private void DropItem(SavableItem item)
        {
            if (item.Name == "blueprints")
            {
                BlueprintObjectsModel.SpawnAtPosition(item.Count, transform.position, transform.position + Vector3.up, 2f);
            }
            else if (item.Name == "coins") 
            {
                CoinObjectsModel.SpawnAtPosition(item.Count, transform.position, transform.position + Vector3.up, 2f, TreasureID.ToString());
            }
            else
            {
                DropItemModel.DropItemFloating(item, transform.position);
            }
        }

        public int GetColor()
        {
            return 0;
        }

        public bool IsUseWeaponRange()
        {
            return true;
        }
        [SerializeField]
        List<Renderer> _renderers;
        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }

        public event Action<IOutlineTarget> OnUpdateRendererList;
    }
}

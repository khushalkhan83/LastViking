using System;
using System.Collections.Generic;
using Game.Audio;
using Game.Models;
using UnityEngine;

namespace UltimateSurvival
{
    public class DroperItemsOnDeath : MonoBehaviour, IOutlineTarget
    {
        #region Data
#pragma warning disable 0649

        [ContextMenuItem("Name", "Function")]
        [SerializeField] private CellSpawnSettings[] _cellsSettings;

        [SerializeField] private GameObject _rootObject;

        [SerializeField] private GameObject _replacementObject;

        [SerializeField] private AudioID _destroySound;

        [SerializeField] private GameObject _deathObjectSpawn;

#pragma warning restore 0649
        #endregion

        public CellSpawnSettings[] CellsSettings => _cellsSettings;

        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public GameObject DeathObjectSpawn => _deathObjectSpawn;

        private int dropCount;

        //


        public GameObject RootObject => _rootObject;
        public GameObject ReplacementObject => _replacementObject;

        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        private AudioSystem AudioSystem => AudioSystem.Instance;
        private BlueprintObjectsModel BlueprintObjectsModel => ModelsSystem.Instance._blueprintObjectsModel;
        private DropItemModel DropItemModel => ModelsSystem.Instance._dropItemModel;

        //

        public void Function()
        {
            foreach (var s in _cellsSettings)
            {
                s.Function();
            }
        }

        private void OnEnable()
        {
            dropCount = 0;
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

            foreach (var cellSettings in CellsSettings)
            {
                DropItem(cellSettings.GenerateItem(ItemsDB.ItemDatabase));
            }
        }

        private void DropItem(SavableItem item)
        {
            var blueprintName = "blueprint_base";
            if (item.Name == blueprintName)
            {
                BlueprintObjectsModel.SpawnAtPosition(BlueprintObjectsModel.CountToDrop, transform.position, transform.position + Vector3.up, 2f);
            }
            else
            {
                var dropedItem = DropItemModel.DropItemFloating(item, DeathObjectSpawn.transform.position);
                onSpawnLoot?.Invoke(dropedItem);
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

        public event System.Action<GameObject> onSpawnLoot;
        public event Action<IOutlineTarget> OnUpdateRendererList;
    }
}

using Core.Storage;
using Game.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class ItemPickup : InteractableObject, IData, IOutlineTarget
    {
        [Serializable]
        public class Data : DataBase
        {
            public SavableItem ItemToAdd;
            public bool IsHasItem;

            public void SetItemToAdd(SavableItem item)
            {
                ItemToAdd = item;
                ChangeData();
            }

            public void SetIsHasItem(bool isHasItem)
            {
                IsHasItem = isHasItem;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private string m_DefaultItem;
        [SerializeField] private int m_DefaultAmount = 1;
        [SerializeField] private AudioClip m_OnDestroySound;
        [SerializeField] private float m_OnDestroyVolume = 0.5f;
        [SerializeField] private Transform _root;
        [SerializeField] private SpriteRenderer _iconSprite;
        [SerializeField] private GameObject _viewObject;

#pragma warning restore 0649
        #endregion

        public SavableItem ItemToAdd
        {
            get
            {
                return _data.ItemToAdd;
            }
            private set
            {
                _data.SetItemToAdd(value);
            }
        }

        public bool IsHasItem
        {
            get
            {
                return _data.IsHasItem;
            }
            private set
            {
                _data.SetIsHasItem(value);
            }
        }

        public Transform Root => _root;
        public SpriteRenderer IconSprite => _iconSprite;
        public GameObject ViewObject => _viewObject;
        public AudioClip DestroySound => m_OnDestroySound;
        public float DestroySoundVolume => m_OnDestroyVolume;

        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        private WorldObjectsModel WorldObjectsModel => ModelsSystem.Instance._worldObjectsModel;

        public event Action OnDataInitialize;
        public event Action<IOutlineTarget> OnUpdateRendererList;

        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return _data;
            }
        }

        public void SetItemToAdd(SavableItem savableItem)
        {
            ItemToAdd = savableItem;
            IsHasItem = true;
            UpdateSpriteIcon();
        }

        public void UUIDInitialize()
        {
            StorageModel.TryProcessing(_data);

            if (!IsHasItem)
            {
                if (!string.IsNullOrEmpty(m_DefaultItem))
                {
                    if (ItemsDB.ItemDatabase.FindItemByName(m_DefaultItem, out var itemData))
                    {
                        SetItemToAdd(new SavableItem(itemData, m_DefaultAmount));
                    }
                }
            }
            else
            {
                ItemToAdd.ItemData = ItemsDB.GetItem(ItemToAdd.Id);
            }
            UpdateSpriteIcon();
            OnDataInitialize?.Invoke();
        }

        public void PickUp()
        {
            IsHasItem = false;
            Root.GetComponent<WorldObjectModel>().Delete();

            Destroy(Root.gameObject);
        }

        public int GetColor()
        {
            return 1;
        }

        public bool IsUseWeaponRange()
        {
            return false;
        }

        [SerializeField]
        List<Renderer> _renderers;

        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }

        public void SetOutlineRenderers(List<Renderer> renderers)
        {
            _renderers = renderers;
            OnUpdateRendererList?.Invoke(this);
        }

        private void UpdateSpriteIcon()
        {
            if(_iconSprite != null)
            {
                if(ItemToAdd != null && ItemToAdd.ItemData != null)
                {
                    _iconSprite.enabled = true;
                    _iconSprite.sprite = ItemToAdd.ItemData.Icon;
                }
                else
                {
                    _iconSprite.enabled = false;
                }
            }
        }
    }
}

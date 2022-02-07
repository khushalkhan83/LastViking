using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class ItemPickUpTimeDelayController : InteractableObject, IOutlineTarget
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ItemPickUpTimeDelayModel _itemPickUpTimeDelayModel;

#pragma warning restore 0649
        #endregion

        public ItemPickUpTimeDelayModel ItemPickUpTimeDelayModel => _itemPickUpTimeDelayModel;

        private void OnEnable()
        {
            StartCoroutine(EnableProcess());
        }

        private IEnumerator EnableProcess()
        {
            yield return null;

            if (ItemPickUpTimeDelayModel.RemainingTime > 0)
            {
                SpawnProcess();
            }
            else
            {
                ItemPickUpTimeDelayModel.OnBeginSpawn += OnBeginSpawnHandler;
            }
        }

        private void OnBeginSpawnHandler()
        {
            ItemPickUpTimeDelayModel.OnBeginSpawn -= OnBeginSpawnHandler;

            SpawnProcess();
        }

        private void SpawnProcess() => StartCoroutine(UpdateRemainingTime());

        private IEnumerator UpdateRemainingTime()
        {
            while (ItemPickUpTimeDelayModel.RemainingTime > 0)
            {
                ItemPickUpTimeDelayModel.UpdateTimeRemaining(Time.deltaTime);
                yield return null;
            }

            ItemPickUpTimeDelayModel.OnBeginSpawn += OnBeginSpawnHandler;
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

        public event Action<IOutlineTarget> OnUpdateRendererList;

        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }
    }
}

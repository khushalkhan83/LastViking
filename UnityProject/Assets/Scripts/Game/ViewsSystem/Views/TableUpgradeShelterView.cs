using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;
using UltimateSurvival;
using System.Collections.Generic;
using System;

namespace Game.Views
{
    public class TableUpgradeShelterView : MonoBehaviour, IOutlineTarget
    {
        #region Data
#pragma warning disable 0649

        [ObscuredID(typeof(ShelterModelID))]
        [SerializeField] private ObscuredInt _shelterModelID;
        [SerializeField] private List<Renderer> _renderers;

#pragma warning restore 0649
        #endregion

        public ShelterModelID ShelterModelID => (ShelterModelID)(int)_shelterModelID;

        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }

        public event Action<IOutlineTarget> OnUpdateRendererList;

        void SetNewRenderersList(List<Renderer> newList)
        {
            _renderers = newList;
            OnUpdateRendererList?.Invoke(this);
        }

        public bool IsUseWeaponRange()
        {
            return true;
        }

        public int GetColor()
        {
            return 1;
        }
    }
}

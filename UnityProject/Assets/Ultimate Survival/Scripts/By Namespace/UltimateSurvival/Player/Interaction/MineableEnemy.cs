using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ObjectPooling;
using System;
using CodeStage.AntiCheat.ObscuredTypes;

namespace UltimateSurvival
{
    public class MineableEnemy : MineableObject, IResettable, IOutlineTarget
    {
        [SerializeField] private ObscuredFloat _intialAmountResources = default;
        [SerializeField] private List<Renderer> _renderers;

        public event Action<IOutlineTarget> OnUpdateRendererList;

        protected override void DestroyObject()
        {
            if (m_DestroyedObject)
            {
                Instantiate(m_DestroyedObject, transform.position + transform.TransformVector(OffsetObjectDestroyed), Quaternion.identity);
            }

            Root.SetActive(false);

            if (Destroyed != null)
                Destroyed.Send();
        }

        public void ResetObject()
        {
            Amount = _intialAmountResources;
        }

        public int GetColor() => 1;

        public List<Renderer> GetRenderers() => _renderers;

        public bool IsUseWeaponRange() => true;

    }
}

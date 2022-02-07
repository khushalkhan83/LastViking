using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PurchasesViewModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt _limitWatchGetCount;

#pragma warning restore 0649
        #endregion

        public int LimitWatchGetCount => _limitWatchGetCount;

        public int CountWatchGetCoint { get; private set; }

        public bool IsHasLimitWatch => CountWatchGetCoint < LimitWatchGetCount;

        public event Action OnChangeCountWatch;

        public void Watching()
        {
            ++CountWatchGetCoint;

            OnChangeCountWatch?.Invoke();
        }
    }
}

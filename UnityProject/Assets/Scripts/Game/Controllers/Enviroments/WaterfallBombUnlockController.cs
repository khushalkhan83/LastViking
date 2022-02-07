using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Interactables;
using Game.Models;

namespace Game.Controllers
{
    public class WaterfallBombUnlockController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private BombDestroyEnter _bombDestroyEnter;

#pragma warning restore 0649
        #endregion

        private WaterfallProgressModel WaterfallProgressModel => ModelsSystem.Instance._waterfallProgressModel;

        private void OnEnable() 
        {
            _bombDestroyEnter.OnBombDetonate += OnBombDetonate;
        }

        private void OnDisable() 
        {
            _bombDestroyEnter.OnBombDetonate -= OnBombDetonate;
        }

        private void OnBombDetonate() {
            WaterfallProgressModel.UnlocLocation();
        }

    }
}

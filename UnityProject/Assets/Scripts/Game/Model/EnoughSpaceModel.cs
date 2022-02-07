using System;
using Core;
using SimpleDiskUtils;
using UnityEngine;

namespace Game.Models
{
    
    public class EnoughSpaceModel : MonoBehaviour
    {

       #region Data
#pragma warning disable 0649

        [Tooltip("Space in MB")]
        [SerializeField] private float _requiredSpaceForSave;
        [Tooltip("Space in MB")]
        [SerializeField] private float _requiredSpaceForSaveCritical;

#pragma warning restore 0649
        #endregion

        public event Action<float> OnNotEnoughSpace;
        public event Action<float> OnNotEnoughSpaceCritical;
        public event Action OnShowNotEnoughSpaceView;
        public event Action OnShowNotEnoughSpaceCriticalView;


        public float RequiredSpaceForSave => _requiredSpaceForSave; 
        public float RequiredSpaceForSaveCritical => _requiredSpaceForSaveCritical;


        public void ShowNotEnoughSpaceView()
        {
            OnShowNotEnoughSpaceView?.Invoke();
        }
        public void ShowNotEnoughSpaceCriticalView()
        {
            OnShowNotEnoughSpaceCriticalView?.Invoke();
        }

        public bool HasEnoughSpace(out bool criticalyNotEnough)
        {
            criticalyNotEnough = false;
            if(!HasEnoughSpaceForSaveCritical())
            {
                criticalyNotEnough = true;
                return false;
            }
            else if (!HasEnoughSpaceForSave())
            {
                return false;
            }

            return true;
        }

        private bool HasEnoughSpaceForSave() => HasEnoughCheck(RequiredSpaceForSave,OnNotEnoughSpace);
        private bool HasEnoughSpaceForSaveCritical() => HasEnoughCheck(RequiredSpaceForSaveCritical,OnNotEnoughSpaceCritical);


        private bool HasEnoughCheck(float spaceForSave, Action<float> notEnoughEvent)
        {
            var spaceLeft = DiskUtils.CheckAvailableSpace();
            var enoughtSpace = spaceForSave <= spaceLeft;

            if(!enoughtSpace) notEnoughEvent?.Invoke(spaceLeft);

            return enoughtSpace;
        }
    }
}

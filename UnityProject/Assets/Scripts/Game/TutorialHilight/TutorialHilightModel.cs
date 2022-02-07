using UnityEngine;

namespace Game.Models
{
    public class TutorialHilightModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private RuntimeAnimatorController _animatorController;
        [SerializeField] private string _animationClipName;
        [SerializeField] private string _noAnimationClipName;
        
        #pragma warning restore 0649
        #endregion

        public RuntimeAnimatorController AnimatorController => _animatorController;

        public string AnimationClipName => _animationClipName;
        public string NoAnimationClipName => _noAnimationClipName;
    }
}

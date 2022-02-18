using Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Components
{
    [RequireComponent(typeof(Canvas))]
    public class TutorialHilight : MonoBehaviour
    {
        protected Canvas canvas;

        #region MonoBehaviour

        private void Awake() => Init();
        private void OnEnable() => ApplyEffect();
        private void OnDisable() => RemoveEffect();

        #endregion

        protected virtual void Init()
        {
            canvas = GetComponent<Canvas>();
        }

        [Button]
        protected virtual void ApplyEffect()
        {
            canvas.SetOverrideSorting(true);
        }

        [Button]
        protected virtual void RemoveEffect()
        {
            canvas.SetOverrideSorting(false);
        }
    }
}
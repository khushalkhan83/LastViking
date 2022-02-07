using Core.Views;
using Game.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Views
{
    public class AddStatEffectView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Animation _animation;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _text;
        [SerializeField] private AudioID _audioID;

#pragma warning restore 0649
        #endregion

        protected Animation Animation => _animation;

        public void SetIcon(Sprite sprite) => _icon.sprite = sprite;
        public void SetText(string text) => _text.text = text;
        public void SetIconColor(Color color) => _icon.color = color;
        public void SetTextColor(Color color) => _text.color = color;
        public void SetOffset(Vector3 offset) => transform.localPosition = offset;
        public void SetAngle(float angle) => transform.rotation = Quaternion.Euler(0, 0, angle);

        public void PlayRandomly()
        {
            var index = Random.Range(0, Animation.GetClipCount());
            foreach (AnimationState animationState in Animation)
            {
                if (index-- == 0)
                {
                    Animation.Play(animationState.clip.name);
                    return;
                }
            }
        }

        public event Action<AddStatEffectView> OnEndPlay;
        public void EndPlay() => OnEndPlay?.Invoke(this);
    }
}

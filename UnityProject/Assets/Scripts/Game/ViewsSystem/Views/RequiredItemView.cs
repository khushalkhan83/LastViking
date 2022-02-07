using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class RequiredItemView : MonoBehaviour
    {
        [SerializeField] private Image icon = default;
        [SerializeField] private Text countText = default;
        [SerializeField] private GameObject AttentionIcon = default;
        [SerializeField] private Animator _animator = default;

        public void SetIcon(Sprite sprite) => icon.sprite = sprite;
        public void SetCountText(string text) => countText.text = text;
        public void SetTextColor(Color color) => countText.color = color;
        public void SetActiveAttentionIcon(bool isActive) => AttentionIcon.SetActive(isActive);
        public void PlayPulseAnimation() => _animator.SetTrigger("Pulse");

    }
}

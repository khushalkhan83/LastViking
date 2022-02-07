using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class MessagesFloatingTextView : ViewBase
    {
#pragma warning disable 0649

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Text _text;
        [SerializeField] private AnimationCurve _moveCurve;
        [SerializeField] private AnimationCurve _alphaCurve;

#pragma warning restore 0649

        public AnimationCurve MoveCurve => _moveCurve;
        public AnimationCurve AlphaCurve => _alphaCurve;

        public void SetText(string text) => _text.text = text;
        public void SetAlpha(float alpha) => _canvasGroup.alpha = alpha;
        public void SetPosition(Vector3 position) => transform.position = position;
    }
}

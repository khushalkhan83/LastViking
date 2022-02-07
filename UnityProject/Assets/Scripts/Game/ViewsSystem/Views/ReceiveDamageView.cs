using Core.Views;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ReceiveDamageView : ViewBase
    {
        [SerializeField] Image _image;
        [SerializeField] Color _colorFrom;
        [SerializeField] Color _colorTo;
        [SerializeField, Range(0f,0.5f)] float _durationIn;
        [SerializeField, Range(0f, 1.5f)] float _durationOut;

        public Color ColorFrom => _colorFrom;
        public Color ColorTo => _colorTo;

        public float DurationIn => _durationIn;
        public float DurationOut => _durationOut;

        public Color Color
        {
            get
            {
                return _image.color;
            }
            set
            {
                _image.color = value;
            }
        }

        public IEnumerator FadeTo(Color to, float time)
        {
            yield return Fade(Color, to, time);
        }

        public IEnumerator Fade(Color from, Color to, float time)
        {
            var timeCurrent = 0f;
            while (timeCurrent < time)
            {
                Color = Color.Lerp(from, to, timeCurrent / time);

                yield return null;
                timeCurrent += Time.unscaledDeltaTime;
            }
        }
    }
}

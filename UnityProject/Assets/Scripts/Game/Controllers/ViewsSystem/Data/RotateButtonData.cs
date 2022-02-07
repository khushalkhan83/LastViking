using UnityEngine;

namespace Game.Controllers
{
    public class RotateButtonData : IDataViewController
    {
        public Sprite ActiveIcon { get; }
        public Sprite DefaultIcon { get; }

        public RotateButtonData(Sprite activeIcon, Sprite defaultIcon)
        {
            ActiveIcon = activeIcon;
            DefaultIcon = defaultIcon;
        }
    }
}
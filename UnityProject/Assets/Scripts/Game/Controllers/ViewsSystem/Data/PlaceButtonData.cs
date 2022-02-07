using UnityEngine;

namespace Game.Controllers
{
    public class PlaceButtonData : IDataViewController
    {
        public Sprite ActiveIcon { get; }
        public Sprite DefaultIcon { get; }

        public PlaceButtonData(Sprite activeIcon, Sprite defaultIcon)
        {
            ActiveIcon = activeIcon;
            DefaultIcon = defaultIcon;
        }
    }
}
using Core.Providers;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_sprites_default", menuName = "Providers/Sprites", order = 0)]
    public class SpritesProvider : ProviderScriptable<SpriteID, Sprite> {
        [EnumNamedArray(typeof(SpriteID))]
        [SerializeField] private Sprite[] _data;

        public override Sprite this[SpriteID key] => _data[((int)(object)key - 1)];
     }
}

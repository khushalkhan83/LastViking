using Core.Providers;
using Game.Controllers.Controllers.States.Modificators;
using Game.Models;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_modificators_minigame_new", menuName = "Providers/Modificators/Minigame", order = 0)]
    public class MinigameModificatorProvider : ProviderScriptable<MinigameID, MinigameModificator>
    {
        [EnumNamedArray(typeof(MinigameID))]
        [SerializeField] private MinigameModificator[] _data;

        public override MinigameModificator this[MinigameID key] => _data[((int)(object)key - 1)];

        public MinigameModificator GetNullValue() => new MinigameModificator();
    }
}

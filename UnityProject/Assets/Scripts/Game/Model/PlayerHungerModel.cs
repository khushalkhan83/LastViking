using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Game.Models
{
    public class PlayerHungerModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _hungerPerSecond;
        [SerializeField] private ObscuredFloat _foodToHunger;

#pragma warning restore 0649
        #endregion

        public float HungerPerSecond => _hungerPerSecond;
        public float FoodToHunger => _foodToHunger;
    }
}

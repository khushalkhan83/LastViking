using UnityEngine;

namespace Game.Providers
{
    public class TreasureHuntPlacesProvider : MonoBehaviour
    {
        [SerializeField] private TreasureHuntPlace[] _treasureHuntPlaces;

        public TreasureHuntPlace[] TreasureHuntPlaces => _treasureHuntPlaces;

        public TreasureHuntPlace this[int index] => TreasureHuntPlaces[index];
        public int Count => TreasureHuntPlaces.Length;
    }
}

using UnityEngine;

namespace Game.Models
{
    public class TutorialResourcesModel : MonoBehaviour
    {
        #region Data
        [SerializeField] private int woodExtractionModifier = 5;
        [SerializeField] private int rocksExtractionModifier = 10;
        #endregion
        public int WoodExtractionModifier => woodExtractionModifier;
        public int RocksExtractionModifier => rocksExtractionModifier;
    }
}

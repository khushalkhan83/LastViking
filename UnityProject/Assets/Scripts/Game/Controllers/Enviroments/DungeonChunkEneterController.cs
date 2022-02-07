using Game.Models;
using UnityEngine;
using Game.AI;

namespace Game.Controllers
{
    public class DungeonChunkEneterController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _chunkNumber;
        [SerializeField] private ColliderTriggerModel _colliderTriggerModel;

#pragma warning restore 0649
        #endregion

        private int ChunkNumber => _chunkNumber;
        private ColliderTriggerModel ColliderTriggerModel => _colliderTriggerModel;
        private DungeonsProgressModel DungeonsProgressModel => ModelsSystem.Instance._dungeonsProgressModel;

        #region MonoBehaviour

        private void OnEnable()
        {
            ColliderTriggerModel.OnEnteredTrigger += OnChunkEnter;
        }

        private void OnDisable()
        {
            ColliderTriggerModel.OnEnteredTrigger -= OnChunkEnter;
        }
        #endregion

        private void OnChunkEnter(Collider col)
        {
            var target = col.GetComponent<Target>();
            if (target && target.ID == TargetID.Player)
            {
                bool error = !TryGetDungeonModel(out var model);
                if(error) return;

                model.UpdateChunkProgress(ChunkNumber);
            }
        }

        private bool TryGetDungeonModel(out DungeonProgressModel model)
        {
            model = DungeonsProgressModel.GetCurrentDungeonProgressModel();
            return model != null;
        }
    }
}

using Game.Interactables;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerRespawnPointDungeonController : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private PlayerRespawnPointsActivatable _playerRespawnPointsActivatable;
        [SerializeField] private int _zoneIndex;

        private DungeonProgressModel dungeonModel;
        #pragma warning restore 0649
        #endregion

        private DungeonsProgressModel DungeonsProgressModel => ModelsSystem.Instance._dungeonsProgressModel;
        

        #region MonoBehaviour
        private void OnEnable()
        {
            dungeonModel = DungeonsProgressModel.GetCurrentDungeonProgressModel();
            if(dungeonModel == null) return;

            dungeonModel.OnChankEnter += TrySetPoint;

            TrySetPoint(_zoneIndex);
        }

        private void OnDisable()
        {
            if(dungeonModel == null) return;
            
            dungeonModel.OnChankEnter -= TrySetPoint;
        }

        private void OnChankEnterCheck()
        {
            if(_zoneIndex == dungeonModel.ControllPoint)
            {
                SetRespawnPoint();
            }
        }

        private void TrySetPoint(int obj) => OnChankEnterCheck();
        private void SetRespawnPoint() => _playerRespawnPointsActivatable.OnActivate();
        #endregion
    }
}
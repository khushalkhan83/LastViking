using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class PlayerRespawnPoints : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _initPlayerPoint;
        [SerializeField] private Transform _pointShelter;
        [SerializeField] private Transform[] _points;
        [SerializeField] private Teleport[] _teleports;

#pragma warning restore 0649
        #endregion

        public Transform InitPlayerPoint => _initPlayerPoint;
        public Transform PointShelter => _pointShelter;
        public Transform[] Points => _points;
        
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;


        public Transform GetRandomRespawnPoint() => Points[Random.Range(0, Points.Length)];

        public Transform GetClosestRespawnPoint()
        {
            if(_teleports.Length > 0)
            {
                float sqrClosestDistance = (PlayerEventHandler.transform.position - _initPlayerPoint.position).sqrMagnitude;
                Transform spawnPoint = _initPlayerPoint;
                
                foreach(var teleport in _teleports)
                {
                    if(!teleport.Active)
                        continue;

                    var sqrDistance = (PlayerEventHandler.transform.position - teleport.PlayerSpawnPosition.position).sqrMagnitude;
                    if(sqrDistance < sqrClosestDistance)
                    {   
                        sqrClosestDistance = sqrDistance;
                        spawnPoint = teleport.PlayerSpawnPosition;
                    }
                }
                return spawnPoint;
            }
            else
            {
                return _initPlayerPoint;
            }
        }
    }
}

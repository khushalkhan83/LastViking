using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Encounters
{
    public class EncounterPortal : MonoBehaviour
    {
        #region Dependencies
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        #endregion

        #region Data
        #pragma warning disable 0649
        [SerializeField] private SpawnPointProvider soketA;
        [SerializeField] private SpawnPointProvider soketB;
        
        #pragma warning restore 0649
        #endregion

        private EncountersModel EncountersModel => ModelsSystem.Instance._encountersModel;
        public void OnPlayerEnterZone()
        {
            var soket = GetMostDistantSoket();
            if(soket == null) return;

            EncountersModel.PlayerEnterZone(soket);
        }

        private ISpawnPointProvider GetMostDistantSoket()
        {
            if(PlayerEventHandler == null) return null;

            var playerTransform = PlayerEventHandler.transform;

            var distanceToSoketA = (playerTransform.position - soketA.transform.position).sqrMagnitude;
            var distanceToSoketB = (playerTransform.position - soketB.transform.position).sqrMagnitude;

            if(distanceToSoketA > distanceToSoketB)
            {
                return soketA;
            }
            else
            {
                return soketB;
            }
        }
    }
}
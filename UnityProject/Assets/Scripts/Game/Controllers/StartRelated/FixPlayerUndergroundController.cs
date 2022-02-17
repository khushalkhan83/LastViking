using System.Linq;
using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using Extensions;

namespace Game.Controllers
{
    public class FixPlayerUndergroundController : IFixPlayerUndergroundController, IController
    {
        [Inject] public PlayerRespawnPointsModel PlayerRespawnPointsModel { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public FixPlayerUndergroundModel FixPlayerUndergroundModel { get; private set; }

        private Transform PlayerTransform => PlayerEventHandler.transform;
        private Vector3 GroundPosition;
        private const float extraHight = 1f;

        void IController.Start() {}
        void IController.Enable() 
        {
            PlayerEventHandler.OnInited+=OnEnvironmentLoadedHandler;
            PlayerRespawnPointsModel.OnPlayerRespawnedAndChunksAreLoaded += OnPlayerRespawnedAndChunksAreLoadedHandler;
            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoadedHandler;
        }
        void IController.Disable() 
        {
            PlayerEventHandler.OnInited-=OnEnvironmentLoadedHandler;
            PlayerRespawnPointsModel.OnPlayerRespawnedAndChunksAreLoaded -= OnPlayerRespawnedAndChunksAreLoadedHandler;
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoadedHandler;
        }

        private void OnPlayerRespawnedAndChunksAreLoadedHandler() => TryFixPlayerUnderground();
        private void OnEnvironmentLoadedHandler() => TryFixPlayerUnderground();

        private void TryFixPlayerUnderground()
        {
            GroundPosition = Vector3.zero;
            if(PlayerIsUnderGround())
            {
                PlacePlayerAboveGround();
            }
        }

        private void PlacePlayerAboveGround()
        {
            PlayerTransform.position = GroundPosition + Vector3.up * extraHight;
        }

        private bool PlayerIsUnderGround()
        {
            LayerMask layerMask = FixPlayerUndergroundModel.GroundLayerMask;
            bool groundAbovePlayer = TryGetObjectPositionAboveTarget(PlayerTransform.position,layerMask, out float resultY);
            if(groundAbovePlayer)
            {
                GroundPosition = new Vector3(PlayerTransform.position.x, resultY, PlayerTransform.position.z);
                return true;
            }

            return false;
        }

        private bool TryGetObjectPositionAboveTarget(Vector3 target, LayerMask layerMask, out float resultY)
		{
            int rayLength = 1000;
            // 1000 miters above player
            var samplePosition = target + Vector3.up * rayLength;

			var hits = Physics.RaycastAll(samplePosition,Vector3.down,rayLength, layerMask).ToList();
			hits.Sort(
				delegate(RaycastHit hit1, RaycastHit hit2){
					return hit1.distance.CompareTo(hit2.distance);
				}
			);

			foreach (var hit in hits)
			{
				if(hit.collider.gameObject.InsideLayerMask(layerMask))
                {
                    resultY = hit.point.y;
                    return true;
                }
			}

            resultY = target.y;
			return false;
		}
    }
}

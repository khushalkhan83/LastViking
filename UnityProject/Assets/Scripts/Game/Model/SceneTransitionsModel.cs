using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Interactables;

namespace Game.Models
{
    [DefaultExecutionOrder(-1)]
    public class SceneTransitionsModel : MonoBehaviour
    {
        private List<EnvironmentSceneLoader> environmentSceneLoaders = new List<EnvironmentSceneLoader>();
        
        private void FindEnvironmentLoadersInScene()
        {
            environmentSceneLoaders = FindObjectsOfType<EnvironmentSceneLoader>().ToList();
        }

        public Transform TransitionPoint(EnvironmentTransition destionation)
        {
            if(destionation == null) return DefaultPoint();

            FindEnvironmentLoadersInScene();

            EnvironmentSceneLoader targetTransition = environmentSceneLoaders.Find(x => x.Place == destionation);

            if(targetTransition == null || targetTransition.PlaceTransform == null) return DefaultPoint();

            return targetTransition.PlaceTransform;

        }
        private Transform DefaultPoint()
        {
            PlayerRespawnPoints respawnPoints = FindObjectOfType<PlayerRespawnPoints>();
            return respawnPoints.InitPlayerPoint;
        }
    }
}

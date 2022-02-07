using Game.Models;
using UnityEditor;
using UnityEngine;
using Extensions;
using DebugActions;
using System;

namespace Game.Interactables
{
    public class EnvironmentSceneLoader : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Transform cantChangeEnvironmetnTeleportPoint;
        [SerializeField] private EnvironmentSceneID loadedSceneId;
        [SerializeField] private EnvironmentSceneID unloadedSceneId;
        [SerializeField] private LayerMask activatorLayer;
        [SerializeField] private EnvironmentTransition place;
        [SerializeField] private EnvironmentTransition destinationPlace;

#pragma warning restore 0649
        #endregion

        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
        private EnvironmentTransitionsModel EnvironmentTransitionsModel => ModelsSystem.Instance._environmentTransitionsModel;

        private bool hasEntered;

        public event Action OnPreLoadNextEnvironment;

        public EnvironmentTransition Place => place;
        public Transform PlaceTransform => cantChangeEnvironmetnTeleportPoint;

        #region MonoBehaviour
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.InsideLayerMask(activatorLayer))
            {
                var colTfm = transform;
                var relativePoint = colTfm.InverseTransformPoint(other.transform.position);

                if (relativePoint.z > 0f)
                {
                    if (!hasEntered)
                    {
                        hasEntered = true;
                        OnEnterZone();
                    }
                }
                else
                {
                    if (hasEntered)
                    {
                        hasEntered = false;
                        OnExitZone();
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Handles.yAxisColor;
            var arrowStart = transform.position;
            var arrowEnd = arrowStart + transform.forward;
            var leftWing = -transform.forward - transform.right;
            var rightWing = -transform.forward + transform.right;

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * 1);
            Gizmos.DrawRay(arrowEnd, leftWing * 0.2f);
            Gizmos.DrawRay(arrowEnd, rightWing * 0.2f);
        }
#endif

        #endregion

        private void OnEnterZone()
        {
            if (loadedSceneId != EnvironmentSceneID.None && EnvironmentTransitionsModel.CanGoToOtherLocation)
            {
                LoadEnvironmentScene(loadedSceneId);
            }
            else
            {
                MovePlayerOutOfZone();
                hasEntered = false;
            }
        }

        private void OnExitZone()
        {
            if (unloadedSceneId != EnvironmentSceneID.None)
                LoadEnvironmentScene(unloadedSceneId);
        }

        private void MovePlayerOutOfZone()
        {
            TeleportActionGeneric teleportAction = new TeleportActionGeneric("teleport", cantChangeEnvironmetnTeleportPoint.position);
            teleportAction.DoAction();
        }

        private void LoadEnvironmentScene(EnvironmentSceneID sceneId)
        {
            OnPreLoadNextEnvironment?.Invoke();
            PlayerScenesModel.TransitionToEnvironment(sceneId, destinationPlace);
        }
    }
}
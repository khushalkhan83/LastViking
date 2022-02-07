using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace UltimateSurvival
{
    public class HitZoneManager : PlayerBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private Camera m_WorldCamera;
        [SerializeField] private Vector3 _initPointShift = Vector3.zero;
        [SerializeField] private FPManager FPManager;
        [SerializeField] private RaycastManager raycaster;
#pragma warning restore 0649

        List<IHitReciver> _inZoneItems;
        IHitReciver _bestTarget = null;

        public IHitReciver target => _bestTarget;
        public List<IHitReciver> allTargets => _inZoneItems;

        float defaultDistance = 3f;
        float maxDistance
        {
            get
            {
                if (FPManager == null)
                    return defaultDistance;
                if (FPManager.currentWeapon == null)
                    return defaultDistance;
                if (FPManager.currentWeapon is FPMelee)
                {
                    return (FPManager.currentWeapon as FPMelee).MaxReach;
                }
                return defaultDistance;
            }
        }
        float maxAngle = 45f;

        static List<IHitReciver> _allSpectateObjects;
        // Update is called once per frame

        public static void RegisterObjectGlobal(IHitReciver t)
        {
            if (_allSpectateObjects == null)
                _allSpectateObjects = new List<IHitReciver>();
            if (!_allSpectateObjects.Contains(t))
            {
                _allSpectateObjects.Add(t);
                onRegister?.Invoke(t);
            }
        }

        public static void UnregisterObjectGlobal(IHitReciver t)
        {
            if (_allSpectateObjects == null)
                return;
            if (_allSpectateObjects.Contains(t))
            {
                _allSpectateObjects.Remove(t);
                onUnregister?.Invoke(t);
            }
        }

        public static System.Action<IHitReciver> onRegister;
        public static System.Action<IHitReciver> onUnregister;

        bool IsItemVisible(IHitReciver itm)
        {
            return true;
            //foreach (Renderer rrr in itm.GetRenderers())
            //{
            //    if (rrr.isVisible)
            //        return true;
            //}

            //return false;
        }

        // void Update()
        // {
        //     if (_inZoneItems == null)
        //     {
        //         _inZoneItems = new List<IHitReciver>();
        //     }

        //     _inZoneItems.Clear();
        //     _bestTarget = null;
        //     float minDistance = float.MaxValue;

        //     if (_allSpectateObjects == null)
        //         return;

        //     Vector3 myPos = m_WorldCamera.transform.TransformPoint(_initPointShift);
        //     Vector3 bestPosition = Vector3.zero;
        //     Vector3 myDirection = m_WorldCamera.transform.forward;
        //     Vector3 directionToBest = Vector3.zero;
        //     float curDist = maxDistance;

        //     foreach (IHitReciver t in _allSpectateObjects)
        //     {                
        //         if (t != null && IsItemVisible(t))
        //         {
        //             Vector3 fromTo = t.GetPosition() - myPos;
        //             float sqrtDist = fromTo.sqrMagnitude;
        //             if (sqrtDist < curDist * curDist)
        //             {
        //                 float angle = Vector3.Angle(myDirection, fromTo);
        //                 if (angle < maxAngle)
        //                 {
        //                     _inZoneItems.Add(t);
        //                     if (sqrtDist < minDistance)
        //                     {
        //                         minDistance = sqrtDist;
        //                         _bestTarget = t;
        //                         directionToBest = fromTo;
        //                         bestPosition = t.GetPosition();
        //                     }
        //                 }
        //             }
        //         }
        //     }

        //     if (_bestTarget!=null)
        //     {
        //         raycaster.SetCurrentTarget((_bestTarget as MonoBehaviour).transform,bestPosition);
        //     }
        // }
    }
}
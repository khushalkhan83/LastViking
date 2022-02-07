using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using System;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

namespace Game.AI.BehaviorDesigner
{
    public class NPCTaskContext : MonoBehaviour
    {
        [SerializeField] private Transform _rightHandHolder;

        public Transform RightHandHolder => _rightHandHolder;
    }
}


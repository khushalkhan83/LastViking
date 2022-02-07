using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using System;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;
using DG.Tweening;
using Game.Models;
using Extensions;

namespace Game.AI.BehaviorDesigner
{

    public class NPCTask : MonoBehaviour
    {
        public enum ToolHolderType
        {
            RightHandTool,
            LeftHandTool,
        }

        #region Data
#pragma warning disable 0649
        [SerializeField] private AnimationClip animationClip;
        [SerializeField] private float duration = 2;
        [SerializeField] private bool endless;
        [SerializeField] private bool compleateOnDayTimeChanged;
        [SerializeField] private float tweenDuration = 0.25f;

        [Header("Task tools")]
        [SerializeField] private Transform tool;
        [SerializeField] private ToolHolderType toolHolderType;

        [Header("Testing")]
        [SerializeField] private NPCContext testNpc;

#pragma warning restore 0649
        #endregion

        private CoroutineModel CoroutineModel => ModelsSystem.Instance._coroutineModel;

        private const string moveTriggerName = "Move";

        private Transform _toolOriginalParrentTransform;

        private NPCContext _npc;
        private Animator _npcAnimator;
        private NavMeshAgent _npcNavMeshAgent;


        public float Duration => duration;
        public bool Endless => endless;
        public bool CompleateOnDayTimeChanged => compleateOnDayTimeChanged;

        #region MonoBehaviour
        private void Awake()
        {
            var preview = transform.GetChild(0);
            if (preview != null) preview.gameObject.SetActive(false);

            if (tool == null) return;
            _toolOriginalParrentTransform = tool.parent;
        }

        private void OnDisable()
        {
            Finish();
        }
        #endregion

        public void Do(NPCContext npc)
        {
            _npc = npc;

            var tween = _npc.transform.DOMove(transform.position, tweenDuration);
            _npc.transform.DORotate(transform.eulerAngles, tweenDuration);

            _npcNavMeshAgent = npc.GetComponent<NavMeshAgent>();
            _npcNavMeshAgent.enabled = false;

            _npcAnimator = _npc.GetComponent<Animator>();
            _npcAnimator.SetTrigger(animationClip.name);

            if (tool == null) return;
            var holder = _npc.GetToolHolder(toolHolderType);

            CoroutineModel.InitCoroutine(DoAfterOneFrame(() =>
            {
                MoveToolToHolder(holder);
            }));
        }

        public void Finish()
        {
            if(_npc == null) return;

            _npcNavMeshAgent.enabled = true;

            _npcAnimator.SetTrigger(moveTriggerName);

            if (tool == null) return;

            CoroutineModel.CheckNull()?.InitCoroutine(DoAfterOneFrame(() =>
            {
                MoveToolToHolder(_toolOriginalParrentTransform);
            }));
        }

        [Button]
        void TestDo()
        {
            var behaviorTree = testNpc.GetComponent<BehaviorTree>();
            behaviorTree.enabled = false;
            Do(testNpc);
        }
        [Button]
        void TestFinish()
        {
            var behaviorTree = testNpc.GetComponent<BehaviorTree>();
            behaviorTree.enabled = true;
            Finish();
        }


        private void MoveToolToHolder(Transform holder)
        {
            if(holder == null || tool == null) return;
            
            tool.SetParent(holder);
            tool.transform.position = holder.position;
            tool.transform.rotation = holder.rotation;
        }


        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator DoAfterOneFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
    }
}


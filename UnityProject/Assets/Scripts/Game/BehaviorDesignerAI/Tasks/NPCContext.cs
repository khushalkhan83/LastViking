using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using BehaviorDesigner.Runtime;
using static Game.AI.BehaviorDesigner.NPCTask;

namespace Game.AI.BehaviorDesigner
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(Animator))]
    public class NPCContext : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private float _speed = 1;
        [SerializeField] private BehaviorTree _behaviorTree = default;

        [ReorderableList]
        [SerializeField] private List<NPCTask> _dayTasks;
        [SerializeField] private List<NPCTask> _nightTasks;
        [SerializeField] private bool _haveTasks;
        [SerializeField] private GameObject _npcDefaultPosition;

        [Header("Tool Holders")]
        [SerializeField] private Transform _rightHandHolder;
        [SerializeField] private Transform _leftHandHolder;

        [Header("Movement")]
        [SerializeField] private float _moveAnimationSpead = 1;
        
        #pragma warning restore 0649
        #endregion

        private const string speedParamName = "Speed";
        private const string dayTasksParamName = "DayTasks";
        private const string nightTasksParamName = "NightTasks";
        private const string haveTasksParamName = "HaveTasks";
        private const string defaultPositionParamName = "NPCDefaultPosition";

        private int MoveAnimationSpeedParam = Animator.StringToHash("MoveAnimationSpeed");

        private Animator _animator;

        #region MonoBehaviour
        private void Awake()
        {
            _behaviorTree.SetVariableValue(speedParamName,_speed);
            _behaviorTree.SetVariableValue(dayTasksParamName,GetTasks(true));
            _behaviorTree.SetVariableValue(nightTasksParamName,GetTasks(false));
            _behaviorTree.SetVariableValue(haveTasksParamName,_haveTasks);
            _behaviorTree.SetVariableValue(defaultPositionParamName,_npcDefaultPosition);

            _animator = GetComponent<Animator>();

            _animator.SetFloat(MoveAnimationSpeedParam,_moveAnimationSpead);
        }

        #endregion

        // TODO: add List<NPCTask> in behaviour tree variables
        private List<GameObject> GetTasks(bool day)
        {
            var tasks = day ? _dayTasks : _nightTasks;
            List<GameObject> answer = new List<GameObject>();

            foreach (var task in tasks)
            {
                answer.Add(task.gameObject);
            }
            return answer;
        }
        public Transform GetToolHolder(ToolHolderType toolHolderType)
        {
            switch (toolHolderType)
            {
                case ToolHolderType.RightHandTool:
                    return _rightHandHolder;
                case ToolHolderType.LeftHandTool:
                    return _leftHandHolder;
                default:
                    return null;
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class _TestMoving : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject renderer;

    [SerializeField] private string pirateMoveTrigger = "Move";
    [SerializeField] private string pirateStopTrigger = "Stop";
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float stoppingDist = 0.5f;

    private bool checkTargetReached = false;

    public void SetTarget(Vector3 target)
    {
        _agent.SetDestination(target);
    }

    public void StartMoving()
    {
        _animator.SetTrigger(pirateMoveTrigger);
        _agent.speed = moveSpeed;
        _agent.stoppingDistance = stoppingDist;
        checkTargetReached = true;
    }

    private void Update() 
    {
        CheckTargetReached();
    }

    private void CheckTargetReached()
    {
        if (checkTargetReached && !_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    _animator.SetTrigger(pirateStopTrigger);
                    checkTargetReached = false;
                }
            }
        }
    }

    public void DisableMoving()
    {
        checkTargetReached = false;
        _agent.enabled = false;
        renderer.SetActive(false);
    }
}
